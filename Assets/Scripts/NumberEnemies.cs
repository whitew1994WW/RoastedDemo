using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberEnemies : MonoBehaviour
{
    public Text numberPlayersText;
    public int numberPlayers = 0;
    public string defaultText = "Challengers Remaining:\n";
    // Start is called before the first frame update
    void Start()
    {
        numberPlayersText = GameObject.Find("UI/NumberEnemies").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        CountEnemies();
    }

    private void CountEnemies()
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
