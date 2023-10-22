using System.Collections;
using UnityEngine;
using NativeWebSocket;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class WebSocketEvents : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManager;

    private UnitSpawner unitSpawner;
    public static WebSocketEvents Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        unitSpawner = gameManager.GetComponent<UnitSpawner>();
    }

    public void HandleMessage(string message)
    {
        WSMessage wsMessage = JsonConvert.DeserializeObject<WSMessage>(message);

        switch (wsMessage.action)
        {
            case "gameStarted":
                Debug.Log("Game started");
                GameStartedMessage gameStartedMessage = wsMessage.data.ToObject<GameStartedMessage>();
                PlayerPrefs.SetString("gameId", gameStartedMessage.gameId);
                Debug.Log("Game ID: " + gameStartedMessage.gameId);

                // Accessing the castles data
                CastleData leftCastle = gameStartedMessage.castles["LeftCastle"];
                CastleData rightCastle = gameStartedMessage.castles["RightCastle"];

                Debug.Log(leftCastle);

                GameManager.Instance.castlesDictionary[SpawnSide.LeftCastle.ToString()].id = leftCastle.id;
                GameManager.Instance.castlesDictionary[SpawnSide.LeftCastle.ToString()].currentHealth = leftCastle.health;
                GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].maxHealth = leftCastle.health;

                GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].id = rightCastle.id;
                GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].currentHealth = rightCastle.health;
                GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].maxHealth = rightCastle.health;
                break;
            case "unitSpawned":
                Debug.Log("Unit spawned");
                UnitSpawnedMessage unitSpawnedMessage = wsMessage.data.ToObject<UnitSpawnedMessage>();
                Debug.Log("Unit ID: " + unitSpawnedMessage.id);
                unitSpawner.HandleServerSpawnEntity(unitSpawnedMessage.type, unitSpawnedMessage.id, unitSpawnedMessage.side);
                break;
            case "unitAttacked":
                Debug.Log("Unit attacked");
                UnitAttackedMessage unitAttackedMessage = wsMessage.data.ToObject<UnitAttackedMessage>();
                Debug.Log("Attacker ID: " + unitAttackedMessage.attackerId);
                GameManager.Instance.unitsDictionary[unitAttackedMessage.targetId].AttackUnitByID(unitAttackedMessage.targetId, unitAttackedMessage.damage);
                break;
            case "castleAttacked":
                Debug.Log("Castle attacked");
                GameManager.Instance.castlesDictionary[wsMessage.data["targetSide"].ToString()].TakeDamage((int)wsMessage.data["damage"]);
                break;
            case "unitDied":
                Debug.Log("Unit died");
                GameManager.Instance.unitsDictionary[wsMessage.data["unitId"].ToString()].Death();
                break;
        }

    }
}

[System.Serializable]
public class WSMessage
{
    public string action;
    public JObject data;
    public bool success = true;
}

[System.Serializable]
public class GameStartedMessage
{
    public string[] players;
    public string gameId;
    public Dictionary<string, CastleData> castles;
}
[System.Serializable]
public class CastleData
{
    public string id;
    public int health;
    public string playerId;
    public string type;
}
[System.Serializable]
public class UnitSpawnedMessage
{
    public string id;
    public string type;
    public int health;
    public string side;
    public string playerId;
}
[System.Serializable]
public class UnitAttackedMessage
{
    public string attackerId;
    public int damage;
    public string targetId;
}
[System.Serializable]
public class CastleAttackedMessage
{
    public string side;
    public int damage;
}