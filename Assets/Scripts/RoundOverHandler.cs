using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RoundOverHandler : NetworkBehaviour
{
    List<Player> alivePlayers = new List<Player>();

    #region Server
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        Player.ServerOnPlayerSpawned += HandlePlayerSpawned;
        Player.ServerOnPlayerDespawned += HandlePlayerDespawned;

    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        Player.ServerOnPlayerSpawned -= HandlePlayerSpawned;
        Player.ServerOnPlayerSpawned -= HandlePlayerDespawned;
    }

    [Server]
    private void HandlePlayerSpawned(Player newPlayer)
    {
        Debug.Log("Player added to tracker");
        alivePlayers.Add(newPlayer);
    }

    [Server]
    private void HandlePlayerDespawned(Player player)
    {
        alivePlayers.Remove(player);
        Debug.Log("Player removedfrom count");
        if (alivePlayers.Count == 1)
        {
            RoundOver();
        }
    }

    private void RoundOver()
    {
        Debug.Log("Round is finished");
    }

    [Server]
    public int CountAlivePlayers()
    {
        return alivePlayers.Count;
    }
    #endregion
}
