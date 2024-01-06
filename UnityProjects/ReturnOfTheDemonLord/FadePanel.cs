using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanel : MonoBehaviour
{
    public Animator fadePanelAnim;
    public string nextLevelName;
    public TextMeshProUGUI[] storyTexts;
    private GameManager gameManager;
    public float textPlayTime;
    public Canvas speechBubble;
    public bool shouldPlayStoryTextboxes = true;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartSceneTransition()
    {
        StartCoroutine(LoadNextLevel());
    }

    public IEnumerator LoadNextLevel()
    {
        fadePanelAnim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1.25f);
        if (gameManager != null)
        {
            gameManager.playerHealth = 10;
        }
        if (shouldPlayStoryTextboxes)
        {
            StartCoroutine(PlayStoryTextboxes());
        } else
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public IEnumerator PlayStoryTextboxes()
    {
        foreach (TextMeshProUGUI text in storyTexts)
        {
            Animator textAnim = text.GetComponent<Animator>();
            textAnim.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(textPlayTime);
            textAnim.SetTrigger("fadeOut");
            yield return new WaitForSeconds(1.25f);
        }
        SceneManager.LoadScene(nextLevelName);
    }

    public void TriggerPlayerDialogue()
    {
        StartCoroutine(ShowPlayerDialogue());
    }

    public IEnumerator ShowPlayerDialogue()
    {
        if (speechBubble != null)
        {
            speechBubble.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            speechBubble.gameObject.SetActive(false);
        }
    }
}
