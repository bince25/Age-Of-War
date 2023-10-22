using UnityEngine;

public class LocalGameStrategy : IGameStrategy
{
    public void HandleUnitSpawn(UnitSpawner spawner, GameObject entityPrefab)
    {
        GameObject unitSpawned = spawner.InstantiateUnit(entityPrefab, spawner.spawnLocation, Quaternion.identity);
        string unitID = System.Guid.NewGuid().ToString();
        unitSpawned.gameObject.name = unitID;
        GameManager.Instance.unitsDictionary.Add(unitID, unitSpawned.GetComponent<UnitController>());

        BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.5f);
        collider.isTrigger = true;

        Rigidbody2D rigidbody = unitSpawned.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

        unitSpawned.gameObject.tag = spawner.spawnSide.ToString();
        if (spawner.spawnSide == SpawnSide.Left)
        {
            spawner.Flip(unitSpawned);
            unitSpawned.GetComponent<UnitController>().isFacingRight = true;
        }
        else
        {
            unitSpawned.GetComponent<UnitController>().isFacingRight = false;
        }
    }

    public void HandleUnitAttack(UnitController attacker, GameObject target)
    {
        if (target == null || !target.activeInHierarchy || attacker.currentHealth <= 0) return; // Check if target is available and if the unit's health is above zero

        UnitController targetController = target.GetComponent<UnitController>();
        Castle castle = target.GetComponent<Castle>();
        if (targetController != null)
        {
            targetController.TakeDamage(attacker.attackDamage);
        }
        else if (castle != null)
        {
            castle.TakeDamage(attacker.attackDamage);
        }
    }
}

