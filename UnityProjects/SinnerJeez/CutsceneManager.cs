using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject AdamDialogueContainer;
    [SerializeField] private GameObject NPCDialogueContainer;
    [SerializeField] private TextMeshProUGUI AdamDialogueText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    [SerializeField] private float initialDialogueWaitTime = 0f;
    [SerializeField] private float timeBetweenCutsceneEvents = 0f;

    [SerializeField] private string sceneToLoadOnFinish;

    [SerializeField] private string[] cutsceneInstructions;
    [SerializeField] private string[] cutsceneDialogues;

    [Space(10)]
    [Header("Priest Cutscene Specific")]
    [SerializeField] private SpriteRenderer[] priestRenderers;

    [Space(5)]
    [Header("Sounds")]
    [SerializeField] private GameObject dialogueBlipPlayer;
    [SerializeField] private GameObject dialogueBlipDarkPriest;
    [SerializeField] private GameObject transformationSound;
    [SerializeField] private GameObject menuInteractSound;

    private bool currentEventDone = false;
    private bool doneDialogueing = false;
    private bool shouldSkipDialogueAnimation = false;

    private int currentInstructionIndex;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        AdamDialogueContainer.SetActive(false);
        NPCDialogueContainer.SetActive(false);

        currentInstructionIndex = 0;

        StartCoroutine(BeginCutscene());

        gameManager = FindObjectOfType<GameManager>();

        dialogueBlipPlayer.gameObject.SetActive(false);
        dialogueBlipDarkPriest.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (doneDialogueing == false)
            {
                shouldSkipDialogueAnimation = true;
            } else
            {
                currentEventDone = true;
            }
        }
    }

    private IEnumerator DoLeftDialogue()
    {
        doneDialogueing = false;

        AdamDialogueContainer.SetActive(true);

        int currentCharacterIndex = 0;

        dialogueBlipPlayer.gameObject.SetActive(true);
        PlayRandomSoundAndPitch randomDialoguer = dialogueBlipPlayer.GetComponent<PlayRandomSoundAndPitch>();

        while (currentCharacterIndex < cutsceneDialogues[currentInstructionIndex].Length) 
        {
            if (shouldSkipDialogueAnimation)
            {
                AdamDialogueText.text = cutsceneDialogues[currentInstructionIndex];
                break;
            }

            AdamDialogueText.text = cutsceneDialogues[currentInstructionIndex].Substring(0, currentCharacterIndex + 1);

            randomDialoguer.PlayRandomAgain();

            yield return new WaitForSeconds(0.05f);
            currentCharacterIndex++;
        }

        doneDialogueing = true;
    }

    private IEnumerator DoRightDialogue()
    {
        doneDialogueing = false;

        NPCDialogueContainer.SetActive(true);

        int currentCharacterIndex = 0;

        dialogueBlipDarkPriest.gameObject.SetActive(true);

        while (currentCharacterIndex < cutsceneDialogues[currentInstructionIndex].Length)
        {
            if (shouldSkipDialogueAnimation)
            {
                NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex];
                break;
            }

            NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex].Substring(0, currentCharacterIndex + 1);
            yield return new WaitForSeconds(0.05f);
            currentCharacterIndex++;
        }

        doneDialogueing = true;
    }

    private IEnumerator DoPriestGoesEvil()
    {
        doneDialogueing = false;

        foreach (SpriteRenderer renderer in priestRenderers)
        {
            renderer.color = Color.black;
        }

        NPCDialogueContainer.SetActive(true);

        Instantiate(transformationSound);

        int currentCharacterIndex = 0;

        dialogueBlipDarkPriest.gameObject.SetActive(true);

        while (currentCharacterIndex < cutsceneDialogues[currentInstructionIndex].Length)
        {
            if (shouldSkipDialogueAnimation)
            {
                NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex];
                break;
            }

            NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex].Substring(0, currentCharacterIndex + 1);
            yield return new WaitForSeconds(0.05f);
            currentCharacterIndex++;
        }

        doneDialogueing = true;
    }

    private IEnumerator DoPriestGoesNormal()
    {
        doneDialogueing = false;

        foreach (SpriteRenderer renderer in priestRenderers)
        {
            renderer.color = Color.white;
        }

        NPCDialogueContainer.SetActive(true);

        Instantiate(transformationSound);

        int currentCharacterIndex = 0;

        dialogueBlipDarkPriest.gameObject.SetActive(true);

        while (currentCharacterIndex < cutsceneDialogues[currentInstructionIndex].Length)
        {
            if (shouldSkipDialogueAnimation)
            {
                NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex];
                break;
            }

            NPCDialogueText.text = cutsceneDialogues[currentInstructionIndex].Substring(0, currentCharacterIndex + 1);
            yield return new WaitForSeconds(0.05f);
            currentCharacterIndex++;
        }

        doneDialogueing = true;
    }

    private IEnumerator BeginCutscene()
    {
        yield return new WaitForSeconds(initialDialogueWaitTime);

        while (currentInstructionIndex < cutsceneInstructions.Length)
        {
            currentEventDone = false;

            switch(cutsceneInstructions[currentInstructionIndex])
            {
                case "dialogueLeft":
                    StartCoroutine(DoLeftDialogue());
                    break;

                case "dialogueRight":
                    StartCoroutine(DoRightDialogue());
                    break;

                case "priestGoesEvil":
                    StartCoroutine(DoPriestGoesEvil());
                    break;
                
                case "priestGoesNormal":
                    StartCoroutine(DoPriestGoesNormal());
                    break;

                case null:
                    break;
            }

            while (currentEventDone == false)
            {
                yield return new WaitForSeconds(0.1f);
            }

            currentInstructionIndex++;

            AdamDialogueContainer.SetActive(false);
            NPCDialogueContainer.SetActive(false);

            dialogueBlipPlayer.gameObject.SetActive(false);
            dialogueBlipDarkPriest.SetActive(false);

            shouldSkipDialogueAnimation = false;

            Instantiate(menuInteractSound);

            yield return new WaitForSeconds(timeBetweenCutsceneEvents);
        }

        gameManager.DoLoadLevelSequence();
    }
}
