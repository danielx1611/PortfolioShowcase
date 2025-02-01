using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject entityToSpawn;

    public GameObject SpawnEntity()
    {
        return Instantiate(entityToSpawn, transform.position, Quaternion.identity);
    }
}
