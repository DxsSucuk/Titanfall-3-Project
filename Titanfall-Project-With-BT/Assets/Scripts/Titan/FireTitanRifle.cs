using System.Collections;
using System.Collections.Generic;
using Fusion;
using Networking.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireTitanRifle : NetworkBehaviour
{
    public EnterVanguardTitan enterScript;
    public VanguardMovement moveScript;

    public Animator Arms;
    public Animator titanAnimator;
    public Animator XO16Animator;
    public Animator secondXO16Animator;

    public Camera cam;
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlashSecondRifle;
    public GameObject impactEffect;
    
    bool readyToShoot = true;
    bool isReloading;

    public float timeBetweenShots, timeBetweenShooting;
    public float spread;

    public int bulletsPerTap, bulletsLeft;
    int bulletsShot;

    public PlayerInput controls;

    private void Start()
    {
        controls = GetComponent<PlayerInput>();
    }

    void HandleInput()
    {

        if (readyToShoot && moveScript.networkTitanInput.Buttons.IsSet(TitanButtons.Shoot) && bulletsLeft > 0 && !isReloading)
        {
            bulletsShot = bulletsPerTap;
            ShootRPC();
        }

        if (moveScript.networkTitanInput.Buttons.IsSet(TitanButtons.Reload) && bulletsLeft < 500 && !moveScript.isDashing)
        {
            StartCoroutine(Reload());
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;
        
        if (enterScript.inTitan)
        {
            HandleInput();
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    private void ShootRPC()
    {
        readyToShoot = false;

        float xSpread = Random.Range(-spread, spread);
        float ySpread = Random.Range(-spread, spread);

        Vector3 direction = cam.transform.forward + new Vector3(xSpread, ySpread, 0);

        muzzleFlash.Play();
        muzzleFlashSecondRifle.Play();

        if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit))
        {
            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.identity) as GameObject;
            impact.transform.forward = hit.normal;

            Destroy(impact, 1.5f);

            if (hit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(hit.collider.GetComponent<RoninMovement>().TakeDamage(10));
            }
        }

        bulletsLeft--;
        bulletsShot--;

        if (!IsInvoking("ResetShot") && !readyToShoot)
        {
            Invoke("ResetShot", timeBetweenShooting);
        }

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("ShootRPC", timeBetweenShots);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Arms.SetTrigger("reload");
        titanAnimator.SetTrigger("reload");
        XO16Animator.SetTrigger("reload");
        secondXO16Animator.SetTrigger("reload");

        yield return new WaitForSeconds(2.5f);

        bulletsLeft = 500;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
}