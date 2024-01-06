using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        // Get all GameManager objects and delete duplicates
        GameManager[] objs = FindObjectsOfType<GameManager>();
        if (objs.Length > 1)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].gameObject != this.gameObject)
                {
                    Destroy(objs[i].gameObject);
                }
            }
        }

        // Don't destroy this object on load to maintain game state variables between scenes
        DontDestroyOnLoad(this.gameObject);
    }
}
