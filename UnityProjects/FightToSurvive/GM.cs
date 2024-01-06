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
        instance = this;

        scoreDisplay.text = "0";
        scoreMultiplierDisplay.text = "X1";
        scoreMultiplier = 1;

        cooldown = timeLimit;
    }

    private void Update()
    {
        goldDisplay.text = "$ " + gold.ToString();

        if (cooldown <= 0)
        {
            scoreMultiplier = 1;
            cooldown = timeLimit;
            scoreMultiplierDisplay.text = "X" + scoreMultiplier;
        } else
        {
            cooldown -= Time.deltaTime;
        }
    }
}
