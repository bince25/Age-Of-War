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
    public void HandleProjectileSpawn(Turret turret, GameObject entityPrefab)
    {
        GameObject projectile = turret.InstantiateProjectile(entityPrefab, turret.firePoint.position, turret.firePoint.rotation);
        projectile.GetComponent<Projectile>().Seek(turret.target);
    }

    public void HandleBuildingLevelUp(UnitSpawner unitSpawner, ResourceController resourceController, Building building)
    {
        Debug.Log(building.buildingType + " Leveled Up!");
        if (building.buildingType == BuildingType.Farm)
        {
            building.farmLevel++;
            building.farmGoldMultiplier *= 1.2f;
            building.farmFoodMultiplier *= 1.4f;
            building.ChangeFarmMultiplier(building.farmGoldMultiplier);
        }
        else if (building.buildingType == BuildingType.Barrack)
        {
            building.barrackLevel++;
            if (building.barrackLevel == 1 && resourceController.ageText.text == Ages.Stone.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (building.barrackLevel == 2 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (building.barrackLevel == 3 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (building.barrackLevel == 4 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (building.barrackLevel == 5 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else if (building.barrackLevel == 6 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnTankUnit = true;
            }
            else
            {
                unitSpawner.canSpawnTankUnit = false;
            }
        }
        else if (building.buildingType == BuildingType.Stable)
        {
            building.stableLevel++;
            if (building.stableLevel == 1 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (building.stableLevel == 2 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (building.stableLevel == 3 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (building.stableLevel == 4 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else if (building.stableLevel == 5 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnMountedUnit = true;
            }
            else
            {
                unitSpawner.canSpawnMountedUnit = false;
            }
        }
        else if (building.buildingType == BuildingType.Workshop)
        {
            building.workshopLevel++;
            if (building.workshopLevel == 1 && resourceController.ageText.text == Ages.Iron.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (building.workshopLevel == 2 && resourceController.ageText.text == Ages.Medieval.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (building.workshopLevel == 3 && resourceController.ageText.text == Ages.Renaissance.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (building.workshopLevel == 4 && resourceController.ageText.text == Ages.Modern.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else if (building.workshopLevel == 5 && resourceController.ageText.text == Ages.Future.ToString() + " Age")
            {
                unitSpawner.canSpawnSiegeUnit = true;
            }
            else
            {
                unitSpawner.canSpawnSiegeUnit = false;
            }
        }
    }

    public void HandleBuildingCreation(Building building)
    {
        building.HandleLevelUp();
    }

    public void HandleAdvanceAge(SpawnSide side)
    {
        ResourceController resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        if (side == SpawnSide.Left)
        {
            GameStateManager.Instance.leftAge++;
            resourceController.currentAge++;
            resourceController.ageText.text = GameStateManager.Instance.leftAge.ToString() + " Age";
        }
        else
        {
            resourceController.currentAge++;
            GameStateManager.Instance.rightAge++;
            resourceController.ageText.text = GameStateManager.Instance.rightAge.ToString() + " Age";
        }

    }
}

