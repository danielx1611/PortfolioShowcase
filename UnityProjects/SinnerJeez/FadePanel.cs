using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanel : MonoBehaviour
{

    public Animator fadePanelAnim;
    public string nextLevelName;

    public void StartSceneTransition(string sceneName = "")
    {
        StartCoroutine(LoadNextLevel(sceneName));
    }

    public IEnumerator LoadNextLevel(string sceneName = "")
    {
        fadePanelAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1.25f);
        
        if (sceneName == "")
        {
            SceneManager.LoadScene(nextLevelName);
        } else
        {
            SceneManager.LoadScene(sceneName);
        }

    }
}

