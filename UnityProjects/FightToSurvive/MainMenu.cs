using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // Transition to game level from main menu
    public void Play()
    {
        SceneManager.LoadScene("FightToSurvive");
    }

}
