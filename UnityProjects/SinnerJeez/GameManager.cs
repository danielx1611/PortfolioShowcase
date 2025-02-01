using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EntitySpawnManager entitySpawnManager;

    [SerializeField] private Button playButton;

    private PlayerController playerController = null;
    private FadePanel fp;

    bool hasSubscribed = false;

    private void Awake()
    {
        GameObject[] gms = GameObject.FindGameObjectsWithTag("gm");

        if (gms.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        entitySpawnManager = FindObjectOfType<EntitySpawnManager>();
        fp = FindObjectOfType<FadePanel>();
        playButton = FindObjectOfType<Button>();
        if (playButton)
        {
            TextMeshProUGUI text = playButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text.text == "Play")
            {
                playButton.onClick.AddListener(OnPlayButton);
            }
        }

        if (entitySpawnManager)
        {
            playerController = entitySpawnManager.SpawnPlayer();
            entitySpawnManager.SpawnEnemies();
            FindCinemachineCamera();
        }

        if (hasSubscribed == false)
        {
            SceneManager.sceneLoaded += SpawnEnemies;
            SceneManager.sceneLoaded += SpawnPlayer;
            SceneManager.sceneLoaded += FindCinemachineCameraSubscription;
            SceneManager.sceneLoaded += FindFadePanel;

            hasSubscribed = true;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnPlayButton()
    {
        DoLoadLevelSequence();
    }

    public void DoLoadLevelSequence(string sceneName = "")
    {
        fp = FindObjectOfType<FadePanel>();
        fp.StartSceneTransition(sceneName);
    }

    private void SpawnEnemies(Scene scene, LoadSceneMode mode)
    {
        entitySpawnManager = FindObjectOfType<EntitySpawnManager>();

        if (entitySpawnManager)
        {
            entitySpawnManager.SpawnEnemies();
        }
    }

    private void SpawnPlayer(Scene scene, LoadSceneMode mode)
    {
        entitySpawnManager = FindObjectOfType<EntitySpawnManager>();

        if (entitySpawnManager)
        {
            playerController = entitySpawnManager.SpawnPlayer();
        }
    }

    private void FindCinemachineCameraSubscription(Scene scene, LoadSceneMode mode)
    {
        FindCinemachineCamera();
    }

    private void FindCinemachineCamera()
    {
        CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();

        if (cam && playerController)
        {
            cam.Follow = playerController.CameraOffsetTransform;
        }
    }

    private void FindFadePanel(Scene scene, LoadSceneMode mode)
    {
        fp = FindObjectOfType<FadePanel>();
    }
}
