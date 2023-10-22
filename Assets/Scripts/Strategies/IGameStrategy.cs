using UnityEngine;

public interface IGameStrategy
{
    void HandleUnitSpawn(UnitSpawner spawner, GameObject entityPrefab);
    void HandleUnitAttack(UnitController attacker, GameObject target);
}
