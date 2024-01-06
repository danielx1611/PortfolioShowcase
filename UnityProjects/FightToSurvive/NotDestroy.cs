using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDestroy : MonoBehaviour
{
    private static NotDestroy instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        } else
        {
            Destroy(gameObject);
        }
    }
}
