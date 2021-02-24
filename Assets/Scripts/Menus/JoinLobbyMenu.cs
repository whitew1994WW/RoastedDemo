using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        NetworkRoomManagerExt.ClientOnConnected += HandleClientConnected;
        //NetworkRoomManagerExt.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkRoomManagerExt.ClientOnConnected -= HandleClientConnected;
        //NetworkRoomManagerExt.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void JoinGame()
    {
        string address = addressInput.text;
        NetworkRoomManager.singleton.networkAddress = address;
        Debug.Log($"Setting Ip address to {address}");
        NetworkRoomManager.singleton.StartClient();
        joinButton.interactable = false;
    }

    private void HandleClientConnected(NetworkConnection conn)
    {
        if (!SceneManager.GetActiveScene().name.StartsWith("Offline")) { return; }
        joinButton.interactable = false;
    }

    //private void HandleClientDisconnected(NetworkConnection conn)
    //{
    //    joinButton.interactable = true;
    //}

}
