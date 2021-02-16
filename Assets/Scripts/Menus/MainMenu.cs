using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void HostGame()
    {
        NetworkRoomManager.singleton.StartHost();
    }
}
