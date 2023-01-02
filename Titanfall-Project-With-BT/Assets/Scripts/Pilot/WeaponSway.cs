using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class WeaponSway : NetworkBehaviour
{
    
    //TODO:: discuss if this class is needed? I could not find any usage of this class.
    public float smooth;
    public float sway;

    Vector2 look;

    public GameObject weapon;

    // Update is called once per frame
    void Update()
    {
        if (!HasStateAuthority) return;

        SwayWeapon();
    }

    void SwayWeapon()
    {
        float x = look.x * sway;
        float y = look.y * sway;

        Quaternion rotationX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        weapon.transform.localRotation = Quaternion.Slerp(weapon.transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

}
