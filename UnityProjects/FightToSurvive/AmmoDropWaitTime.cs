using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropWaitTime : MonoBehaviour
{

    public GameObject pickeable;

    public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn ammo pickeable after specified amount of time for a "respawning" effect
        Invoke("SpawnAmmoPickeable", waitTime);
    }

    // Spawn ammo pickeable item to replenish player ammo
    void SpawnAmmoPickeable()
    {
        Instantiate(pickeable, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
