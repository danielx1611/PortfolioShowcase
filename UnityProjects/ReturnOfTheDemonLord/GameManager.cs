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
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Slider healthBar = FindObjectOfType<Slider>();
        if (healthBar != null)
        {
            healthBar.value = playerHealth;
        }
    }
}
