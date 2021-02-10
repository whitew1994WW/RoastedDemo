using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] [SyncVar(hook =nameof(HandleHealthUpdated))] private int currentHealth;

    public static event Action ServerOnDie;
    public static event Action<int, int> ClientOnHealthUpdated;
    #region Server

    public int GetHealth()
    {
        return currentHealth;
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    public void DealDamage(int damageToDeal)
    {
        if (currentHealth == 0) { return; }
        currentHealth = Mathf.Max(currentHealth - damageToDeal, 0);
        Debug.Log($"Health Now: {currentHealth}");
        if (currentHealth != 0) { return; }
        ServerOnDie?.Invoke();

        Debug.Log("Player Died");
    }
    #endregion
    #region client
    [Client]
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }
    #endregion
}
