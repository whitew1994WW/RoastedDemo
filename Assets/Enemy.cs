using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    public GameObject hud;
    public Image healthBar;

    void Start()
    {
        hud = GameObject.Find(this.name + "/CanvasSimple");
        hud.SetActive(false);
        print(this.name);
        print(hud);
        print(hud.transform.childCount);
        healthBar = GameObject.Find(this.name + "/CanvasSimple/Simple/Bars/Healthbar").GetComponent<Image>();
        print(healthBar);
    }

    private void checkHealth()
    {
        if (healthBar.fillAmount == 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        print(other.tag);
        print(healthBar);
        healthBar.fillAmount = (float)(healthBar.fillAmount - 0.25);
        print(healthBar.fillAmount);
        checkHealth();
    }

    public void ToggleHud()
    {
        print("Toggle: " + hud.activeSelf);
        hud.SetActive(!hud.activeSelf);
        print("Toggle: " + hud.activeSelf);
        if (hud.activeSelf==true)
        {
            healthBar = GameObject.Find(this.name + "/CanvasSimple/Simple/Bars/Healthbar").GetComponent<Image>();
        }
    }
}
