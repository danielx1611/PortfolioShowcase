using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using cowsins;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public GameObject[] enemies;
        public float timeBetweenSpawns;
    }

    public Wave[] waves;

    public Transform[] spawnPoints;

    public float timeBetweenWaves;

    Wave currentWave;
    [HideInInspector] public int currentWaveIndex;

    public TMP_Text waveText;
    public GameObject waveStartSound;

    public GameObject winScreen;
    
    bool gameEnded;
    bool currentWaveEnded;

    public bool playerDead;

    public int aliveZombiesCount;

    public TextMeshProUGUI intermissionDisplay;
    public TextMeshProUGUI pickUpGunText;

    public SceneTransitioner sceneTransitioner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveEnded)
        {
            if (gameEnded == true)
            {

                if (aliveZombiesCount == 0)
                {
                    winScreen.SetActive(true);
                    GM.instance.finalScoreDisplay.text = GM.instance.totalScore.ToString();

                    if (GM.instance.totalScore > PlayerPrefs.GetInt("highscore", 0))
                    {
                        PlayerPrefs.SetInt("highscore", GM.instance.totalScore);
                    }

                    GM.instance.highscoreDisplay.text = PlayerPrefs.GetInt("highscore", 0).ToString();

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        sceneTransitioner.TriggerFadeIn(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
            if (aliveZombiesCount == 0 && currentWaveIndex != waves.Length)
            {
                StartCoroutine(Intermission());
            }
        } 
    }

    IEnumerator SpawnWaves()
    {
        Instantiate(waveStartSound);
        waveText.gameObject.SetActive(true);
        waveText.text = "Wave " + (currentWaveIndex + 1).ToString();
        Invoke("DeactivateText", 4.5f);
        currentWave = waves[currentWaveIndex];
        aliveZombiesCount = currentWave.numberOfEnemies;

        for (int a = 0; a < currentWave.numberOfEnemies; a++)
        {
            if (playerDead)
            {
                yield break;
            }
            GameObject randomEnemy = currentWave.enemies[Random.Range(0, currentWave.enemies.Length)];
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(randomEnemy, randomSpawnPoint.position, randomSpawnPoint.rotation);

            yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
        }

        currentWaveIndex++;
        //if (currentWaveIndex <= waves.Length - 1)
        //{
        //    yield return new WaitForSeconds(timeBetweenWaves);
        //} else 
        if (currentWaveIndex == waves.Length)
        {
            gameEnded = true;
        }
        currentWaveEnded = true;
    }

    IEnumerator Intermission()
    {
        currentWaveEnded = false;
        intermissionDisplay.gameObject.SetActive(true);
        for (int i = 0; i < timeBetweenWaves; i++)
        {
            intermissionDisplay.text = "ZOMBIES INCOMING IN " + (timeBetweenWaves - i).ToString();
            yield return new WaitForSeconds(1f);
        }
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
        pickUpGunText.gameObject.SetActive(false);
        intermissionDisplay.text = "ZOMBIES INCOMING IN " + timeBetweenWaves.ToString();
        StartCoroutine(Intermission());
    }
}

