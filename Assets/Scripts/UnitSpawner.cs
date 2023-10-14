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
            BoxCollider2D collider = clubman.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.5f);
            collider.isTrigger = true;

            Rigidbody2D rigidbody = clubman.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            clubman.transform.position = spawnLocation;

            clubman.gameObject.tag = spawnSide.ToString();
            if (spawnSide == SpawnSide.Left)
            {
                Flip(clubman);
                clubman.GetComponent<UnitController>().isFacingRight = true;
            }
            else
            {
                clubman.GetComponent<UnitController>().isFacingRight = false;

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

public enum SpawnSide
{
    Left,
    Right
}

