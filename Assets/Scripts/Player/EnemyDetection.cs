using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] Befriend befriend = null;
    int numberEnemies = 0;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            numberEnemies += 1;
            if (numberEnemies == 1)
            {
                befriend.ToggleInteractive();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            numberEnemies -= 1;
            if (numberEnemies == 0)
            {
                befriend.ToggleInteractive();
            }
        }
    }
}
