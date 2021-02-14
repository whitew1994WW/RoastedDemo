using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class RoundOverDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerText = null;
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private GameObject uiObjectToDisable = null;
    public static event Action ClientMoveToShopScene;

    void Awake()
    {
        RoundOverHandler.ClientRoundOver += ClientHandleRoundOver;
    }

    void OnDestroy()
    {
        RoundOverHandler.ClientRoundOver -= ClientHandleRoundOver;

    }

    public void LeaveRound()
    {
        ClientMoveToShopScene?.Invoke();
    }

    public void ClientHandleRoundOver(Player winner)
    {
        gameOverDisplayParent.SetActive(true);
        uiObjectToDisable.SetActive(false);
        winnerText.text = $"{winner.name} has won the round!";
    }
}
