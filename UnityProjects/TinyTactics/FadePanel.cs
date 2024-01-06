using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanel : MonoBehaviour 
{

    public Animator fadePanelAnim;
    public string nextLevelName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSceneTransition()
    {
        StartCoroutine(LoadNextLevel());
    }

    public IEnumerator LoadNextLevel()
    {
        fadePanelAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(nextLevelName);
    }
}
