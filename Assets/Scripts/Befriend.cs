using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Befriend : MonoBehaviour
{
    [SerializeField] Button befriendButton = null;
    // Start is called before the first frame update
    void Start()
    {
        befriendButton.interactable = false;
    }

    public void ToggleInteractive()
    {
        befriendButton.interactable = !befriendButton.interactable;
    }
}
