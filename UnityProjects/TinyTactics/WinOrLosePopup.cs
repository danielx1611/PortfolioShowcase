using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinOrLosePopup : MonoBehaviour
{
    GameManager gm;
    TMP_Text winLoseText;

    private void Start()
    {
        // Assign references
        gm = FindObjectOfType<GameManager>();
        winLoseText = GetComponent<TMP_Text>();

        // If the game was won, present the win screen.
        // If the game was lost, present the lose screen.
        if (gm.gameWon)
        {
            winLoseText.text = "you won!";
            winLoseText.color = Color.green;
        } else
        {
            winLoseText.text = "you lost!";
            winLoseText.color = Color.red;
        }
    }
}
