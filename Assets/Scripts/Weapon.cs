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
        //Activate event that starts the energy bar cooldown
        EnergyDepleted?.Invoke(coolDown);

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
    }



    [ServerCallback]
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            // Ensures that there are no double collisions - otherwise this function will be called twice
            int numCollisionEvents = stoneEmitter.GetCollisionEvents(other, collisionEvents);

            int i = 0;
            while (i < numCollisionEvents)
            {
                Debug.Log(collisionEvents[i]);
                Debug.Log(collisionEvents[i].colliderComponent);
                Debug.Log($"DealingDamage to {collisionEvents[i].colliderComponent} from {this.name}");
                if (this.TryGetComponent<Weapon>(out Weapon weapon))
                {
                    if (other.TryGetComponent<Health>(out Health health))
                    {
                        health.DealDamage(weapon.GetDamage());

                    }
                }
                i++;
            }
        }
    }




    #endregion
}
