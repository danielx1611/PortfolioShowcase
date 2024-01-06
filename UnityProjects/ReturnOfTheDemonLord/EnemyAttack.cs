using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Enemy enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponentInParent<Enemy>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackPlayer()
    {
        enemyScript.DamagePlayer();
    }
}
