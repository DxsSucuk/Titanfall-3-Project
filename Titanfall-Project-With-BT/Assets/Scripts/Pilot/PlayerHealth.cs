using Fusion;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Networked]
    private float health { get; set; }
    public TextMeshProUGUI healthText;

    public GameObject rig;
    public CapsuleCollider capsuleCollider;
    public Animator animator;

    public Collider[] ragDollColliders;
    public Rigidbody[] ragDollRigidBodies;

    PilotMovement moveScript;

    void Awake()
    {
        health = 100;
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

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void DamageRPC(float damage, float armorPiercing)
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
