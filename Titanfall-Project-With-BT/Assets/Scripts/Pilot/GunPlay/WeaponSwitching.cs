using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    PlayerInputHandling inputHandling;

    // Update is called once per frame

    private void Start()
    {
        inputHandling = GetComponentInParent<PlayerInputHandling>();
    }

    private void Update()
    {
        Select();
    }

    void Select()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            i++;
            if (i == inputHandling.weapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
        }
    }
}
