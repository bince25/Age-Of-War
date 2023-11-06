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
    private ResourceController resourceController;
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

        if (gameManager != null)
        {
            resourceController = GameObject.FindGameObjectWithTag("LeftCastle").GetComponent<ResourceController>();
            unitSpawner = gameManager.GetComponent<UnitSpawner>();
        }
    }

    public void HandleGameMessage(string message)
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
                Debug.Log("CastleController attacked");
                GameManager.Instance.castlesDictionary[wsMessage.data["targetSide"].ToString()].TakeDamage((int)wsMessage.data["damage"]);
                break;
            case "unitDied":
                Debug.Log("Unit died");
                GameManager.Instance.unitsDictionary[wsMessage.data["unitId"].ToString()].Death();
                break;
            case "projectileSent":
                Debug.Log("Projectile sent");
                ProjectileSentMessage projectileSentMessage = wsMessage.data.ToObject<ProjectileSentMessage>();
                GameManager.Instance.turretsDictionary[projectileSentMessage.turretId].ShootById(projectileSentMessage.targetId);
                break;
            case "ageAdvanced":
                Debug.Log("Age advanced");
                AgeUpMessage ageUpMessage = wsMessage.data.ToObject<AgeUpMessage>();
                if (ageUpMessage.side == SpawnSide.Left.ToString())
                {
                    GameStateManager.Instance.leftAge = (Ages)ageUpMessage.age;

                    resourceController.ageText.text = GameStateManager.Instance.leftAge.ToString() + " Age";
                    resourceController.currentAge++;
                }
                else
                {
                    GameStateManager.Instance.rightAge = (Ages)ageUpMessage.age;
                }
                break;
            case "buildingUpgraded":
                Debug.Log("BuildingController upgraded");
                BuildingMessage buildingMessage_ = wsMessage.data.ToObject<BuildingMessage>();
                GameManager.Instance.buildingsDictionary[buildingMessage_.id].HandleLevelUp();
                break;
            case "buildingCreated":
                Debug.Log("BuildingController created");
                BuildingMessage buildingMessage = wsMessage.data.ToObject<BuildingMessage>();
                GameManager.Instance.buildingsDictionary[buildingMessage.id].HandleLevelUp();
                break;
            case "gameOver":
                Debug.Log("Game over");
                GameOverMessage gameOverMessage = wsMessage.data.ToObject<GameOverMessage>();

                if (gameOverMessage.targetSide == SpawnSide.LeftCastle.ToString())
                {
                    GameManager.Instance.castlesDictionary[SpawnSide.LeftCastle.ToString()].GetComponent<CastleController>().currentHealth = 0;
                    GameManager.Instance.castlesDictionary[SpawnSide.Left.ToString()].GetComponent<CastleController>().healthBar.SetHealth(0);
                }
                else
                {
                    GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].GetComponent<CastleController>().currentHealth = 0;
                    GameManager.Instance.castlesDictionary[SpawnSide.RightCastle.ToString()].GetComponent<CastleController>().healthBar.SetHealth(0);
                }
                break;
        }

    }

    public void HandleLobbyMessage(string message)
    {
        WSMessage wsMessage = JsonConvert.DeserializeObject<WSMessage>(message);
        Debug.Log("Lobby message: " + wsMessage.action);
        switch (wsMessage.action)
        {
            case "connectedToLobby":
                Debug.Log("Connected to Lobby");
                Debug.Log(wsMessage.data);
                ConnectedToLobbyMessage response = wsMessage.data.ToObject<ConnectedToLobbyMessage>();
                LobbyStateManager.Instance.lobby = response.lobby;
                LobbyStateManager.Instance.invitationCodeText.text = response.lobby.invitationCode;
                LoadingScreen.Instance.HideLoadingPanel();
                UIManager.Instance.ShowPanel(UIManager.Instance.lobbyPanel);
                break;
            case "selectedCountry":
                Debug.Log("Selected country");
                SelectedCountryMessage selectedCountryMessage = wsMessage.data.ToObject<SelectedCountryMessage>();
                Debug.Log("Country ID: " + selectedCountryMessage.countryId);
                CountryManager.Instance.HandleCountryUpdate((Country)selectedCountryMessage.countryId);
                break;
            case "toggledReady":
                Debug.Log("Toggled ready");
                ToggledReadyMessage toggledReadyMessage = wsMessage.data.ToObject<ToggledReadyMessage>();
                Debug.Log("Side: " + toggledReadyMessage.side);
                if (toggledReadyMessage.side == (int)SpawnSide.Left)
                {
                    LobbyStateManager.Instance.ready = toggledReadyMessage.ready;
                }
                else
                {
                    LobbyStateManager.Instance.opponentReady = toggledReadyMessage.ready;
                }
                break;
            case "toggledPublic":
                Debug.Log("Toggled public");
                TogglePrivateMessage togglePrivateMessage = wsMessage.data.ToObject<TogglePrivateMessage>();
                LobbyStateManager.Instance.HandleTogglePublic(togglePrivateMessage.isPrivate);
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

[System.Serializable]
public class UnitDiedMessage
{
    public string unitId;
}

[System.Serializable]
public class ProjectileSentMessage
{
    public string turretId;
    public string targetId;
    public string gameId;
}

[System.Serializable]
public class AgeUpMessage
{
    public string side;
    public int age;
}

[System.Serializable]
public class BuildingMessage
{
    public string id;
    public string type;
    public int level;
    public string side;
}


[System.Serializable]
public class GameOverMessage
{
    public string targetSide;
}

[System.Serializable]
public class ConnectedToLobbyMessage
{
    public Lobby lobby;
    public string side;
}

[System.Serializable]
public class SelectedCountryMessage
{
    public int countryId;
}

[System.Serializable]
public class ToggledReadyMessage
{
    public int side;
    public bool ready;
}

[System.Serializable]
public class TogglePrivateMessage
{
    public bool isPrivate;
}
