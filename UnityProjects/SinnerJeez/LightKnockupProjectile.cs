using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightKnockupProjectile : MonoBehaviour
{
    public float rotationSpeed = 360f; // Degrees per second
    public GameObject KnockUpEffect;

    // Update is called once per frame
    void Update()
    {
        // Rotate the projectile around its Z-axis at a constant speed
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicEnemy enemy = collision.GetComponent<BasicEnemy>();

        if (enemy)
        {
            enemy.TryKnockup(KnockUpEffect);
        }

        Destroy(gameObject);
    }
}
