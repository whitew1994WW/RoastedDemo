using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

[AddComponentMenu("")]
public class NetworkRoomManagerExt : NetworkRoomManager
{
    [SerializeField] public RoundOverHandler roundOverHandlerPrefab = null;
    [SerializeField] public string shopSceneName = null;

    public static event Action<NetworkConnection> ClientOnConnected;
    public static event Action<NetworkConnection> ClientOnDisconnected;

    public override void OnStartServer()
    {
        base.OnStartServer();
        RootPlayer.ServerMoveToShopScene += ChangeToShopScene;
        NetworkRoomPlayerExt.ReadyStateChanged += ReadyStateChanged;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        RootPlayer.ServerMoveToShopScene -= ChangeToShopScene;
        NetworkRoomPlayerExt.ReadyStateChanged -= ReadyStateChanged;
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            Debug.Log("Spawning RoundHandler");
            RoundOverHandler roundOverHandlerInstance = Instantiate(roundOverHandlerPrefab);
            NetworkServer.Spawn(roundOverHandlerInstance.gameObject);
        }
    }

    public void ReadyStateChanged(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        PlayerReadyBar readyBar = null;
        GameObject readyBarObject = null;
        int i = 1;
        while (i <= 10)
        {
            string nodeName = $"PlayerBoxes/Panel/Player ({i})/PlayerReadyUpBar";

            readyBarObject = GameObject.Find(nodeName);
            Debug.Log($"Fetching ready bar object from {readyBarObject}");
            readyBar = readyBarObject.GetComponent<PlayerReadyBar>();

            Debug.Log($"Trying {nodeName} to see if free");
            int currentClientId = readyBar.GetPlayerId();
            if (currentClientId == conn.connectionId)
            {
                Debug.Log("Calling RpcToggleReady");
                readyBar.ServerToggleIsReady();
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
        Debug.Log($"Connection is {conn.connectionId}");
        ClientOnConnected?.Invoke(conn);
        PlayerReadyBar readyBar = null;
        GameObject readyBarObject = null;
        int i = 1;
        // Loop until fetching a free ready bar
        while (i <= 10)
        {
            string nodeName = $"/PlayerBoxes/Panel/Player ({i})/PlayerReadyUpBar";
            readyBarObject = GameObject.Find(nodeName);
            readyBar = readyBarObject.GetComponent<PlayerReadyBar>();

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


    public override void OnRoomServerDisconnect(NetworkConnection conn)
    {
        Debug.Log($"Client has disconnected with {conn.connectionId}");
        if (SceneManager.GetActiveScene().name == "RoomScene")
        { 
            ClientOnDisconnected?.Invoke(conn);
            PlayerReadyBar readyBar = null;
            GameObject readyBarObject = null;
            int i = 1;
            // Loop until fetching a free ready bar
            while (i <= 10)
            {

                string nodeName = $"/PlayerBoxes/Panel/Player ({i})/PlayerReadyUpBar";

                readyBarObject = GameObject.Find(nodeName);
                readyBar = readyBarObject.GetComponent<PlayerReadyBar>();

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
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
        showStartButton = true;
#endif
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
        {
            // set to false to hide it in the game scene
            showStartButton = false;

            ServerChangeScene(GameplayScene);
        }
    }

    public void ChangeToShopScene()
    {
        Debug.Log("Change Scene to SHop");
        this.ServerChangeScene(shopSceneName);
    }
}
