using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Enemy enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        // Assign references
        enemyScript = GetComponentInParent<Enemy>();   
    }

    public void AttackPlayer()
    {
        enemyScript.DamagePlayer();
    }
}
