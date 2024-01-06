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
        Invoke("SpawnAmmoPickeable", waitTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnAmmoPickeable()
    {
        Instantiate(pickeable, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
