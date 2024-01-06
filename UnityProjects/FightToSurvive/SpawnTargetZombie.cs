using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTargetZombie : MonoBehaviour
{
    public Zombie targetZombie;
    public float timeBetweenSpawns;

    // Start is called before the first frame update
    void Start()
    {
        // Wait for given time then respawn given zombie asset
        StartCoroutine(SpawnDelay());
    }

    // Spawn given zombie asset after specified time between spawns
    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        Instantiate(targetZombie, transform.position, transform.rotation);
    }
}
