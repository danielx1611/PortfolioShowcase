using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum EnemyAttackType
{
    Melee = 0,
    Bow = 1,
    Magic = 2
}

public class BasicEnemy : Enemy
{

    public float moveSpeed = 3f;            // Enemy movement speed
    public Transform groundCheck;           // Point to check if there is ground ahead
    public float groundCheckRadius = 0.2f;  // Radius for the ground check
    public Transform cliffCheck;            // Point to check if there is cliff ahead
    public float cliffCheckRadius = 0.2f;   // Radius for the cliff check
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask groundLayer;           // Layer that defines what is considered ground

    protected bool isMovingLeft = true;    // Tracks the current direction of the enemy
    protected PlayerController nearbyPlayer = null;
    protected GameObject pullingTarget = null;

    private bool isKnockedUp = false;
    private bool isBeingPulled = false;
    private float timerKnockUp = 0f;
    private float timerAttack = 0f;
    [SerializeField] protected bool isGrounded = false;

    [Space(5)]
    [Header("Sounds")]
    [SerializeField] private GameObject meleeSound;
    [SerializeField] private GameObject magicSound;
    [SerializeField] private GameObject hurtSound;
    [SerializeField] private GameObject deathSound;

    private GameObject currentKnockupEffect = null;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (isDead)
        {
            return;
        }

        if (health <= 0)
        {
            DeathSequence();
            return;
        }

        timerAttack -= Time.deltaTime;
        if (timerAttack < 0) timerAttack = 0;

        if (isBeingPulled)
        {
            timerKnockUp = 0.1f;
            if (health <= 0)
            {
                DeathSequence();
                return;
            }

            return;
        }

        if (isKnockedUp)
        {
            timerKnockUp -= Time.deltaTime;

            if (timerKnockUp <= 0)
            {
                isKnockedUp = false;
                UndoKnockup();
            }
            return;
        }

        // Check if there's ground below enemy to stand on
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isAttacking)
        {
            if (timerAttack <= 0)
            {
                Attack();
            } else if (isGrounded)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("Velocity", rb.velocity.magnitude);
            }
        } else
        {
            Patrol();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (isBeingPulled)
        {
            MoveTowardsPullingTarget();
        }
    }

    public GameObject SpawnChildEffect(GameObject spawner)
    {
        return Instantiate(spawner, effectInstantiateLocation);
    }

    protected virtual void DeathSequence()
    {
        animator.SetBool("IsDead", true);
        isDead = true;
        UndoKnockup();
        isKnockedUp = false;
        isBeingPulled = false;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        hitbox.enabled = false;
        playerDetector.enabled = false;
        whenDeadHitbox.enabled = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        Instantiate(deathSound);

        Destroy(gameObject, 3);
    }

    public virtual void TryKnockup(GameObject effectSpawner)
    {
        if (isKnockupVulnerable && !isKnockedUp && isGrounded)
        {
            DoKnockup(effectSpawner);

            isGrounded = false;
            isKnockedUp = true;
            timerKnockUp = knockUpTime;
        }
    }

    protected void DoKnockup(GameObject effectSpawner)
    {
        rb.gravityScale = -0.5f;

        StartCoroutine(KnockupFloat(effectSpawner));

        animator.SetFloat("Velocity", Math.Abs(rb.velocity.x));
    }

    protected void UndoKnockup()
    {
        rb.gravityScale = 1;
        animator.SetBool("KnockedUp", false);
        if (currentKnockupEffect)
        {
            Destroy(currentKnockupEffect);
            currentKnockupEffect = null;
        }
    }

    public bool CheckKnockedUp()
    {
        return isKnockedUp;
    }

    public void PullTowards(GameObject target, float pullDuration)
    {
        pullingTarget = target;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        isKnockedUp = false;
        animator.SetBool("KnockedUp", false);
        animator.SetFloat("Velocity", 0);

        isBeingPulled = true;
        gameObject.layer = LayerMask.NameToLayer("PulledEnemy");
        StartCoroutine(PullTowardsForDuration(pullDuration));
    }

    private void MoveTowardsPullingTarget()
    {
        // Calculate the direction towards the player
        Vector2 direction = (pullingTarget.transform.position - transform.position);

        // Get distance from player
        float distance = direction.magnitude;

        // Get normalized direction for pulling speed velocity calculations
        Vector2 normalizedDirection = (pullingTarget.transform.position - transform.position).normalized;

        float margin = 0.1f; // Margin of error for distance calculation

        if (distance > beingPulledStopDistance + margin)
        {
            // Set the Rigidbody's velocity towards the player
            rb.velocity = normalizedDirection * beingPulledSpeed;
        } else if (distance < beingPulledStopDistance - margin)
        {
            // Too close to player, push back slightly
            rb.velocity = normalizedDirection * -beingPulledSpeed;
        }
        else
        {
            // Stop entity, no need to get closer or else it'll drag player along
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void Patrol()
    {
        // Turn around if there is no ground ahead
        if (isGrounded)
        {
            // Move the enemy left or right
            rb.velocity = new Vector2((isMovingLeft ? -1 : 1) * moveSpeed, rb.velocity.y);

            animator.SetFloat("Velocity", Math.Abs(rb.velocity.x));

            // Check if at cliff or running into wall. If so, turn around
            bool isRoomToWalk = Physics2D.OverlapCircle(cliffCheck.position, cliffCheckRadius, groundLayer);
            bool isRunningIntoWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
            if (!isRoomToWalk || isRunningIntoWall)
            {
                Flip();
            }
        }
    }

    protected virtual void Attack(bool forceIsAttacking = false)
    {
        if (forceIsAttacking)
        {
            isAttacking = forceIsAttacking;
        }

        if (isGrounded == false || timerAttack > 0) return;

        if (nearbyPlayer.IsDead)
        {
            nearbyPlayer = null;
            return;
        }

        rb.velocity = new Vector2(0, 0);

        animator.SetInteger("AttackType", (int)attackType);
        animator.SetTrigger("Attack");
        animator.SetFloat("Velocity", rb.velocity.x);

        if (attackType == EnemyAttackType.Melee) 
        {
            Instantiate(meleeSound);
        } else if (attackType == EnemyAttackType.Magic)
        {
            Instantiate(magicSound);
        }

        timerAttack = attackCooldown;
    }

    // Called by animator
    private void TryDealDamage()
    {
        if (nearbyPlayer != null)
        {
            nearbyPlayer.TakeDamage(damage);
        }
    }

    private void Flip()
    {
        isMovingLeft = !isMovingLeft;

        // Flip the sprite's direction by scaling it
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();

        if (health > 0) Instantiate(hurtSound);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player && !isBeingPulled)
        {
            nearbyPlayer = player;
            Attack(true);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player)
        {
            nearbyPlayer = null;
            isAttacking = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cliffCheck.position, cliffCheckRadius);
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }

    private IEnumerator KnockupFloat(GameObject effectSpawner)
    {
        // Stop current momentum
        rb.velocity = new Vector2(0, 0);

        // Wait for knockup
        yield return new WaitForSeconds(knockUpFloatTime);

      
        // Stop motion after knockup
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);

        // Play stun animation
        animator.SetBool("KnockedUp", true);

        currentKnockupEffect = SpawnChildEffect(effectSpawner);
    }

    private IEnumerator PullTowardsForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        isBeingPulled = false;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;

        if (currentKnockupEffect)
        {
            Destroy(currentKnockupEffect); 
            currentKnockupEffect = null;
        }
    }
}
