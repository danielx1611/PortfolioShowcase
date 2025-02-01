using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrinkOverTime : MonoBehaviour
{
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 1f);
    public Vector3 maxScale = new Vector3(1.5f, 1.5f, 1f);
    public float speed = 2f;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2; // Generates a value between 0 and 1
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);
    }
}
