using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class TitanHealth : MonoBehaviour, IDamageable
{
    [Networked]
    private float health { get; set; }
    private float armor = 3f;
    public TextMeshProUGUI healthText;

    public GameObject deathEffect;
    public GameObject upperbody;
    public GameObject rifle;

    bool alreadyDead;

    private void Awake()
    {
        health = 100;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void DamageRPC(float damage, float armorPiercing)
    {
        if (armorPiercing > armor)
        {
            health -= damage;
        }
        else if (armorPiercing == armor)
        {
            health -= (damage * 0.2f);
        }

        if (health <= 0 && !alreadyDead)
            Die();
    }

    void Die()
    {
        SkinnedMeshRenderer[] meshes;
        meshes = upperbody.GetComponentsInChildren<SkinnedMeshRenderer>().Concat(rifle.GetComponentsInChildren<SkinnedMeshRenderer>()).ToArray();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = false;
        }

        Debug.Log("Titan Dead");
        Instantiate(deathEffect, transform.position + new Vector3(0f, 4f, 0f), Quaternion.identity);
        alreadyDead = true;
        //Destroy(gameObject, 5f);
    }
}
