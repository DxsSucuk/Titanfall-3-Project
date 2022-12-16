using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Weapon/Projectile")]
public class ProjectileTemplate : ScriptableObject
{
    public new string name;

    public float damage;
    
    public bool explode;
    public float explosionRange;

    public bool useGravity;
    public float bounciness;

    public int maxCollisions;
    public float lifeTime;

    public PhysicMaterial physics_mat;



}
