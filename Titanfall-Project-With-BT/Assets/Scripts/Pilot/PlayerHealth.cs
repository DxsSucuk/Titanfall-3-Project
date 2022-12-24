using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float health = 100f;
    public TextMeshProUGUI healthText;

    public GameObject rig;
    public CapsuleCollider capsuleCollider;
    public Animator animator;

    public Collider[] ragDollColliders;
    public Rigidbody[] ragDollRigidBodies;

    PilotMovement moveScript;

    void Awake()
    {
        ragDollColliders = rig.GetComponentsInChildren<Collider>();
        ragDollRigidBodies = rig.GetComponentsInChildren<Rigidbody>();

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody body in ragDollRigidBodies)
        {
            body.isKinematic = true;
        }

        moveScript = GetComponentInParent<PilotMovement>();
    }

    public void Damage(float damage, float armorPiercing)
    {
        health -= damage;
        if (health <= 0)    
            Die();
    }

    private void Update()
    {
        healthText.text = health.ToString();
    }

    void ActivateRagdoll()
    {
        animator.enabled = false;

        ragDollColliders = rig.GetComponentsInChildren<Collider>();
        ragDollRigidBodies = rig.GetComponentsInChildren<Rigidbody>();

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody body in ragDollRigidBodies)
        {
            body.isKinematic = false;
        }

        capsuleCollider.enabled = false;
    }

    void Die()
    {
        ActivateRagdoll();
        Debug.Log("PilotDead");
        moveScript.canMove = false;
        //Destroy(gameObject);
    }
}
