using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSoundAndPitch : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource audioSource;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        // Get a random clip, then randomize the pitch of the clip and play it from the audio source
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
