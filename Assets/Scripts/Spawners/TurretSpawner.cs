using UnityEngine;
using Mirror;
using System.Collections;

public class TurretSpawner : NetworkBehaviour
{
    [SerializeField]
    private SpawnSide spawnSide;
    private ResourceController resourceController;
    private UnitSpawner unitSpawner;

    [SerializeField]
    private GameObject turretPrefab;

    [SerializeField]
    private GameObject leftSpawnPoint;
    [SerializeField]
    private GameObject rightSpawnPoint;
    private Vector3 spawnLocation;

    void Start()
    {
        resourceController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceController>();
        unitSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UnitSpawner>();

        if (spawnSide == SpawnSide.Left)
        {
            spawnLocation = leftSpawnPoint.transform.position;
        }
        else
        {
            spawnLocation = rightSpawnPoint.transform.position;
        }
    }

    public void SpawnTurret()
    {
        if (isServer) // Ensure this is run on the server
        {
            if (resourceController.gold >= resourceController.turretCost)
            {
                resourceController.DecreaseGold(resourceController.turretCost);
                StartCoroutine(SpawnTurretCoroutine());
            }
        }
        else if (isClient && hasAuthority) // Client asks server to spawn
        {
            CmdSpawnTurret();
        }
    }

    [Command]
    void CmdSpawnTurret()
    {
        SpawnTurret(); // Call the original spawn method on the server
    }

    [Server]
    IEnumerator SpawnTurretCoroutine()
    {
        yield return new WaitForSeconds(5);
        GameObject turretSpawned = Instantiate(turretPrefab, spawnLocation, Quaternion.identity);
        NetworkServer.Spawn(turretSpawned);

        if (spawnSide == SpawnSide.Left)
        {
            turretSpawned.GetComponent<Turret>().isFacingRight = true;
        }
        else
        {
            turretSpawned.GetComponent<Turret>().isFacingRight = false;
            Flip(turretSpawned);
        }
    }

    void Flip(GameObject turret)
    {

        // Get the local scale
        Vector3 localScale = turret.transform.localScale;

        // Flip the x-component (horizontal mirroring)
        localScale.x *= -1;

        // Apply the adjusted local scale
        turret.transform.localScale = localScale;
    }
}
