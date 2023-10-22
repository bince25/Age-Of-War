using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Spawn Times")]
    public int clubmanSpawnTime;
    public int slingerSpawnTime;
    public int stoneTankSpawnTime;

    [Space(10)]

    [SerializeField]
    private GameObject leftSpawnPoint;
    [SerializeField]
    private GameObject rightSpawnPoint;

    private Vector3 spawnLocation;
    private Vector3 spawnCheckLocation;

    public Slider progressBarSlider;
    public TextMeshProUGUI progressBarText;

    //--------------------------------Queue & Unit Spawn------------------------------------------
    private bool isSpawning = false;
    private Queue<UnitSpawnData> unitQueue = new Queue<UnitSpawnData>();

    private int queueLimit = 5;

    public Image[] queueImages;

    public Sprite[] unitSpritesForQueue;


    //------------------------------------------------------------------------------------------

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
        if (unitQueue.Count > 0)
        {
            if (!isSpawning)
            {
                SpawnNextUnitInQueue();
                isSpawning = true;
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
        // Clear the current queue display.
        for (int i = 0; i < queueImages.Length; i++)
        {
            if (i < unitQueue.Count)
            {
                // Customize the queue image or sprite with the unit's appearance.
                // For example: queueImages[i].sprite = unitQueue.ElementAt(i).prefab.GetComponent<SpriteRenderer>().sprite;
                queueImages[i].gameObject.SetActive(true); // Show the image.
                queueImages[i].sprite = unitSpritesForQueue[i];
            }
            else
            {
                queueImages[i].gameObject.SetActive(false); // Hide unused slots.
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
            QueueUnitSpawn(clubmanPrefab, clubmanSpawnTime);
            resourceController.DecreaseGold(resourceController.clubmanCost);
        }
    }

    public void SpawnSlinger()
    {
        if (resourceController.gold >= resourceController.slingerCost && canSpawnRangedUnit)
        {
            QueueUnitSpawn(slingerPrefab, slingerSpawnTime);
            resourceController.DecreaseGold(resourceController.slingerCost);
        }
    }

    public void SpawnStoneTank()
    {
        if (resourceController.gold >= resourceController.stoneTankCost && canSpawnTankUnit)
        {
            QueueUnitSpawn(stoneTankPrefab, stoneTankSpawnTime);
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

    public IEnumerator SpawnEntity(GameObject entityPrefab, float spawnTime)
    {
        yield return StartCoroutine(SpawnProgress(spawnTime));

        GameObject unitSpawned = Instantiate(entityPrefab);
        progressBarSlider.value = 0f;
        progressBarText.text = "0%";
        BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.5f);
        collider.isTrigger = true;
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

        public IEnumerator SpawnProgress(float spawnTime)
        {

            float timer = 0f;
            while (timer < spawnTime)
            {
                float progress = timer / spawnTime;

                if (progressBarSlider != null)
                {
                    progressBarSlider.value = progress;
                    progressBarText.text = Mathf.RoundToInt(progress * 100) + "%";
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // Set the progress bar to its final value (1) after the loop ends.
            progressBarSlider.value = 1f;
            isSpawning = false;
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

