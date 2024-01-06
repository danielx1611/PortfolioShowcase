using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIKnight : MonoBehaviour
{
    // Start is called before the first frame update
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

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_animator.speed = 0.5f;
        m_body2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<DemonLord>().GetComponent<Transform>();
        timeForAttackTimer = timeForAttack;
        m_animator.SetBool("noBlood", m_noBlood);
    }

    // Update is called once per frame
    void Update()
    {

        // -- Handle movement --

        if (m_isDead)
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
        if (Vector2.Distance(player.transform.position, transform.position) < 15f)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

            m_body2d.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * m_speed;

            if (Vector2.Distance(player.transform.position, transform.position) < 2f && !m_isAttacking)
            {
                m_body2d.velocity = Vector2.zero;
                m_animator.SetTrigger("Attack1");
                knightAttackSource = Instantiate(knightAttack, transform.position, transform.rotation).GetComponent<AudioSource>();
                m_isAttacking = true;
            }
        }
        else
        {
            m_body2d.velocity = Vector2.zero;
        }

        // Swap direction of sprite depending on walk direction
        /*if (m_body2d.velocity.x > 0)
            transform.localScale = new Vector3(-sizeScale, sizeScale, sizeScale);
        else if (m_body2d.velocity.x < 0)
            transform.localScale = new Vector3(sizeScale, sizeScale, sizeScale);*/

        // -- Handle Animations --

/*        //Run
        if (Mathf.Abs(m_body2d.velocity.x) > Mathf.Epsilon || Mathf.Abs(m_body2d.velocity.y) > Mathf.Epsilon)
            m_animator.SetBool("isMoving", true);

        //Idle
        else
            m_animator.SetBool("isMoving", false);*/
    }

    public void DamagePlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < 1f)
        {
            // playerDL.TakeDamage(m_damage);
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (!m_isDead)
        {
            if (m_health <= 0)
            {
                m_animator.SetTrigger("Death");
                m_isDead = true;
                Instantiate(knightDeath, transform.position, transform.rotation);
            }
            else
            {
                // m_animator.SetTrigger("Hurt");
            }
        }
        if (knightAttackSource != null)
        {
            knightAttackSource.Stop();
        }
    }

    public void DamageAllEnemiesInRange()
    {
        // Redundant
    }
}
