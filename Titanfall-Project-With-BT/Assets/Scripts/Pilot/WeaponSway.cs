using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class WeaponSway : NetworkBehaviour
{
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

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    void SwayWeapon()
    {
        float x = look.x * sway;
        float y = look.y * sway;

        Quaternion rotationX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotationy = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationy;

        weapon.transform.localRotation = Quaternion.Slerp(weapon.transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

}
