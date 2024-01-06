using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get music player objects and destroy any duplicates
        PersistMusic[] objs = GameObject.FindObjectsOfType<PersistMusic>();
        if (objs.Length > 1)
        {
            if (objs[0] == gameObject.GetComponent<PersistMusic>())
            {
                Destroy(objs[1]);
            }
            else
            {
                Destroy(objs[0]);
            }
        }

        // Dont destroy this object on scene load to make music persist between levels
        DontDestroyOnLoad(this.gameObject);
    }

    public void StopMusic()
    {
        // Destroy music player
        Destroy(this.gameObject);
    }
}
