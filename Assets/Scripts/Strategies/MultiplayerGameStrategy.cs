
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
        Castle castle = target.GetComponent<Castle>();
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
}