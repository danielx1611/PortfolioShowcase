using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        // Assign particle system and play the given particle effect
        ps = GetComponent<ParticleSystem>();
        ps.Play();
    }
}
