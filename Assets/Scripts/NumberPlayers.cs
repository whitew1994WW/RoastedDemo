using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NumberPlayers : NetworkBehaviour
{
    public int numberPlayers = 0;
    public string defaultText = "Challengers Remaining:\n";

    [SerializeField] Text numberPlayersText = null;
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Health.ServerOnDie += CountPlayers;
    }


    private void CountPlayers()
    {
        numberPlayers = 0;
        GameObject[] gameobjects = gameObject.scene.GetRootGameObjects();
        foreach (GameObject go in gameobjects)
        {
            if (go.tag == "Player")
            {
                numberPlayers += 1;
            }
        }
        numberPlayersText.text = defaultText + numberPlayers.ToString();
    }
}
