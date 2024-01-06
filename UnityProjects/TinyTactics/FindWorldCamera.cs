using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWorldCamera : MonoBehaviour
{
    Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        // Get world camera for instantiated world space canvas elements
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = FindObjectOfType<Camera>();
    }
}
