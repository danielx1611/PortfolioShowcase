using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
