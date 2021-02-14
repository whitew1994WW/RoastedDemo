using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NumberPlayers : NetworkBehaviour
{
    [SyncVar(hook =nameof(UpdateText))] public int numberPlayers = 0;
    public string defaultText = "Challengers Remaining:\n";
    [SerializeField] Text numberPlayersText = null;

    [Server]
    public override void OnStartServer()
    {
        Player.ServerOnPlayerDespawned += PlayerDespawned;
        Player.ServerOnPlayerSpawned += PlayerSpawned;

    }
    [Server]
    private void PlayerDespawned(Player obj)
    {
        numberPlayers -= 1;
    }

    [Server]
    private void PlayerSpawned(Player obj)
    {
        numberPlayers += 1;
    }

    [Client]
    private void UpdateText(int oldValue, int newValue)
    {
        numberPlayersText.text = defaultText + newValue.ToString();
    }
}
