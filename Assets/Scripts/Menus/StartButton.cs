using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] Button startButton = null;
    public static event Action ServerStartButtonPressed;

    private void Awake()
    {
        NetworkRoomManagerExt.ServerAllPlayersNotReady += OnPlayersNotReady;
        NetworkRoomManagerExt.ServerAllPlayersReady += OnPlayersReady;

    }

    private void OnDestroy()
    {
        NetworkRoomManagerExt.ServerAllPlayersNotReady -= OnPlayersReady;
        NetworkRoomManagerExt.ServerAllPlayersReady -= OnPlayersReady;
    }

    public void OnPlayersReady()
    {
        startButton.interactable = true;
    }

    public void OnPlayersNotReady()
    {
        startButton.interactable = false;
    }

    public void StartButtonPressed()
    {
        Debug.Log($"Trying to Press start button: {startButton.interactable}");
        if (startButton.interactable)
        {
            ServerStartButtonPressed?.Invoke();
        }
    }
}
