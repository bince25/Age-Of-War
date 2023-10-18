using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnSide spawnSide;
    private ResourceController resourceController;

    [Space(10)]
    [SerializeField]
    private GameObject clubmanPrefab, slingerPrefab, stoneTankPrefab;

    public bool canSpawnMeleeUnit, canSpawnRangedUnit, canSpawnTankUnit, canSpawnMountedUnit, canSpawnSiegeUnit;

    [Space(10)]

    [SerializeField]
    private GameObject leftSpawnPoint;
    [SerializeField]
    private GameObject rightSpawnPoint;

    private Vector3 spawnLocation;
    private Vector3 spawnCheckLocation;

    void Start()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        canSpawnMeleeUnit = true;
        canSpawnRangedUnit = true;
        canSpawnTankUnit = false;
        canSpawnMountedUnit = false;
        canSpawnSiegeUnit = false;
        if (spawnSide == SpawnSide.Left)
        {
            spawnLocation = leftSpawnPoint.transform.position;
            spawnCheckLocation = spawnLocation + Vector3.right;
        }
        else
        {
            spawnLocation = rightSpawnPoint.transform.position;
            spawnCheckLocation = spawnLocation + Vector3.left;
        }
    }

    public void SpawnClubman()
    {
        if (resourceController.gold >= resourceController.clubmanCost && canSpawnMeleeUnit)
        {
            SendSpawnEntity(clubmanPrefab);
            resourceController.DecreaseGold(resourceController.clubmanCost);
        }
    }

    public void SpawnSlinger()
    {
        if (resourceController.gold >= resourceController.slingerCost && canSpawnRangedUnit)
        {
            SendSpawnEntity(slingerPrefab);
            resourceController.DecreaseGold(resourceController.slingerCost);
        }
    }

    public void SpawnStoneTank()
    {
        if (resourceController.gold >= resourceController.stoneTankCost && canSpawnTankUnit)
        {
            SendSpawnEntity(stoneTankPrefab);
            resourceController.DecreaseGold(resourceController.stoneTankCost);
        }
    }

    // To send a spawn unit message to the server, use this method:
    public void SendSpawnEntity(GameObject entityPrefab)
    {
        string id = System.Guid.NewGuid().ToString();
        SpawnMessage message = new SpawnMessage
        {
            action = "spawnUnit",
            data = new SpawnData
            {
                id = id,
                type = entityPrefab.name,
                side = spawnSide.ToString(),
                health = entityPrefab.GetComponent<UnitController>().maxHealth,
                gameId = PlayerPrefs.GetString("gameId")
            },
            token = PlayerPrefs.GetString("accessToken"),
        };

        string jsonMessage = JsonUtility.ToJson(message);
        WebSocketClient.Instance.SendWebSocketMessage(jsonMessage);
    }

    // Handle the server's spawn unit message using this method:
    public void HandleServerSpawnEntity(string unitType, string id, string unitSpawnSide)
    {
        Debug.Log("Unit type: " + unitType);
        switch (unitType)
        {
            case "Clubman":
                SpawnEntity(clubmanPrefab, id, unitSpawnSide);
                break;
            case "Slinger":
                SpawnEntity(slingerPrefab, id, unitSpawnSide);
                break;
            case "StoneTank":
                SpawnEntity(stoneTankPrefab, id, unitSpawnSide);
                break;
        }
    }

    public void SpawnEntity(GameObject entityPrefab)
    {
        if (true)
        {
            GameObject unitSpawned = Instantiate(entityPrefab);

            string unitID = System.Guid.NewGuid().ToString();
            unitSpawned.gameObject.name = unitID;
            GameManager.Instance.unitsDictionary.Add(unitID, unitSpawned.GetComponent<UnitController>());

            BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.5f);
            collider.isTrigger = true;

            Rigidbody2D rigidbody = unitSpawned.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Vector3 unitSpawnLocation =
            unitSpawned.transform.position = spawnLocation;

            unitSpawned.gameObject.tag = spawnSide.ToString();
            if (spawnSide == SpawnSide.Left)
            {
                Flip(unitSpawned);
                unitSpawned.GetComponent<UnitController>().isFacingRight = true;
            }
            else
            {
                unitSpawned.GetComponent<UnitController>().isFacingRight = false;

            }
        }
    }

    public void SpawnEntity(GameObject entityPrefab, string id, string unitSpawnSide)
    {
        if (true)
        {
            GameObject unitSpawned = Instantiate(entityPrefab);

            unitSpawned.gameObject.name = id;

            GameManager.Instance.unitsDictionary.Add(id, unitSpawned.GetComponent<UnitController>());

            BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.5f);
            collider.isTrigger = true;

            Rigidbody2D rigidbody = unitSpawned.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            Vector3 unitSpawnLocation = unitSpawnSide == SpawnSide.Left.ToString() ? leftSpawnPoint.transform.position : rightSpawnPoint.transform.position;
            unitSpawned.transform.position = unitSpawnLocation;

            unitSpawned.gameObject.tag = unitSpawnSide;
            if (unitSpawnSide == SpawnSide.Left.ToString())
            {
                Flip(unitSpawned);
                unitSpawned.GetComponent<UnitController>().isFacingRight = true;
            }
            else
            {
                unitSpawned.GetComponent<UnitController>().isFacingRight = false;

            }
        }
    }

    public bool CheckSpawnPoint()
    {
        Collider2D collider1 = Physics2D.OverlapCircle(spawnCheckLocation, 1f);
        Collider2D collider2 = Physics2D.OverlapCircle(spawnLocation, 1f);
        if (collider1 && collider2)
        {
            if (collider1.gameObject.tag != "LeftCastle" || collider2.gameObject.tag == "RightCastle")
            {
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Flip(GameObject unit)
    {

        // Get the local scale
        Vector3 localScale = unit.transform.localScale;

        Transform firstChild = unit.transform.GetChild(0).GetChild(0);
        Vector3 firstChildLocalScale = firstChild.localScale;


        // Flip the x-component (horizontal mirroring)
        localScale.x *= -1;
        firstChildLocalScale.x *= -1;
        // Apply the adjusted local scale
        unit.transform.localScale = localScale;
        firstChild.localScale = firstChildLocalScale;
    }

}


[System.Serializable]
public class SpawnMessage
{
    public string action;
    public SpawnData data;
    public string token;
}

[System.Serializable]
public class SpawnData
{
    public string id;
    public string type;
    public string side;
    public int health;

    public string gameId;
}

