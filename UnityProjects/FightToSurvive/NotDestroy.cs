using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDestroy : MonoBehaviour
{
    private static NotDestroy instance;

    private void Awake()
    {
        // If the object doesn't already exist, then set this object to not destroy on load
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        } else
        {
            // Object already exists, and does not need to be duplicated. Destroy this object.
            Destroy(gameObject);
        }
    }
}
