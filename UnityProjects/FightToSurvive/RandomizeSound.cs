using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSound : MonoBehaviour
{

    public AudioClip[] clips;
    public float minPitch;
    public float maxPitch;

    AudioSource source;

    public bool playOnStart;

    // Start is called before the first frame update
    void Start()
    {
        // Assign audio source and pick random clip from available sounds
        source = GetComponent<AudioSource>();
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;

        // Set pitch to random value between minPitch and maxPitch
        source.pitch = Random.Range(minPitch, maxPitch);

        // If the sound should be played on start, then play the sound
        if (playOnStart)
        {
            source.Play();
        }
    }

    // Option to play the sound if not played on start
    public void PlaySound()
    {
        source.Play();
    }
}
