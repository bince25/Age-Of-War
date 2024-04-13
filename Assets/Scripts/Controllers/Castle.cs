using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CastleController : NetworkBehaviour
{
    public string id;
    [SyncVar]
    public int maxHealth = 100;
    [SyncVar]
    public int currentHealth;
    public HealthBar healthBar;

    public SpawnSide spawnSide;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void CmdTakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        RpcTakeDamage(damage);
    }

    public void RpcTakeDamage(int damage)
    {
        healthBar.SetHealth(currentHealth);
    }
}
