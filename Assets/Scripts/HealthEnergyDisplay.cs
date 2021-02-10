using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthEnergyDisplay : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Image energyBarImage = null;
    [SerializeField] Image healthBarImage = null;
    [SerializeField] Canvas healthEnergyCanvas = null;

    private float energyCoolDown;
    private void Awake()
    {
        Health.ClientOnHealthUpdated += HandleHealthUpdated;
        Weapon.EnergyDepleted += HandleEnergyDepleted;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        faceCamera(healthEnergyCanvas);

    }

    private void faceCamera(Canvas canvas)
    {
        canvas.transform.LookAt(canvas.transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }

    private void Update()
    {
        if (energyBarImage.fillAmount < 1) 
        {
            energyBarImage.fillAmount += Time.deltaTime / energyCoolDown;
            energyBarImage.fillAmount = Mathf.Min(energyBarImage.fillAmount, 1);
        }
    }


    private void OnDestroy()
    {
        Health.ClientOnHealthUpdated -= HandleHealthUpdated;
        Weapon.EnergyDepleted -= HandleEnergyDepleted;

    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth; 
    }

    private void HandleEnergyDepleted(float cooldDown)
    {
        energyBarImage.fillAmount = 0;
        energyCoolDown = cooldDown;
    }

    public void SetCamera(Camera camera)
    {
        Debug.Log("Setting main camera in canvas");
        healthEnergyCanvas.worldCamera = camera;
    }
}
