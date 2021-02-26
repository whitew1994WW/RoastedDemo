using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

[AddComponentMenu("")]
public class NetworkRoomManagerExt : NetworkRoomManager
{
    [SerializeField] public RoundOverHandler roundOverHandlerPrefab = null;
    [SerializeField] public string shopSceneName = null;
    [SerializeField] public GameObject readyBarPrefab = null;

    public static event Action<NetworkConnection> ClientOnConnected;
    public static event Action<NetworkConnection> ClientOnDisconnected;

    public static event Action ServerAllPlayersReady;
    public static event Action ServerAllPlayersNotReady;

    private List<GameObject> readyBars = new List<GameObject>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        RootPlayer.ServerMoveToShopScene += ChangeToShopScene;
        NetworkRoomPlayerExt.ServerReadyStateChanged += ReadyStateChanged;
        StartButton.ServerStartButtonPressed += ChangeToGamePlayScene;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        RootPlayer.ServerMoveToShopScene -= ChangeToShopScene;
        NetworkRoomPlayerExt.ServerReadyStateChanged -= ReadyStateChanged;
        StartButton.ServerStartButtonPressed -= ChangeToGamePlayScene;
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (IsSceneActive(onlineScene))
        {
            Debug.Log("Spawning RoundHandler");
            RoundOverHandler roundOverHandlerInstance = Instantiate(roundOverHandlerPrefab);
            NetworkServer.Spawn(roundOverHandlerInstance.gameObject);
        }

        if (IsSceneActive(RoomScene))
        {
            // Spawn Ready Bars in
            GameObject readyBarHolder = null;
            int i = 1;
            // Loop until fetching a free ready bar slot
            while (i <= 10)
            {
                string nodeName = $"/PlayerBoxes/Panel/Player ({i})";
                readyBarHolder = GameObject.Find(nodeName);

                GameObject readyBarInstance = Instantiate(readyBarPrefab, readyBarHolder.transform);
                readyBarInstance.name = "ReadyBar";
                Debug.Log("Spawning ReadyBar");
                readyBars.Add(readyBarInstance);
                NetworkServer.Spawn(readyBarInstance);
                i++;
            }
        }
    }

    // Deletes any objects prior to scene change
    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        if (IsSceneActive(RoomScene))
        {
            // Despawn the ready bars to prevent any errors
            foreach (GameObject readyBar in readyBars)
            {
                NetworkServer.Destroy(readyBar);
            }
        } 
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("Server Add Player");
        Debug.Log($"Connection is {conn.connectionId}");
        ClientOnConnected?.Invoke(conn);
        PlayerReadyBar readyBar = null;
        GameObject readyBarParent = null;
        int i = 1;
        // Loop until fetching a free ready bar slot
        while (i <= 10)
        {
            string nodeName = $"/PlayerBoxes/Panel/Player ({i})";
            readyBarParent = GameObject.Find(nodeName);
            Debug.Log($"Finding node {nodeName}: {readyBarParent}");
            Debug.Log($"Children are {readyBarParent.transform.GetChild(0)}");
            readyBar = readyBarParent.GetComponentInChildren<PlayerReadyBar>(true);

            Debug.Log("Setting ready bar to active");
            readyBar.ServerToggleParentActive(true);

            Debug.Log($"Trying {nodeName} to see if free, id is {readyBar.GetPlayerId()}");
            int currentClientId = readyBar.GetPlayerId();
            if (currentClientId == -1)
            {
                Debug.Log($"Setting {nodeName} to player {conn.connectionId}");
                readyBar.SetPlayerId(conn.connectionId);
                break;
            }
            i++;
        }
    }

    public void ReadyStateChanged(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        PlayerReadyBar readyBar = null;
        GameObject readyBarParent = null;
        int i = 1;
        while (i <= 10)
        {
            string nodeName = $"/PlayerBoxes/Panel/Player ({i})";

            readyBarParent = GameObject.Find(nodeName);
            Debug.Log($"Fetching ready bar object from {readyBarParent}");
            readyBar = readyBarParent.GetComponentInChildren<PlayerReadyBar>(true);

            Debug.Log($"Trying {nodeName} to see if free");
            int currentClientId = readyBar.GetPlayerId();
            if (currentClientId == conn.connectionId)
            {
                Debug.Log("Calling RpcToggleReady");
                readyBar.ServerToggleIsReady();
                // If not ready then make sure the start button is disabled
                break;
            }
            i++;
        }
    }


    private int playersNamed = 0;
    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        playersNamed += 1;
        gamePlayer.name = $"Player {playersNamed}";
        Debug.Log("Player Connected. Number of players:" + playersNamed);
        return true;
    }


    public override void OnRoomStopClient()
    {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopClient();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

    }


    public override void OnRoomServerDisconnect(NetworkConnection conn)
    {
        Debug.Log($"Client has disconnected with {conn.connectionId}");
        if (SceneManager.GetActiveScene().name == "RoomScene")
        { 
            ClientOnDisconnected?.Invoke(conn);
            PlayerReadyBar readyBar = null;
            GameObject readyBarParent = null;
           
            int i = 1;
            // Loop until fetching a free ready bar
            while (i <= 10)
            {

                string nodeName = $"/PlayerBoxes/Panel/Player ({i})";

                readyBarParent = GameObject.Find(nodeName);
                readyBar = readyBarParent.GetComponentInChildren<PlayerReadyBar>(true);

                Debug.Log($"Trying {nodeName} to see if free");
                int currentClientId = readyBar.GetPlayerId();
                if (currentClientId == conn.connectionId)
                {
                    Debug.Log("Setting ready bar to deactive");
                    readyBar.ServerToggleParentActive(false);
                    Debug.Log($"Setting {nodeName} to -1");
                    readyBar.SetPlayerId(-1);
                    break;
                }
                i++;
            }
        }
        base.OnRoomServerDisconnect(conn);
    }

    public override void OnRoomStopServer()
    {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopServer();
    }

    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
        is set as DontDestroyOnLoad = true.
    */

    bool showStartButton;

    public override void OnRoomServerPlayersReady()
    {
        showStartButton = true;
        ServerAllPlayersReady?.Invoke();
    }

    public override void OnRoomServerPlayersNotReady()
    {
        ServerAllPlayersNotReady?.Invoke();
        base.OnRoomServerPlayersNotReady();
    }

    public void ChangeToGamePlayScene()
    {
        base.OnRoomServerPlayersReady();
    }

    // So that we dont use the GUI they have built
    public override void OnGUI()
    {
        return;
    }

    public void ChangeToShopScene()
    {
        Debug.Log("Change Scene to SHop");
        this.ServerChangeScene(shopSceneName);
    }
}
