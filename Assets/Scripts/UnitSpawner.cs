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
            clubman.AddComponent<BoxCollider2D>();
            clubman.transform.position = spawnLocation;

            if (spawnSide == SpawnSide.Left)
            {
                Flip(clubman);
            }
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

    public void Flip(GameObject unit)
    {

        // Get the local scale
        Vector3 localScale = unit.transform.localScale;

        // Flip the x-component (horizontal mirroring)
        localScale.x *= -1;

        // Apply the adjusted local scale
        unit.transform.localScale = localScale;
    }

}

public enum SpawnSide
{
    Left,
    Right
}
