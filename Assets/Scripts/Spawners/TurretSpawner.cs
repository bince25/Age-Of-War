using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
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
        if (resourceController.gold >= resourceController.turretCost)
        {
            resourceController.DecreaseGold(resourceController.turretCost);
            StartCoroutine(SpawnTurretCoroutine());
        }
    }

    IEnumerator SpawnTurretCoroutine()
    {
        yield return new WaitForSeconds(5);
        GameObject turretSpawned = Instantiate(turretPrefab, spawnLocation, Quaternion.identity);
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
