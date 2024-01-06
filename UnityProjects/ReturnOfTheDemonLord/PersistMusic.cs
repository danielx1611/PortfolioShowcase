using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopMusic()
    {
        Destroy(this.gameObject);
    }
}
