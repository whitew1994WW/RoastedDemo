using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    [Tooltip("In ms-1")] [SerializeField] float moveScaling = 2;
    Vector3 moveThrow;
    public GameObject hud;
    public Image healthBar;
    public ParticleSystem throwingStones;

    bool canShoot = true;
    bool hasFocus = true;
    public float coolDown = 2.0f;


    void Start()
    {
        throwingStones = GameObject.Find("Stones Particles").GetComponent<ParticleSystem>();
//        hud = GameObject.Find(this.name + "/CanvasSimple").GetComponent<GameObject>();
 //       healthBar = GameObject.Find(this.name + "/CanvasSimple/Simple/Bars/AttackCooldown").GetComponent<Image>();
    }

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
        UpdateShootBar();

    }

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
        Debug.Log($"Screen Focus is {hasFocus}");
    }

    private void UpdateShootBar()
    {
        //        healthBar.fillAmount = healthBar.fillAmount + Time.deltaTime / coolDown;
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
//            healthBar.fillAmount = 0;
            Invoke("CooledDown", coolDown);

        }
    }

    void Shoot()
    {
//        throwingStones.Emit(1);
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

    private void checkHealth()
    {
        //if (healthBar.fillAmount == 0)
        //{
        //    Destroy(this.gameObject);
        //}
    }

    private void OnParticleCollision(GameObject other)
    {
        print(other.tag);
    //    print(healthBar);
   //     healthBar.fillAmount = (float)(healthBar.fillAmount - 0.25);
    //    print(healthBar.fillAmount);
        checkHealth();
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<SmoothFollow>().setTarget(gameObject.transform);
    }
}
