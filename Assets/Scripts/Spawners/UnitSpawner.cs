using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Mirror;

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField]
    public SpawnSide spawnSide;
    private ResourceController resourceController;

    [Space(10)]
    [SerializeField]
    private GameObject clubmanPrefab, slingerPrefab, stoneTankPrefab;

    public bool canSpawnMeleeUnit, canSpawnRangedUnit, canSpawnTankUnit, canSpawnMountedUnit, canSpawnSiegeUnit;

    [Space(10)]

    [Header("Spawn Times")]
    public int clubmanSpawnTime;
    public int slingerSpawnTime;
    public int stoneTankSpawnTime;

    [Space(10)]

    [SerializeField]
    private GameObject leftSpawnPoint;
    [SerializeField]
    private GameObject rightSpawnPoint;

    public Vector3 spawnLocation;
    private Vector3 spawnCheckLocation;

    public Slider progressBarSlider;
    public TextMeshProUGUI progressBarText;

    private bool isSpawning = false;
    private Queue<UnitSpawnData> unitQueue = new Queue<UnitSpawnData>();

    private int queueLimit = 5;

    public Image[] queueImages;

    public Sprite[] unitSpritesForQueue;

    void Start()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        canSpawnMeleeUnit = true;
        canSpawnRangedUnit = true;
        canSpawnTankUnit = false;
        canSpawnMountedUnit = false;
        canSpawnSiegeUnit = false;
        progressBarSlider.interactable = false;

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

    private void Update()
    {
        if (isServer) // Ensure the server handles the spawning logic
        {
            if (unitQueue.Count > 0 && !isSpawning)
            {
                SpawnNextUnitInQueue();
            }
        }
    }

    private class UnitSpawnData
    {
        public GameObject prefab;
        public float spawnTime;

        public UnitSpawnData(GameObject prefab, float spawnTime)
        {
            this.prefab = prefab;
            this.spawnTime = spawnTime;
        }
    }

    public void QueueUnitSpawn(GameObject unitPrefab, float spawnTime)
    {
        if (unitQueue.Count < queueLimit)
        {
            unitQueue.Enqueue(new UnitSpawnData(unitPrefab, spawnTime));
            UpdateQueueUI();
        }
    }

    private void UpdateQueueUI()
    {
        for (int i = 0; i < queueImages.Length; i++)
        {
            if (i < unitQueue.Count)
            {
                queueImages[i].gameObject.SetActive(true);
                queueImages[i].sprite = unitSpritesForQueue[i];
            }
            else
            {
                queueImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void SpawnNextUnitInQueue()
    {
        if (unitQueue.Count > 0)
        {
            UnitSpawnData nextUnitData = unitQueue.Dequeue();
            UpdateQueueUI();
            StartCoroutine(SpawnEntity(nextUnitData.prefab, nextUnitData.spawnTime));
        }
    }

    public void SpawnClubman()
    {
        if (resourceController.gold >= resourceController.clubmanCost && canSpawnMeleeUnit)
        {
            clubmanPrefab.GetComponent<UnitController>().spawnSide = spawnSide;
            QueueUnitSpawn(clubmanPrefab, clubmanSpawnTime);
            resourceController.DecreaseGold(resourceController.clubmanCost);
        }
    }


    public void SpawnSlinger()
    {
        if (resourceController.gold >= resourceController.slingerCost && canSpawnRangedUnit)
        {
            slingerPrefab.GetComponent<UnitController>().spawnSide = spawnSide;
            QueueUnitSpawn(slingerPrefab, slingerSpawnTime);
            resourceController.DecreaseGold(resourceController.slingerCost);
        }
    }

    public void SpawnStoneTank()
    {
        if (resourceController.gold >= resourceController.stoneTankCost && canSpawnTankUnit)
        {
            clubmanPrefab.GetComponent<UnitController>().spawnSide = spawnSide;
            QueueUnitSpawn(stoneTankPrefab, stoneTankSpawnTime);
            resourceController.DecreaseGold(resourceController.stoneTankCost);
        }
    }

    public void HandleServerSpawnEntity(string unitType, string id, string unitSpawnSide)
    {
        GameObject prefabToSpawn = null;

        switch (unitType)
        {
            case "Clubman":
                prefabToSpawn = clubmanPrefab;
                break;
            case "Slinger":
                prefabToSpawn = slingerPrefab;
                break;
            case "StoneTank":
                prefabToSpawn = stoneTankPrefab;
                break;
            default:
                Debug.LogError($"Unknown unit type: {unitType}");
                return;
        }

        Vector3 unitSpawnLocation = unitSpawnSide == SpawnSide.Left.ToString() ? leftSpawnPoint.transform.position : rightSpawnPoint.transform.position;

        GameObject unitSpawned = Instantiate(prefabToSpawn, unitSpawnLocation, Quaternion.identity);
        unitSpawned.gameObject.name = id;
        GameManager.Instance.unitsDictionary.Add(id, unitSpawned.GetComponent<UnitController>());

        BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.5f);
        collider.isTrigger = true;

        Rigidbody2D rigidbody = unitSpawned.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

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

    public IEnumerator SpawnEntity(GameObject entityPrefab, float spawnTime)
    {
        yield return StartCoroutine(SpawnProgress(spawnTime, entityPrefab));
    }

    public IEnumerator SpawnProgress(float spawnTime, GameObject entityPrefab)
    {
        float timer = 0f;

        bool spawned = false;

        while (timer < spawnTime)
        {
            float progress = timer / spawnTime;
            progressBarSlider.value = progress;
            progressBarText.text = Mathf.RoundToInt(progress * 100) + "%";
            timer += Time.deltaTime;

            if (spawnTime - timer < 0.1f && !spawned)
            {
                GameManager.Instance.CurrentStrategy.HandleUnitSpawn(this, entityPrefab);
                spawned = true;
            }

            yield return null;
        }

        progressBarSlider.value = 1f;

        isSpawning = false;
    }

    public GameObject InstantiateUnit(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        return Instantiate(gameObject, position, rotation);
    }

    public bool CheckSpawnPoint(SpawnSide spawnSide)
    {
        Vector3 spawnCheckLocation = spawnSide == SpawnSide.Left ? spawnLocation + Vector3.right : spawnLocation + Vector3.left;

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
        Vector3 localScale = unit.transform.localScale;
        Transform firstChild = unit.transform.GetChild(0).GetChild(0);
        Vector3 firstChildLocalScale = firstChild.localScale;

        localScale.x *= -1;
        firstChildLocalScale.x *= -1;

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
