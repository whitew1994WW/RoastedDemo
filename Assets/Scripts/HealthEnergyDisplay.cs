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

    #region Client
    [ClientCallback]
    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
        Weapon.EnergyDepleted += HandleEnergyDepleted;
    }

    //Orient the camera after all movement is completed this frame to avoid jitterin
    [ClientCallback]
    void LateUpdate()
    {
        faceCamera(healthEnergyCanvas);

    }
    [Client]
    private void faceCamera(Canvas canvas)
    {
        canvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }

    [ClientCallback]
    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
        Weapon.EnergyDepleted -= HandleEnergyDepleted;

    }

    [Client]
    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    [Client]
    private void HandleEnergyDepleted(float coolDown)
    {
        Debug.Log($"Energy Depleted, sending message to server");
        CmdEnergyDepleted(coolDown);

    }

    [ClientRpc]
    private void setEnergyBarCooldown(float fillAmount)
    {
        energyBarImage.fillAmount = fillAmount;
    }
    #endregion


    #region Server

    [Command]
    private void CmdEnergyDepleted(float coolDown)
    {
        energyBarImage.fillAmount = 0;
        energyCoolDown = coolDown;
    }

    [ServerCallback]
    private void Update()
    {
        if (energyBarImage.fillAmount < 1)
        {
            energyBarImage.fillAmount += Time.deltaTime / energyCoolDown;
            energyBarImage.fillAmount = Mathf.Min(energyBarImage.fillAmount, 1);
            setEnergyBarCooldown(energyBarImage.fillAmount);
        }
    }


    #endregion
}
