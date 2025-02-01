using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSoundAndPitch : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource audioSource;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public bool shouldLoop = false;

    private AudioClip currentClip;

    // Start is called before the first frame update
    void Start()
    {
        currentClip = clips[Random.Range(0, clips.Length)];
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = currentClip;
        audioSource.loop = shouldLoop;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayRandomAgain()
    {
        if (currentClip != null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.Stop();
            currentClip = null;
        }

        currentClip = clips[Random.Range(0, clips.Length)];
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = currentClip;
        audioSource.loop = shouldLoop;
        audioSource.Play();
    }
}
