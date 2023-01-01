using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

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
    public AudioClip gunShot;
    public AudioSource audioSource;

    public bool canShoot;
    bool readyToShoot = true;
    public bool shouldReload;
    bool isReloading = false;

    public float timeBetweenShots, timeBetweenShooting;
    public float spread;
    float currentSpread;

    public int bulletsPerTap, bulletsLeft;
    int bulletsShot;

    void HandleInput()
    {

        if (readyToShoot && canShoot && bulletsLeft > 0 && !isReloading)
        {
            bulletsShot = bulletsPerTap;
            ShootRPC();
        }

        if (shouldReload && bulletsLeft < 500 && !moveScript.isDashing)
        {
            StartCoroutine(Reload());
        }
    }

    void Update()
    {
        if (enterScript.inTitan)
        {
            HandleInput();
        }
        if (moveScript.isWalking)
            currentSpread = (spread / 2);
        else
            currentSpread = spread;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void ShootRPC()
    {
        audioSource.Stop();
        readyToShoot = false;

        float xSpread = Random.Range(-currentSpread, currentSpread);
        float ySpread = Random.Range(-currentSpread, currentSpread);

        Vector3 direction = cam.transform.forward + new Vector3(xSpread, ySpread, 0);

        muzzleFlash.Play();
        muzzleFlashSecondRifle.Play();

        if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit))
        {
            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.identity) as GameObject;
            impact.transform.forward = hit.normal;

            audioSource.PlayOneShot(gunShot);

            Destroy(impact, 1.5f);

            if (hit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(hit.collider.GetComponent<RoninMovement>().TakeDamage(10));
            }

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.DamageRPC(10, 4);
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