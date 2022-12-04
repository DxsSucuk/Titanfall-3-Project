using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float health = 100f;
    public TextMeshProUGUI healthText;

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)    
            Die();
    }

    private void Update()
    {
        healthText.text = health.ToString();
    }

    void Die()
    {
        Debug.Log("Dead");
        Destroy(gameObject);
    }
}
