using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject clubmanPrefab;
    public void SpawnClubman()
    {
        GameObject clubman = Instantiate(clubmanPrefab);
        clubman.transform.position = new Vector3(2, -1, 0);
    }

}
