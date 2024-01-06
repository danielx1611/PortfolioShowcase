using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{

    public float lifetime;

    public GameObject vanishEffect;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Vanish", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Vanish()
    {
        if (vanishEffect != null)
        {
            Instantiate(vanishEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
