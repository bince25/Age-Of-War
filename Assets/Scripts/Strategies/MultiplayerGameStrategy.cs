
using UnityEngine;

public class OnlineGameStrategy : IGameStrategy
{
    public void HandleUnitSpawn(UnitSpawner spawner, GameObject entityPrefab)
    {
        string id = System.Guid.NewGuid().ToString();
        SpawnMessage message = new SpawnMessage
        {
            action = "spawnUnit",
            data = new SpawnData
            {
                id = id,
                type = entityPrefab.name,
                side = spawner.spawnSide.ToString(),
                health = entityPrefab.GetComponent<UnitController>().maxHealth,
                gameId = PlayerPrefs.GetString("gameId")
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
    }

    public void HandleUnitAttack(UnitController attacker, GameObject target)
    {
        if (target == null || !target.activeInHierarchy || attacker.currentHealth <= 0) return; // Check if target is available and if the unit's health is above zero

        UnitController targetController = target.GetComponent<UnitController>();
        CastleController castle = target.GetComponent<CastleController>();
        if (targetController != null)
        {
            AttackMessage message = new AttackMessage
            {
                action = "unitAttack",
                data = new UnitAttackData
                {
                    attackerId = attacker.gameObject.name, // or some unique ID for the unit
                    targetId = target.gameObject.name, // or some unique ID for the target
                    damage = attacker.attackDamage,
                    gameId = PlayerPrefs.GetString("gameId")
                },
                token = PlayerPrefs.GetString("accessToken"),
            };

            string jsonMessage = JsonUtility.ToJson(message);
            WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        }
        else if (castle != null)
        {
            CastleAttackMessage message = new CastleAttackMessage
            {
                action = "attackCastle",
                data = new CastleAttackData
                {
                    attackerId = attacker.gameObject.name, // or some unique ID for the unit
                    targetSide = attacker.isFacingRight ? SpawnSide.RightCastle.ToString() : SpawnSide.LeftCastle.ToString(), // or some unique ID for the target
                    damage = attacker.attackDamage,
                    gameId = PlayerPrefs.GetString("gameId")
                },
                token = PlayerPrefs.GetString("accessToken"),
            };

            string jsonMessage = JsonUtility.ToJson(message);
            WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        }
    }

    public void HandleProjectileSpawn(Turret turret, GameObject entityPrefab)
    {
        ProjectileSpawnMessage message = new ProjectileSpawnMessage
        {
            action = "spawnProjectile",
            data = new ProjectileSpawnData
            {
                turretId = turret.gameObject.name,
                targetId = turret.target.gameObject.name,
                gameId = PlayerPrefs.GetString("gameId"),
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        return;
    }

    public void HandleBuildingLevelUp(UnitSpawner unitSpawner, ResourceController resourceController, BuildingController building)
    {
        BuildingLevelUpMessage message = new BuildingLevelUpMessage
        {
            action = "levelUpBuilding",
            data = new BuildingLevelUpData
            {
                gameId = PlayerPrefs.GetString("gameId"),
                tag = building.gameObject.tag,
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        return;
    }

    public void HandleBuildingCreation(BuildingController building)
    {
        BuildingCreationMessage message = new BuildingCreationMessage
        {
            action = "createBuilding",
            data = new BuildingCreationData
            {
                gameId = PlayerPrefs.GetString("gameId"),
                tag = building.gameObject.tag,
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        return;
    }

    public void HandleAdvanceAge(SpawnSide side)
    {
        AdvanceAgeMessage message = new AdvanceAgeMessage
        {
            action = "advanceAge",
            data = new AdvanceAgeData
            {
                gameId = PlayerPrefs.GetString("gameId"),
                side = side.ToString(),
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
        return;
    }
}

[System.Serializable]
public class AttackMessage
{
    public string action;
    public UnitAttackData data;
    public string token;
}

[System.Serializable]
public class UnitAttackData
{
    public string attackerId;
    public string targetId;
    public int damage;

    public string gameId;
}

[System.Serializable]
public class CastleAttackMessage
{
    public string action;
    public CastleAttackData data;
    public string token;
}
[System.Serializable]
public class CastleAttackData
{
    public string gameId;
    public string attackerId;
    public string targetSide;
    public int damage;
}

[System.Serializable]
public class ProjectileSpawnMessage
{
    public string action;
    public ProjectileSpawnData data;
    public string token;
}

[System.Serializable]
public class ProjectileSpawnData
{
    public string turretId;
    public string targetId;
    public string gameId;
    public float damage;
}

[System.Serializable]
public class BuildingLevelUpMessage
{
    public string action;
    public BuildingLevelUpData data;
    public string token;
}

[System.Serializable]
public class BuildingLevelUpData
{
    public string gameId;
    public string tag;
}

[System.Serializable]
public class BuildingCreationMessage
{
    public string action;
    public BuildingCreationData data;
    public string token;
}

[System.Serializable]
public class BuildingCreationData
{
    public string tag;
    public string gameId;
}

[System.Serializable]
public class AdvanceAgeMessage
{
    public string action;
    public AdvanceAgeData data;
    public string token;
}
[System.Serializable]
public class AdvanceAgeData
{
    public string gameId;
    public string side;
}