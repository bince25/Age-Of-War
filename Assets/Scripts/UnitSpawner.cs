using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnSide spawnSide;
    [SerializeField]
    private GameObject clubmanPrefab;

    [SerializeField]
    private GameObject leftSpawnPoint;
    [SerializeField]
    private GameObject rightSpawnPoint;

    private Vector3 spawnLocation;
    private Vector3 spawnCheckLocation;

    void Start()
    {
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
        SpawnEntity(clubmanPrefab);
    }

    public void SpawnEntity(GameObject entityPrefab)
    {
        if (CheckSpawnPoint())
        {
            GameObject clubman = Instantiate(entityPrefab);
            clubman.GetComponent<SPUM_Prefabs>().PlayAnimation("attack_normal");
            clubman.GetComponent<SPUM_Prefabs>().PlayAnimation("idle");
            clubman.GetComponent<SPUM_Prefabs>().PlayAnimation("run");
            clubman.AddComponent<BoxCollider2D>();
            clubman.transform.position = spawnLocation;
        }
    }
    public bool CheckSpawnPoint()
    {
        if (Physics2D.OverlapCircle(spawnCheckLocation, 1f) && Physics2D.OverlapCircle(spawnLocation, 1f))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

public enum SpawnSide
{
    Left,
    Right
}
