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
        // Assign references
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
            // Reset player health
            gameManager.playerHealth = 10;
        }
        if (shouldPlayStoryTextboxes)
        {
            // Play story textboxes before transition
            StartCoroutine(PlayStoryTextboxes());
        } else
        {
            // No story textboxes, load next screen
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public IEnumerator PlayStoryTextboxes()
    {
        foreach (TextMeshProUGUI text in storyTexts)
        {
            // Get reference to text animator then set the text to be visible
            Animator textAnim = text.GetComponent<Animator>();
            textAnim.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            // The above sequence enables the text animator, which automatically fades the text in over 0.5 seconds, so wait for fade in

            // Let player have time to read text then fade out to next text sequence
            yield return new WaitForSeconds(textPlayTime);
            textAnim.SetTrigger("fadeOut");
            yield return new WaitForSeconds(1.25f);
        }

        // Story texts done, load next scene
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
            // Quickly show player's comment on this level, then hide it again
            speechBubble.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            speechBubble.gameObject.SetActive(false);
        }
    }
}
