using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class RootPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    private GameObject playerInstance;

    public static event Action ServerMoveToShopScene;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            SpawnPlayer();
        }
    }


    private void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, transform.position, transform.rotation);
        playerInstance.name = this.name + "Instance";
        Debug.Log("Spawning Player");
        NetworkServer.Spawn(playerInstance, connectionToClient);
        Debug.Log("Trying to set camera following player instance");

    }

    [Command]
    public void CmdLeaveRound()
    {
        Debug.Log("Leaving Round");
        ServerMoveToShopScene?.Invoke();
    }

    #endregion


    #region Client
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            RoundOverDisplay.ClientLeaveRound += LeaveRound;
            SetCameraToFollow(playerInstance);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            RoundOverDisplay.ClientLeaveRound -= LeaveRound;
        }
    }

    [Client]
    public void LeaveRound()
    {
        CmdLeaveRound();
    }

    [ClientRpc]
    private void SetCameraToFollow(GameObject obj)
    {
        if (hasAuthority) {
            Camera.main.GetComponent<SmoothFollow>().setTarget(obj.transform);
        }
    }


    #endregion
}
