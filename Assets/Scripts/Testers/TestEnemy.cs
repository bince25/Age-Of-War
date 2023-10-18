using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    void Start()
    {
        // Continuously spawn units every 5 seconds
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            this.gameObject.GetComponent<UnitSpawner>().SpawnClubman();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
