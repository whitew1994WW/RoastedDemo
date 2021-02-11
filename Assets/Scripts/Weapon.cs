using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] ParticleSystem stoneEmitter = null;
    [SerializeField] float coolDown = 2.0f;
    [SerializeField] int damage = 25;

    public List<ParticleCollisionEvent> collisionEvents;
    bool canShoot = true;

    public static event Action<float> EnergyDepleted;

        #region Server
    [ServerCallback]
    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    [Command]
    public void CmdShoot()
    {
        if (!canShoot) { return; }
        Debug.Log("Shooting");

        canShoot = false;
        rpcShoot();


        Invoke("CooledDown", coolDown);


    }

    [Server]
    void CooledDown()
    {
        canShoot = true;
    }

    [Server]
    public int GetDamage()
    {
        return damage;
    }


    #endregion

    #region Client


    [ClientRpc]
    public void rpcShoot()
    {
        stoneEmitter.Emit(1);
        //Activate event that starts the energy bar cooldown
        Debug.Log($"Sending cooldown message if {hasAuthority}");
        if (hasAuthority)
        {
            EnergyDepleted?.Invoke(coolDown);
        }
    }
    #endregion
}
