using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    FadePanel fadePanel;
    DemonLord player;

    // Start is called before the first frame update
    void Start()
    {
        // Assign references
        player = FindObjectOfType<DemonLord>();
        fadePanel = FindObjectOfType<FadePanel>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collision.isTrigger)
        {
            // When player boss rigidbody enters the finish game zone, transition to the final sequence and return to main menu
            player.gameOver = true;
            fadePanel.nextLevelName = "MainMenu";
            fadePanel.StartSceneTransition();
        }
    }
}
