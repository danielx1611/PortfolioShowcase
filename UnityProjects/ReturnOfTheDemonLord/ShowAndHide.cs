using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAndHide : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
