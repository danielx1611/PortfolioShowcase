using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using cowsins;

public class WaveManager : MonoBehaviour
{
    // Create list to make set of waves in inspector
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public GameObject[] enemies;
        public float timeBetweenSpawns;
    }

    // Create list to keep track of the waves
    public Wave[] waves;

    // Create list to keep track of zombie spawn points
    public Transform[] spawnPoints;

    // Data to keep track of current wave and wave transitions
    public float timeBetweenWaves;
    Wave currentWave;
    [HideInInspector] public int currentWaveIndex;

    // Elements for wave transition and win state transition
    public TMP_Text waveText;
    public GameObject waveStartSound;
    public GameObject winScreen;

    // Booleans to manage game state
    bool gameEnded;
    bool currentWaveEnded;
    public bool playerDead;

    // Int to keep track of how many zombies are left in current wave
    public int aliveZombiesCount;

    // Canvas UI elements to aide player in starting/preparing for next wave
    public TextMeshProUGUI intermissionDisplay;
    public TextMeshProUGUI pickUpGunText;

    // Reference to scene transitioner game object
    public SceneTransitioner sceneTransitioner;

    // Update is called once per frame
    void Update()
    {
        // If the current wave ends and the game is over, make sure all of the zombies are dead.
        if (currentWaveEnded)
        {
            if (gameEnded == true)
            {

                if (aliveZombiesCount == 0)
                {
                    // If all of the zombies are dead, enable the winscreen UI
                    winScreen.SetActive(true);
                    GM.instance.finalScoreDisplay.text = GM.instance.totalScore.ToString();

                    // If player reaches new high score, save it to PlayerPrefs.
                    if (GM.instance.totalScore > PlayerPrefs.GetInt("highscore", 0))
                    {
                        PlayerPrefs.SetInt("highscore", GM.instance.totalScore);
                    }

                    // Update highscore text accordingly
                    GM.instance.highscoreDisplay.text = PlayerPrefs.GetInt("highscore", 0).ToString();

                    // If player presses R, reload the scene to start from beginning
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        sceneTransitioner.TriggerFadeIn(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
            if (aliveZombiesCount == 0 && currentWaveIndex != waves.Length)
            {
                // Start intermission before transitioning to next wave
                StartCoroutine(Intermission());
            }
        } 
    }

    IEnumerator SpawnWaves()
    {
        // Transition to next wave with visual and audio cues
        Instantiate(waveStartSound);
        waveText.gameObject.SetActive(true);
        waveText.text = "Wave " + (currentWaveIndex + 1).ToString();
        Invoke("DeactivateText", 4.5f);

        // Update current wave and number of zombies currently alive
        currentWave = waves[currentWaveIndex];
        aliveZombiesCount = currentWave.numberOfEnemies;

        // Keep spawning random enemy zombies until reaching number of enemies on current wave or player is dead
        for (int a = 0; a < currentWave.numberOfEnemies; a++)
        {
            if (playerDead)
            {
                // Stop spawning zombies
                yield break;
            }
            GameObject randomEnemy = currentWave.enemies[Random.Range(0, currentWave.enemies.Length)];
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(randomEnemy, randomSpawnPoint.position, randomSpawnPoint.rotation);

            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }

        // Move to next wave
        currentWaveIndex++;

        // Check if all waves have been passed. If so, mark game as finished
        if (currentWaveIndex == waves.Length)
        {
            gameEnded = true;
        }
        // Indicate the current wave is over
        currentWaveEnded = true;
    }

    IEnumerator Intermission()
    {
        // Reset currentWaveEnded status and indicate to player when next wave is arriving
        currentWaveEnded = false;
        intermissionDisplay.gameObject.SetActive(true);
        for (int i = 0; i < timeBetweenWaves; i++)
        {
            // Make countdown until next wave
            intermissionDisplay.text = "ZOMBIES INCOMING IN " + (timeBetweenWaves - i).ToString();
            yield return new WaitForSeconds(1f);
        }

        // Hide intermission text, then start the next wave
        intermissionDisplay.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnWaves());
    }

    void DeactivateText()
    {
        waveText.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartNewWave()
    {
        // Hide pickUpGunText UI Element, then start a new wave
        pickUpGunText.gameObject.SetActive(false);
        intermissionDisplay.text = "ZOMBIES INCOMING IN " + timeBetweenWaves.ToString();
        StartCoroutine(Intermission());
    }
}

