using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class PlayerReadyBar : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleReadyBoolUpdate))] private bool readyBool = false;
    [SyncVar(hook = nameof(SetPlayer))] uint playerId = 0;
    [SerializeField] TMP_Text readyText = null;
    [SerializeField] TMP_Text notReadyText = null;

    private void HandleReadyBoolUpdate(bool oldBool, bool newBool)
    {
        readyBool = newBool;
        readyText.gameObject.SetActive(readyBool);
        notReadyText.gameObject.SetActive(!readyBool);
    }

    [Client]
    public void ToggleReady()
    {
        CmdToggleReady();
    }

    [Server]
    private void CmdToggleReady()
    {
        RpcToggleReady();
    }

    [ClientRpc]
    void RpcToggleReady()
    {
        readyBool = !readyBool;
    }

    internal bool HasPlayer()
    {
        if (playerId == 0) {
            return false;
        } else {
            return true;
        }
    }

    [Client]
    internal void SetPlayer(uint oldNetId, uint newNetId)
    {
        playerId = newNetId;
    }

    [Server]
    internal void ServerSetPlayer(uint netId)
    {
        playerId = netId;
    }
}
