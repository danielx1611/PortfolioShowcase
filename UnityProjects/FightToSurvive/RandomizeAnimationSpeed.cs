using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeAnimationSpeed : MonoBehaviour
{

    private Animator anim;

    public float minSpeed;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Get animator component and set the animation speed
        // to random value between minSpeed and maxSpeed
        anim = GetComponent<Animator>();
        anim.speed = Random.Range(minSpeed, maxSpeed);
    }
}
