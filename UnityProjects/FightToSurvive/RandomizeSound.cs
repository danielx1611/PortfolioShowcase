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
        source = GetComponent<AudioSource>();
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        source.clip = clip;
        source.pitch = Random.Range(minPitch, maxPitch);
        if (playOnStart)
        {
            source.Play();
        }
    }

    public void PlaySound()
    {
        source.Play();
    }
}
