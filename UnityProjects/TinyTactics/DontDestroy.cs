using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
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

        DontDestroyOnLoad(this.gameObject);
    }
}