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
        m_animator = GetComponentInChildren<Animator>();
        m_animator.speed = 0.5f;
        m_body2d = GetComponent<Rigidbody2D>();
        timeForAttackTimer = timeForAttack;
        player = FindObjectOfType<HeroKnight>().GetComponent<Transform>();
        playerHK = FindObjectOfType<HeroKnight>();
        transform.localScale = new Vector3(sizeScale, Mathf.Abs(sizeScale), Mathf.Abs(sizeScale));
    }

    // Update is called once per frame
    void Update()
    {

        // -- Handle movement --

        if (m_isDead || playerHK.m_isDead)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            return;
        }

        if (m_isAttacking)
        {
            timeForAttackTimer -= Time.deltaTime;
            if (timeForAttackTimer <= 0)
            {
                timeForAttackTimer = timeForAttack;
                m_isAttacking = false;
            }
            return;
        }

        // Move
        if (Vector2.Distance(player.transform.position, transform.position) < 5f)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            m_body2d.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * m_speed;

            if (Vector2.Distance(player.transform.position, transform.position) < 1f && !m_isAttacking)
            {
                m_body2d.velocity = Vector2.zero;
                m_animator.SetTrigger("Attack");
                enemyAttackSource = Instantiate(enemyAttackSound, transform.position, transform.rotation).GetComponent<AudioSource>();
                m_isAttacking = true;
            }
        }
        else
        {
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
                m_isDead = true;
                Instantiate(deathEffect, transform.position, transform.rotation);
                Instantiate(enemyDeathSound, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                m_animator.SetTrigger("Hurt");
                Instantiate(enemyHurtSound, transform.position, transform.rotation);
                timeForAttackTimer = 0.3f;
            }
        }
        if (enemyAttackSource != null)
        {
            enemyAttackSource.Stop();
        }
    }
}
