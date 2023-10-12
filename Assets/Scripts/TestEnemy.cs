using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(5f);
        this.gameObject.GetComponent<UnitSpawner>().SpawnClubman();
    }
}
