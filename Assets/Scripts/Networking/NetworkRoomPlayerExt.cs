using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


[AddComponentMenu("")]
public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));

    public static event Action<NetworkConnection> ServerReadyStateChanged;
    #region Client
    [Client]
    public override void OnStartClient()
    {
        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);
        PlayerReadyBar.ClientToggleReadyEvent += ToggleReady;
        base.OnStartClient();
    }

    [Client]
    public override void OnStopClient()
    {
        PlayerReadyBar.ClientToggleReadyEvent -= ToggleReady;
        base.OnStopClient();
    }


    [Client]
    private void ToggleReady()
    {
        if (isLocalPlayer)
        {
            CmdChangeReadyState(!readyToBegin);
            CmdReadyStateChanged();
        }
    }

    [Command]
    private void CmdReadyStateChanged()
    {
        ServerReadyStateChanged?.Invoke(connectionToClient);
    }

    [Client]
    public override void OnClientExitRoom()
    {
        if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
        base.OnClientExitRoom();
    }
    #endregion


}

