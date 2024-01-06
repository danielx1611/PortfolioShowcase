using cowsins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveZombie : Zombie
{
    public float explosionDistance;
    public ExplosiveBarrel barrel;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        // If in range of target, explode the zombie
        if (Vector3.Distance(transform.position, target.position) < explosionDistance && !isDead)
        {
            barrel.Die();
        }
    }
}
