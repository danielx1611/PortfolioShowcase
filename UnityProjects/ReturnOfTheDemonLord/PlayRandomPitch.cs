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
        // Get audio source, and pick a random pitch between min and max values, then play the audio clip
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}
