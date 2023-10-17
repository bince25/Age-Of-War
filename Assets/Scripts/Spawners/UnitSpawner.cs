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

    public IEnumerator SpawnEntity(GameObject entityPrefab, float spawnTime)
    {
        yield return StartCoroutine(SpawnProgress(spawnTime));

        GameObject unitSpawned = Instantiate(entityPrefab);
        progressBarSlider.value = 0f;
        progressBarText.text = "0%";
        BoxCollider2D collider = unitSpawned.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.5f);
        collider.isTrigger = true;

        Rigidbody2D rigidbody = unitSpawned.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
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
