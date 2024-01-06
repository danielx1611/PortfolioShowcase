using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossInteraction : MonoBehaviour
{
    public TextMeshProUGUI interactText;
    public GameObject deathEffect;
    public PersistMusic bgMusic;

    private HeroKnight player;
    private bool canInteract = false;

    private Vector3 offset = new Vector3(0, 0.46f, 0);

    // Start is called before the first frame update
    void Start()
    {
        // Assign references
        player = FindObjectOfType<HeroKnight>();
        bgMusic = FindObjectOfType<PersistMusic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // If player interacts with boss chest, play death sequence and stop music
                Instantiate(deathEffect, transform.position + offset, transform.rotation);
                GetComponent<AudioSource>().Play();
                player.CustomDeath();
                bgMusic.StopMusic();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Make interact text visible to player when in distance of chest
            interactText.gameObject.SetActive(true);
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Make interact text invisible to player when no longer in distance of chest
            interactText.gameObject.SetActive(false);
            canInteract = false;
        }
    }
}
