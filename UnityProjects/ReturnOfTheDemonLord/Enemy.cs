using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float m_speed = 4.0f;
    public int m_health;
    public int m_damage;
    public float sizeScale;
    public GameObject deathEffect;
    public GameObject enemyAttackSound;
    public GameObject enemyHurtSound;
    public GameObject enemyDeathSound;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private bool m_isDead = false;
    private bool m_isAttacking;
    private float timeForAttack = 1f;
    private float timeForAttackTimer;

    private Transform player;
    private HeroKnight playerHK;

    private AudioSource enemyAttackSource;

    // Use this for initialization
    void Start()
    {
        // Assign references
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<HeroKnight>().GetComponent<Transform>();
        playerHK = FindObjectOfType<HeroKnight>();

        // Set animator to half speed for slower animations
        m_animator.speed = 0.5f;

        // Set attack timer to starting timer value
        timeForAttackTimer = timeForAttack;

        // Scale enemy relative to specified size, where only x can be negative in order to "flip" the enemy direction
        transform.localScale = new Vector3(sizeScale, Mathf.Abs(sizeScale), Mathf.Abs(sizeScale));
    }

    // Update is called once per frame
    void Update()
    {

        // If enemy is dead or player is dead, disable collisions and return, as the game is over or the enemy is dead and shouldn't move
        if (m_isDead || playerHK.m_isDead)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            return;
        }

        // Countdown to prevent enemy from attacking multiple times at once
        if (m_isAttacking)
        {
            timeForAttackTimer -= Time.deltaTime;
            if (timeForAttackTimer <= 0)
            {
                // Countdown is done, finish attack
                timeForAttackTimer = timeForAttack;
                m_isAttacking = false;
            }
            return;
        }

        // Move
        if (Vector2.Distance(player.transform.position, transform.position) < 5f)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            // Calculate speed towards player
            m_body2d.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * m_speed;

            // If in range, attack if not already attacking
            if (Vector2.Distance(player.transform.position, transform.position) < 1f && !m_isAttacking)
            {
                // Stop moving then attack the player
                m_body2d.velocity = Vector2.zero;
                m_animator.SetTrigger("Attack");
                enemyAttackSource = Instantiate(enemyAttackSound, transform.position, transform.rotation).GetComponent<AudioSource>();
                m_isAttacking = true;
            }
        }
        else
        {
            // Player out of range, stop chasing
            m_body2d.velocity = Vector2.zero;
        }

        // Swap direction of sprite depending on walk direction
        if (m_body2d.velocity.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(sizeScale), Mathf.Abs(sizeScale), Mathf.Abs(sizeScale));
        else if (m_body2d.velocity.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(sizeScale), Mathf.Abs(sizeScale), Mathf.Abs(sizeScale));

        // -- Handle Animations --

        //Run
        if (Mathf.Abs(m_body2d.velocity.x) > Mathf.Epsilon || Mathf.Abs(m_body2d.velocity.y) > Mathf.Epsilon)
            m_animator.SetBool("isMoving", true);

        //Idle
        else
            m_animator.SetBool("isMoving", false);
    }

    public void DamagePlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < 1f)
        {
            // Player in range of attack, attempt to damage player
            playerHK.TakeDamage(m_damage);
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (!m_isDead)
        {
            if (m_health <= 0)
            {
                // Initiate death sequence then destroy the enemy body
                m_isDead = true;
                Instantiate(deathEffect, transform.position, transform.rotation);
                Instantiate(enemyDeathSound, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                // Play hurt animation and sound, then stagger next attack timer to allow player to combo the enemy
                m_animator.SetTrigger("Hurt");
                Instantiate(enemyHurtSound, transform.position, transform.rotation);
                timeForAttackTimer = 0.3f;
            }
        }
        if (enemyAttackSource != null)
        {
            // Stop attack sound if enemy gets hit, as attack animation stops too
            enemyAttackSource.Stop();
        }
    }
}
