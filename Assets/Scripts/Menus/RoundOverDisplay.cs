using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class RoundOverDisplay : NetworkBehaviour
{
    [SerializeField] private TMP_Text winnerText = null;
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private GameObject uiObjectToDisable = null;


    public static event Action ClientLeaveRound;

    #region Client
    [Client]
    void Awake()
    {
        RoundOverHandler.ClientRoundOver += ClientHandleRoundOver;
    }

    [Client]
    void OnDestroy()
    {
        Debug.Log("Object Destroyed");
        RoundOverHandler.ClientRoundOver -= ClientHandleRoundOver;

    }

    [Client]
    public void ClientHandleRoundOver(Player winner)
    {
        gameOverDisplayParent.SetActive(true);
        uiObjectToDisable.SetActive(false);
        winnerText.text = $"{winner.name} has won the round!";
    }

    [Client]
    public void LeaveRound()
    {
        Debug.Log("Calling ClientLeaveRound event");
        ClientLeaveRound?.Invoke();
    }

    #endregion


}
