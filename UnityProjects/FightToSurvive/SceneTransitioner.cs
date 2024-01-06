using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator fadePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFadeIn(int sceneToLoad)
    {
        StartCoroutine(TriggerFadeIn(sceneToLoad));
    }

    public IEnumerator TriggerFadeIn(int sceneToLoad)
    {
        fadePanel.SetTrigger("fadeIn");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
