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
        gm = FindObjectOfType<GameManager>();
        winLoseText = GetComponent<TMP_Text>();

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
