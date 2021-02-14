using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SerializeField] [SyncVar(hook =nameof(HandleHealthUpdated))] private int currentHealth;

    public event Action ServerOnDie;
    public static event Action ServerStaticOnDie;
    public event Action<int, int> ClientOnHealthUpdated;
    #region Server
    public int GetHealth()
    {
        return currentHealth;
    }

    [Server]
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageToDeal)
    {
        if (currentHealth == 0) { return; }
        currentHealth = Mathf.Max(currentHealth - damageToDeal, 0);
        Debug.Log($"Health Now: {currentHealth}");
        if (currentHealth != 0) { return; }
        ServerOnDie?.Invoke();
        ServerStaticOnDie?.Invoke();
        Debug.Log("Player Died");
    }
    #endregion
    #region client
    // Called on client, but broadcast to all
    [Client]
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }
    #endregion
}
