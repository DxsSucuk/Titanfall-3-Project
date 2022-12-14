using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunTemplate : MonoBehaviour
{
    //bools 
    bool fire, alreadyShot;
    float timeSinceLastShot;
    public GunValues gunValues;

    //Reference
    public Camera cam;
    public RaycastHit rayHit;
    public LayerMask damageable;

    PlayerInputHandling inputHandling;

    private void Start()
    {
        inputHandling = GetComponentInParent<PlayerInputHandling>();
    }

    private void Update()
    {     
        if (gunValues.Automatic)
        {
            if (inputHandling.canShoot)
            {
                Shoot();
            }
        }
        else
        {
            if (!inputHandling.canShoot)
            {
                alreadyShot = false;
            }
            if (inputHandling.canShoot && alreadyShot == false)
            {
                alreadyShot = true;
                Shoot();
            }
        }

        if (inputHandling.shouldReload && gunValues.ammoLeft < gunValues.magSize && !gunValues.isReloading)
        {
            StartCoroutine(Reload());
        }

        timeSinceLastShot += 1f * Time.deltaTime;
    }

 
    private void Shoot()
    {
        if (!gunValues.isReloading && gunValues.ammoLeft > 0 && timeSinceLastShot > 1f / (gunValues.fireRate / 60f))
        {
            Debug.Log("shot");

            //Spread
            float x = Random.Range(-gunValues.spread, gunValues.spread);
            float y = Random.Range(-gunValues.spread, gunValues.spread);

            //Calculate Direction with Spread
            Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

            //RayCast
            if (Physics.Raycast(cam.transform.position, direction, out rayHit, gunValues.range, damageable))
            {
                Debug.Log(rayHit.collider.name);

                IDamageable damageable = rayHit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.Damage(gunValues.damage);
            }

            //ShakeCamera

            //Graphics
            gunValues.ammoLeft--;
            timeSinceLastShot = 0f;
            OnGunFire();
        }     
    }
    private IEnumerator Reload()
    {
        gunValues.isReloading = true;

        yield return new WaitForSeconds(gunValues.reloadTime);
        gunValues.ammoLeft = gunValues.magSize;
        gunValues.isReloading = false;
    }

    void OnGunFire()
    {
        //whatever logic you need here
    }
}
