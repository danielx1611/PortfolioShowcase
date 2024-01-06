using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    public int m_health;
    public int m_damage;
    public float sizeScale;
    public GameObject knightDeath;
    public GameObject knightAttack;

    private AudioSource knightAttackSource;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private bool m_isDead = false;
    private bool m_isAttacking;
    private float timeForAttack = 1f;
    private float timeForAttackTimer;

    private Transform player;
    private bool m_noBlood = false;

    void Start()
    {
        // Set references
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<DemonLord>().GetComponent<Transform>();

        // Configure animator settings
        m_animator.speed = 0.5f;
        m_animator.SetBool("noBlood", m_noBlood);

        // Set timer to starting value
        timeForAttackTimer = timeForAttack;
    }

    // Update is called once per frame
    void Update()
    {
        // Disable collisions on death to prevent AI stopping player movement
        if (m_isDead)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            return;
        }

        // Count down until AI stops attacking to prevent trying to attack multiple times at once
        if (m_isAttacking)
        {
            timeForAttackTimer -= Time.deltaTime;
            if (timeForAttackTimer <= 0)
            {
                // AI has finished attacking
                timeForAttackTimer = timeForAttack;
                m_isAttacking = false;
            }
            return;
        }

        // Move
        if (Vector2.Distance(player.transform.position, transform.position) < 15f)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            // Calculate velocity of AI knight in direction of player
            m_body2d.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * m_speed;

            // If close enough, start attacking (does no damage, AI Knights are for visuals only)
            if (Vector2.Distance(player.transform.position, transform.position) < 2f && !m_isAttacking)
            {
                // Stop AI from moving
                m_body2d.velocity = Vector2.zero;

                // Follow through with attack sequence
                m_animator.SetTrigger("Attack1");
                knightAttackSource = Instantiate(knightAttack, transform.position, transform.rotation).GetComponent<AudioSource>();
                m_isAttacking = true;
            }
        }
        else
        {
            // Player target out of range
            m_body2d.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (!m_isDead)
        {
            if (m_health <= 0)
            {
                // Follow through with death sequence
                m_animator.SetTrigger("Death");
                m_isDead = true;
                Instantiate(knightDeath, transform.position, transform.rotation);
            }
        }
        if (knightAttackSource != null)
        {
            knightAttackSource.Stop();
        }
    }
}
