using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] ParticleSystem stoneEmitter = null;
    [SerializeField] float coolDown = 2.0f;
    [SerializeField] int damage = 25;

    bool canShoot = true;

    #region Server
    [Command]
    public void CmdShoot()
    {
        if (!canShoot) { return; }
        Debug.Log("Shooting");

        canShoot = false;
        Invoke("CooledDown", 1.0f);

        rpcShoot();
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

    private void UpdateShootBar()
    {
        //        healthBar.fillAmount = healthBar.fillAmount + Time.deltaTime / coolDown;
    }







    #endregion
}
