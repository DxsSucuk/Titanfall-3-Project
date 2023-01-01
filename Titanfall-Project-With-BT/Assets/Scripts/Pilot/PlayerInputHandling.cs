using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandling : MonoBehaviour
{
    public bool canShoot, shouldReload;
    public int weapon;
    WeaponSwitching weaponSwitch;
    PilotMovement moveScript;

    private void Start()
    {
        weaponSwitch = GetComponentInChildren<WeaponSwitching>();
        moveScript = GetComponentInParent<PilotMovement>();
    }
    public void OnFire(InputValue value)
    {
        if (!moveScript.isSprinting)
            canShoot = value.isPressed;
    }
    public void OnReload(InputValue value)
    {
        shouldReload = value.isPressed;
    }

    public void OnPrimary()
    {
        weapon = 1;
        weaponSwitch.Select();
        weaponSwitch.HandleRig();
    }
    public void OnSecondary()
    {
        weapon = 2;
        weaponSwitch.Select();
        weaponSwitch.HandleRig();
    }
    public void OnAntiTitan()
    {
        weapon = 3;
        weaponSwitch.Select();
        weaponSwitch.HandleRig();
    }

}
