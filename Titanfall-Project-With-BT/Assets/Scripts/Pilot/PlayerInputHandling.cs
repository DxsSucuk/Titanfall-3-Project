using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandling : MonoBehaviour
{
    public bool canShoot, shouldReload;
    public int weapon;
    public void OnFire(InputValue value)
    {
        canShoot = value.isPressed;
    }
    public void OnReload(InputValue value)
    {
        shouldReload = value.isPressed;
    }

    public void OnPrimary()
    {
        weapon = 1;
    }
    public void OnSecondary()
    {
        weapon = 2;
    }
    /*public void OnAntiTitan()
    {
        weapon = 3;
    }*/

}
