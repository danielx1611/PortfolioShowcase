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
        StartCoroutine(SpawnDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        Instantiate(targetZombie, transform.position, transform.rotation);
    }
}
