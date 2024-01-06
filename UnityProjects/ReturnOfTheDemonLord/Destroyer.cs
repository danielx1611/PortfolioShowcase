using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float timeUntilDestroy;
    
    // Destroy object after specified amount of time
    void Start()
    {
        Destroy(gameObject, timeUntilDestroy);
    }
}
