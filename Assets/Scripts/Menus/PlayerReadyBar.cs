using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class PlayerReadyBar : NetworkBehaviour
{
    [SerializeField] TMP_Text readyText = null;
    [SerializeField] TMP_Text notReadyText = null;
    [SyncVar(hook=nameof(UpdatePlayerId))] private int playerId = -1;
    [SyncVar(hook=nameof(ClientToggleParentActive))] private bool parentActive = false;
    [SyncVar(hook = nameof(ClientToggleIsReady))] private bool isReady = false;

    public static event Action ClientToggleReadyEvent;

    [Client]
    private void ClientToggleIsReady(bool oldIsReady, bool newIsReady)
    {
        UpdateReadyBool(isReady);
    }

    [Client]
    private void UpdateReadyBool(bool readyBool)
    {
        readyText.gameObject.SetActive(readyBool);
        notReadyText.gameObject.SetActive(!readyBool);
    }

    [Client]
    public void TriggerToggleReady()
    {
        Debug.Log("Has authority, toggling ready");
        ClientToggleReadyEvent?.Invoke();
    }

    [Client]
    private void UpdatePlayerId(int oldVar, int newVar)
    {
        Debug.Log("Setting Player id on client");
        playerId = newVar;
    }

    [Server]
    public void SetPlayerId(int newId)
    {
        Debug.Log("setting player ID on server");
        playerId = newId;
    }

    [Server]
    public void ServerToggleIsReady()
    {
        isReady = !isReady;
    }

    [Server]
    public int GetPlayerId()
    {
        return playerId;
    }

    [Server]
    public void ServerToggleParentActive(bool activeBool)
    {
        Debug.Log("Setting parent active in ToggleParentActive");
        parentActive = activeBool;
    }

    public void ClientToggleParentActive(bool oldBool, bool newBool)
    {
        Debug.Log("Setting parent active in hook call");
        gameObject.transform.parent.gameObject.SetActive(newBool);
    }
}
