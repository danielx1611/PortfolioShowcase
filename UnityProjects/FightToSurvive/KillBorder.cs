using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cowsins;

public class KillBorder : MonoBehaviour
{

    public PlayerStats playerStats;

    // If the player enters the deadzone barrier, kill them.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerStats.Damage(1000);
        }
    }
}
