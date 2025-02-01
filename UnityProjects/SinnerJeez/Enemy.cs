using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected bool isDead = false;
    protected bool canRestartAttack = false;
    [SerializeField] protected bool isAttacking = false;

    [SerializeField] protected int health = 0;
    [SerializeField] protected int damage = 0;
    [SerializeField] protected float attackCooldown = 0f;
    [SerializeField] protected float knockUpTime = 5.0f;
    [SerializeField] protected float knockUpFloatTime = 1.0f;
    [SerializeField] protected float beingPulledSpeed = 1.0f;
    [SerializeField] protected float beingPulledStopDistance = 1.0f;
    [SerializeField] protected bool isKnockupVulnerable = true;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected Transform effectInstantiateLocation;
    [SerializeField] protected Collider2D hitbox;
    [SerializeField] protected BoxCollider2D playerDetector;
    [SerializeField] protected Collider2D whenDeadHitbox;
    [SerializeField] protected EnemyAttackType attackType = EnemyAttackType.Melee;
    [SerializeField] protected Animator animator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public bool IsDead() { return health <= 0; }

    public void TakeDamage(int damage)
    {
        health -= damage;

        OnTakeDamage();
    }

    protected virtual void OnTakeDamage()
    {
        SpawnEffect(hitEffect);
    }

    public GameObject SpawnEffect(GameObject spawner)
    {
        return Instantiate(spawner, effectInstantiateLocation.position, Quaternion.identity);
    }
}
