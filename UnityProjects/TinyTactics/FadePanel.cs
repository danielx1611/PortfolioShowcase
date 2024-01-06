using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanel : MonoBehaviour 
{

    public Animator fadePanelAnim;
    public string nextLevelName;

    public void StartSceneTransition()
    {
        StartCoroutine(LoadNextLevel());
    }

    public IEnumerator LoadNextLevel()
    {
        fadePanelAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1.25f);

        // Allow fade out transition to finish, then load next scene
        SceneManager.LoadScene(nextLevelName);
    }
}
