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
        // Make this object vanish after specified amount of time
        Invoke("Vanish", lifetime);
    }

    // Spawn vanishing effect and destroy given object for nice visual removal of actor
    void Vanish()
    {
        if (vanishEffect != null)
        {
            Instantiate(vanishEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
