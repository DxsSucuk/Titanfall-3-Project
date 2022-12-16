using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public ProjectileTemplate projectile;
    public Rigidbody rb;
    int collisions;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        Destroy(gameObject, projectile.lifeTime);
        rb = GetComponent<Rigidbody>();
    }

    private void Setup()
    {
        //Create a new Physic material
        projectile.physics_mat = new PhysicMaterial();
        projectile.physics_mat.bounciness = projectile.bounciness;
        projectile.physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        projectile.physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = projectile.physics_mat;

        //Set gravity
        rb.useGravity = projectile.useGravity;
    }

    void Explode()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, projectile.explosionRange);

        for (int i = 0; i < enemies.Length; i++)
        {
            IDamageable damageable = enemies[i].GetComponent<IDamageable>();
            if (damageable != null)
                damageable.Damage(projectile.damage);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (collisions > projectile.maxCollisions)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;
        if (collision.collider.CompareTag("Weapon")) return;

        //Count up collisions
        collisions++;

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        var enemy = collision.collider.GetComponent<IDamageable>(); 
        if (enemy != null)
            Explode();
    }
}
