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
            SpawnEntity(clubmanPrefab);
            resourceController.DecreaseGold(resourceController.clubmanCost);
        }
    }

    public void SpawnSlinger()
    {
        if (resourceController.gold >= resourceController.slingerCost && canSpawnRangedUnit)
        {
            SpawnEntity(slingerPrefab);
            resourceController.DecreaseGold(resourceController.slingerCost);
        }
    }

    public void SpawnStoneTank()
    {
        if (resourceController.gold >= resourceController.stoneTankCost && canSpawnTankUnit)
        {
            SpawnEntity(stoneTankPrefab);
            resourceController.DecreaseGold(resourceController.stoneTankCost);
        }
    }

    public void SpawnEntity(GameObject entityPrefab)
    {
        if (true)
        {
            GameObject unitSpawned = Instantiate(entityPrefab);
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
