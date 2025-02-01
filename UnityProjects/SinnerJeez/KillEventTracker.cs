using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillEventTracker : MonoBehaviour
{
    [Header("Level 2 Event Specifics")]
    [SerializeField] private GameObject hiddenBridge;

    [Space(5)]
    [Header("Level 3 Event Specifics")]
    [SerializeField] private GameObject bossDoor;
    [SerializeField] private GameObject bossBackWall;
    [SerializeField] private GameObject bossInvisibleBarrier;

    private bool isBossDoorGuardDead = false;
    private int numEnemiesKilled = 0;
    private List<BasicEnemy> basicEnemies = new List<BasicEnemy>();

    [SerializeField] private BossEnemy boss;

    private EntitySpawnManager entitySpawnManager;

    private void Awake()
    {
        entitySpawnManager = FindObjectOfType<EntitySpawnManager>();
        entitySpawnManager.OnSpawnEnemy += AddEnemyToList;
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (SceneManager.GetActiveScene().name) 
        {
            case "Level 2":
                hiddenBridge.SetActive(false);
                break;

            case "Level 3":
                bossDoor.SetActive(true);
                bossBackWall.SetActive(true);
                boss = FindObjectOfType<BossEnemy>();
                break;
        }
    }

    private void Update()
    {
        List<BasicEnemy> enemiesToRemove = new List<BasicEnemy>();
        boss = FindObjectOfType<BossEnemy>();

        foreach (BasicEnemy enemy in basicEnemies)
        {
            if (enemy.IsDead())
            {
                IncreaseKillCount();
                enemiesToRemove.Add(enemy);
            }
        }

        foreach(BasicEnemy enemy in enemiesToRemove)
        {
            basicEnemies.Remove(enemy);
        }

        if (boss && boss.IsDead())
        {
            boss = null;

            bossBackWall.SetActive(false);
            bossInvisibleBarrier.SetActive(false);
        }

        enemiesToRemove.Clear();
    }

    private void AddEnemyToList(object sender, GameObject go)
    {
        BasicEnemy basicEnemy = go.GetComponent<BasicEnemy>();
        if (basicEnemy)
        {
            basicEnemies.Add(basicEnemy);
        }
    }

    private void IncreaseKillCount()
    {
        numEnemiesKilled += 1;
        CheckForKillEvents();
    }

    private void CheckForKillEvents()
    {
        switch(SceneManager.GetActiveScene().name) 
        {
            case "Level 2":
                if (numEnemiesKilled >= 2)
                {
                    hiddenBridge.SetActive(true);
                }
                break;

            case "Level 3":
                if (isBossDoorGuardDead == false && numEnemiesKilled == 1)
                {
                    isBossDoorGuardDead = true;
                    bossDoor.SetActive(false);
                }
                break;
        }
        
    }
}
