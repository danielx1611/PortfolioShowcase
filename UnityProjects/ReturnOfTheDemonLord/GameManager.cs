using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playerHealth;

    private void Awake()
    {
        // Get one or both instances of the GameManager script, then remove duplicate if needed
        GameManager[] objs = GameObject.FindObjectsOfType<GameManager>();
        if (objs.Length > 1)
        {
            if (objs[0] == gameObject.GetComponent<GameManager>())
            {
                Destroy(objs[1]);
            } else
            {
                Destroy(objs[0]);
            }
        }

        // Don't destroy this object on scene load
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Update player health bar to reflect player's current health
        Slider healthBar = FindObjectOfType<Slider>();
        if (healthBar != null)
        {
            healthBar.value = playerHealth;
        }
    }
}
