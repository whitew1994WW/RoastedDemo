using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


[AddComponentMenu("")]
public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));

    [SerializeField] private GameObject readyUpBarPrefab = null;
    private GameObject readyUpBarInstance;


    #region Client
    [Client]
    public override void OnStartClient()
    {
        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);

        base.OnStartClient();
    }

    [Client]
    public override void OnClientEnterRoom()
    {
        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
        CmdAttatchReadyUpBar();
    }

    [ClientRpc]
    private void AttatchReadyUpBar()
    {
        readyUpBarInstance = Instantiate(readyUpBarPrefab, transform.position, transform.rotation);
        GameObject parentReadyBar = GetParentReadyBar();
        if (parentReadyBar == null) { Debug.Log("Lobby Full"); return; ; }
        parentReadyBar.gameObject.transform.parent.gameObject.SetActive(true);
        Debug.Log("Assigning Client Authotiry to ready bar");
        parentReadyBar.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }

    [Client]
    private GameObject GetParentReadyBar()
    {
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
            if (!readyBar.HasPlayer()) {
                Debug.Log($"Setting {nodeName} to player");
                CmdSetReadyBarPlayer(nodeName);
                return readyBarObject;
            }
            i++;
        }
        return null;

    }

    [Command]
    private void CmdSetReadyBarPlayer(string nodeName)
    {
            GameObject readyBarObject = GameObject.Find(nodeName);
            PlayerReadyBar readyBar = readyBarObject.GetComponent<PlayerReadyBar>();
            readyBar.ServerSetPlayer(connectionToClient.identity.netId);

    }

    [Client]
    public override void OnClientExitRoom()
    {
        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
    }
    #endregion


    #region Server
    [Command]
    private void CmdAttatchReadyUpBar()
    {
        AttatchReadyUpBar();
    }
    #endregion
}

