using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RootPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    private GameObject playerInstance;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnPlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetCameraToFollow(playerInstance);
    }

    private void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, transform.position, transform.rotation);
        playerInstance.name = this.name + "Instance";
        Debug.Log("Spawning Player");
        NetworkServer.Spawn(playerInstance, connectionToClient);
        Debug.Log("Trying to set camera following player instance");

    }


    #endregion


    #region Client

    [ClientRpc]
    private void SetCameraToFollow(GameObject obj)
    {
        if (hasAuthority) {
            Camera.main.GetComponent<SmoothFollow>().setTarget(obj.transform);
        }
    }
    #endregion
}
