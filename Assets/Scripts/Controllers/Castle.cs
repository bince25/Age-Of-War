using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public string id;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public SpawnSide spawnSide;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        healthBar.SetHealth(currentHealth);
    }
}
