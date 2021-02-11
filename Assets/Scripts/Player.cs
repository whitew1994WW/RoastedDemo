﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using Mirror;
using System.ComponentModel;

public class Player : NetworkBehaviour
{
    [SerializeField] float moveScaling = 2;
    Vector3 moveThrow;
    [SerializeField] Weapon weapon = null;
    [SerializeField] Health health = null;
    [SerializeField] ParticleSystem part = null;

    bool hasFocus = true;
    public float coolDown = 2.0f;



    #region Client

    // Update is called once per frame
    [ClientCallback]
    void Update()
    {
        //Only update movement if it is the local player doing so
        if (!isLocalPlayer | !hasFocus | !hasAuthority)
        {
            return;
        }


        ProcessTranslation();
        ProcessShooting();
        TurnPlayer();

    }

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
        Debug.Log($"Screen Focus is {hasFocus}");
    }

    private void TurnPlayer()
    {

        float h = Input.mousePosition.x - Screen.width / 2;
        float v = Input.mousePosition.y - Screen.height / 2;
        float angle = -Mathf.Atan2(v, h) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle + 90, 0);
    }

    private void ProcessShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            weapon.CmdShoot();

        }
    }

    private void ProcessTranslation()
    {
        moveThrow = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"),
                                         0, CrossPlatformInputManager.GetAxis("Vertical"));

        Vector3 moveOffset = Time.deltaTime * moveThrow * moveScaling;

        Vector3 newPos = transform.localPosition + moveOffset;
        transform.localPosition = newPos;
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<SmoothFollow>().setTarget(gameObject.transform);
    }


    #endregion

    #region Server

    [ServerCallback]
    public override void OnStartServer()
    {
        Health.ServerOnDie += CmdCheckDeath;
    }

    [Server]
    public void SetPlayerName(string newName)
    {
        Debug.Log($"Player.cs - SetPlayerName({newName})");
        this.name = newName;
    }

    [ServerCallback]
    public override void OnStopServer()
    {
        Health.ServerOnDie -= CmdCheckDeath;
    }

    [Command]
    private void CmdCheckDeath()
    {
        if (health.GetHealth() == 0) {
            OnDeath();
        }
    }

    [ClientRpc]
    private void OnDeath()
    {

        Destroy(this.gameObject);
    }

    // Function called when hit by a particle attack
    [ServerCallback]
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player" & other.name != this.name)
        {

            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            // If this particle system has registered a collision then this is the wron function call
            // as it is called on both the player firing and the player being hit
            if (numCollisionEvents != 0) { return; }

            Debug.Log($"DealingDamage to {this.name} from {other.name}");
            if (other.TryGetComponent<Weapon>(out Weapon weapon))
            {
                if (this.TryGetComponent<Health>(out Health health))
                {
                    health.DealDamage(weapon.GetDamage());


                }
            }

        }
    }

    #endregion
}
