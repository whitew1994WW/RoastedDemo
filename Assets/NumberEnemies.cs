using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberEnemies : MonoBehaviour
{
    public Text numberEnemiesText;
    public int numberEnemies = 0;
    public string defaultText = "Challengers Remaining:\n";
    // Start is called before the first frame update
    void Start()
    {
        numberEnemiesText = GameObject.Find("UI/NumberEnemies").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        CountEnemies();
    }

    private void CountEnemies()
    {
        numberEnemies = 0;
        GameObject[] gameobjects = gameObject.scene.GetRootGameObjects();
        foreach (GameObject go in gameobjects)
        {
            if (go.tag == "Enemy")
            {
                numberEnemies += 1;
            }
        }
        numberEnemiesText.text = defaultText + numberEnemies.ToString();
    }
}
