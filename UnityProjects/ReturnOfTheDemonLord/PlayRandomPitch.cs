using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomPitch : MonoBehaviour
{
    private AudioSource audioSource;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
