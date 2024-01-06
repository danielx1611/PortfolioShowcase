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
        player = FindObjectOfType<DemonLord>();
        fadePanel = FindObjectOfType<FadePanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collision.isTrigger)
        {
            player.gameOver = true;
            fadePanel.nextLevelName = "MainMenu";
            fadePanel.StartSceneTransition();
        }
    }
}
