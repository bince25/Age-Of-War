using UnityEngine;

public interface IGameStrategy
{
    void HandleUnitSpawn(UnitSpawner spawner, GameObject entityPrefab);
    void HandleUnitAttack(UnitController attacker, GameObject target);
    void HandleProjectileSpawn(Turret turret, GameObject entityPrefab);
    void HandleBuildingLevelUp(UnitSpawner unitSpawner, ResourceController resourceController, Building building);
    void HandleAdvanceAge(SpawnSide side);
}
