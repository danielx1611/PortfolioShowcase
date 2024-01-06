using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator fadePanel;

    // Function to trigger scene transition sequence
    public void StartFadeIn(int sceneToLoad)
    {
        StartCoroutine(TriggerFadeIn(sceneToLoad));
    }

    // Set screen to fade to black, then wait for 1.5 seconds before switching scenes
    public IEnumerator TriggerFadeIn(int sceneToLoad)
    {
        fadePanel.SetTrigger("fadeIn");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
