using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GM : MonoBehaviour
{
    
    public int gold;
    public TextMeshProUGUI goldDisplay;

    public static GM instance;

    public TextMeshProUGUI finalScoreDisplay;
    public TextMeshProUGUI highscoreDisplay;

    public int totalScore;
    public TextMeshProUGUI scoreDisplay;
    public int scoreMultiplier;
    public TextMeshProUGUI scoreMultiplierDisplay;
    public float cooldown;
    public float timeLimit;

    private void Start()
    {
        // Assign public reference to this instance of the game manager
        instance = this;

        // Set UI text and score multiplier values
        scoreDisplay.text = "0";
        scoreMultiplierDisplay.text = "X1";
        scoreMultiplier = 1;

        // Initialize cooldown to be equal to initial timeLimit for increasing score multiplier
        cooldown = timeLimit;
    }

    private void Update()
    {
        // Update gold display text based on how much gold player currently has
        goldDisplay.text = "$ " + gold.ToString();

        // If the player has not gotten a kill within the cooldown, reset the multiplier
        // and the cooldown timer. Set score multiplier text on screen appropriately
        if (cooldown <= 0)
        {
            scoreMultiplier = 1;
            cooldown = timeLimit;
            scoreMultiplierDisplay.text = "X" + scoreMultiplier;
        } else
        {
            // Decrease cooldown until it reaches 0 seconds
            cooldown -= Time.deltaTime;
        }
    }
}
