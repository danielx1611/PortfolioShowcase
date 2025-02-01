using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnManager : MonoBehaviour
{
    public event EventHandler<GameObject> OnSpawnEnemy;

    [SerializeField] private List<EntitySpawnPoint> enemySpawnPoints;
    [SerializeField] private EntitySpawnPoint playerSpawnPoint;
    [SerializeField] private EntitySpawnPoint bossSpawnPoint;

    private bool hasBoss = false;

    private void Start()
    {
        if (bossSpawnPoint != null)
        {
            hasBoss = true;
        }
    }

    public void SpawnEnemies()
    {
        foreach (EntitySpawnPoint point in enemySpawnPoints)
        {
            GameObject go = point.SpawnEntity();
            OnSpawnEnemy?.Invoke(this, go);
        }
    }

    public PlayerController SpawnPlayer()
    {
        PlayerController player = playerSpawnPoint.SpawnEntity().GetComponent<PlayerController>();
        return player;
    }

    public void SpawnBoss()
    {
        //BossEnemy boss = bossSpawnPoint.SpawnEntity().GetComponent<BossEnemy>();
    }

    public bool HasBoss() {  return hasBoss; }
}
