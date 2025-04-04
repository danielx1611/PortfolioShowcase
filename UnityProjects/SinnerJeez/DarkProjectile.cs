using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkProjectile : MonoBehaviour
{
    [HideInInspector] public int damage = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
