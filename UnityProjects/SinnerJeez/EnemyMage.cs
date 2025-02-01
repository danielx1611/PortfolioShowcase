using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMage : BasicEnemy
{
    [SerializeField] private float darkProjectileSpeed;
    [SerializeField] private GameObject darkProjectile;
    [SerializeField] private Transform projectileSpawnPoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Attack(bool forceIsAttacking = false)
    {
        base.Attack(forceIsAttacking);
    }

    protected override void Patrol()
    {
        base.Patrol();
    }

    public void DoFireballAttack()
    {
        if (nearbyPlayer == null) return;

        Transform t = nearbyPlayer.gameObject.transform;

        if (t == null) return;

        Vector3 playerAttackOffset = new Vector3(t.position.x, t.position.y + 0.4f, t.position.z);

        Vector2 direction = (playerAttackOffset - projectileSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject spawnedProjectile = Instantiate(darkProjectile, projectileSpawnPoint.position, Quaternion.Euler(0, 0, angle));

        spawnedProjectile.GetComponent<Rigidbody2D>().velocity = direction.normalized * darkProjectileSpeed;
        DarkProjectile projectile = spawnedProjectile.GetComponent<DarkProjectile>();
        projectile.damage = damage;
    }
}
