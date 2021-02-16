using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RoundOverHandler : NetworkBehaviour
{
    List<Player> alivePlayers = new List<Player>();

    public static event Action<Player> ClientRoundOver;
    public static event Action<Player> ServerRoundOver;


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

    [Server]
    private void RoundOver()
    {
        Debug.Log("Round is finished");
        ServerRoundOver?.Invoke(alivePlayers[0]);
        RpcRoundOver(alivePlayers[0]);
    }

    [Server]
    public int CountAlivePlayers()
    {
        return alivePlayers.Count;
    }
    #endregion

    #region client
    [ClientRpc]
    private void RpcRoundOver(Player winner)
    {
        ClientRoundOver?.Invoke(winner);
    }

    #endregion
}
