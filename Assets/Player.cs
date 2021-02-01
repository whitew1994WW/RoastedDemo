using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Tooltip("In ms-1")] [SerializeField] float moveScaling = 2;
    Vector3 moveThrow;
    public GameObject hud;
    public Image healthBar;
    public ParticleSystem throwingStones;

    bool canShoot = true;
    public float coolDown = 2.0f;


    void Start()
    {
        throwingStones = GameObject.Find("Stones Particles").GetComponent<ParticleSystem>();
        hud = GameObject.Find(this.name + "/CanvasSimple").GetComponent<GameObject>();
        healthBar = GameObject.Find(this.name + "/CanvasSimple/Simple/Bars/AttackCooldown").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessTranslation();
        ProcessShooting();
        TurnPlayer();
        UpdateShootBar();

    }

    private void UpdateShootBar()
    {
        healthBar.fillAmount = healthBar.fillAmount + Time.deltaTime / coolDown;
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
        if (Input.GetButtonDown("Fire1") && canShoot)
        {

            Shoot();

            canShoot = false;
            healthBar.fillAmount = 0;
            Invoke("CooledDown", coolDown);

        }
    }

    void Shoot()
    {
        throwingStones.Emit(1);
    }

    void CooledDown()
    {

        canShoot = true;

    }

    private void ProcessTranslation()
    {
        moveThrow = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"),
                                         0, CrossPlatformInputManager.GetAxis("Vertical"));

        Vector3 moveOffset = Time.deltaTime * moveThrow * moveScaling;

        Vector3 newPos = transform.localPosition + moveOffset;
        transform.localPosition = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hitting Something");
    }
}
