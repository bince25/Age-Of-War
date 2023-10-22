using System.Collections;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    private bool startToSpawn;
    private bool spawned = false;
    void Start()
    {
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
        if (startToSpawn && !spawned)
        {
            spawned = true;
            StartCoroutine(Spawn());
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            startToSpawn = true;
        }
    }

}
