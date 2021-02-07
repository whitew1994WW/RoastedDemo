using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetection : MonoBehaviour
{
    Befriend befriend;
    int numberEnemies = 0;
    // Start is called before the first frame update
    void Start()
    {
        befriend = GameObject.Find("UI/Button").GetComponent<Befriend>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.ToggleHud();
            numberEnemies += 1;
            if (numberEnemies == 1)
            {
                befriend.ToggleInteractive();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            numberEnemies -= 1;
            enemy.ToggleHud();
            if (numberEnemies == 0)
            {
                befriend.ToggleInteractive();
            }
        }
    }
}
