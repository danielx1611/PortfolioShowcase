using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAtConstantSpeed : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0f;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, 0, rotationSpeed);
    }
}
