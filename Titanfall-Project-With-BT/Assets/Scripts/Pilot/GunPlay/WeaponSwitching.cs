using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponSwitching : MonoBehaviour
{
    PlayerInputHandling inputHandling;
    public GameObject primary;
    public GameObject secondary;
    public GameObject antiTitan;

    public GameObject publicPrimary;
    public GameObject publicSecondary;
    public GameObject publicAntiTitan;

    public Animator animator;
    public GameObject publicWeaponSwitcher;

    public TwoBoneIKConstraint primaryRig;
    public TwoBoneIKConstraint secondaryRig;
    public TwoBoneIKConstraint antiTitanRig;


    public string[] weaponTypes = new string[] { "Primary", "Secondary", "AntiTitan" };
    public bool[] isActive = new bool[] { false, false, false };

    private void Start()
    {
        inputHandling = GetComponentInParent<PlayerInputHandling>();
        Instantiate(primary, transform);
        Instantiate(secondary, transform);
        Instantiate(antiTitan, transform);

        Instantiate(publicPrimary, publicWeaponSwitcher.transform);
        Instantiate(publicSecondary, publicWeaponSwitcher.transform);
        Instantiate(publicAntiTitan, publicWeaponSwitcher.transform);

        Select();
    }

    public void HandleAnimations()
    {
        for (int i = 0; i < weaponTypes.Length; i++)
        {
            animator.SetBool(weaponTypes[i], isActive[i]);
        }
    }

    public void Select()
    {
        int i = 0;
        foreach (Transform weapon in publicWeaponSwitcher.transform)
        {
            i++;
            if (i == inputHandling.weapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
        }

        i = 0;
        foreach(Transform weapon in transform)
        {
            i++;
            if (i == inputHandling.weapon)
            {
                weapon.gameObject.SetActive(true);
                var script = weapon.gameObject.GetComponent<GunScript>();
                script.Switch();
            }
            else
                weapon.gameObject.SetActive(false);
        }
    }

    public void HandleRig()
    {
        switch(inputHandling.weapon)
        {
            case 1:
                primaryRig.weight = 1;
                secondaryRig.weight = 0;
                antiTitanRig.weight = 0;
                break;
            case 2:
                secondaryRig.weight = 1;
                primaryRig.weight = 0;
                antiTitanRig.weight = 0;
                break;
            case 3:
                antiTitanRig.weight = 1;
                secondaryRig.weight = 0;
                primaryRig.weight = 0;
                break;
        }
    }
}
