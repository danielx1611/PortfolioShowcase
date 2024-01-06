using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class DemonLord : MonoBehaviour
{
    [SerializeField] float speed = 4.0f;
    [SerializeField] int damage = 10;
    public Slider healthbar;
    public GameObject bossAttack;
    [HideInInspector] public bool gameOver = false;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private GameManager gameManager;
    private float m_delayTilNextAttack = 0.75f;
    private float m_delayTilNextAttackTimer;
    private bool m_isAttackDelayed = false;
    private bool m_isAttacking;
    private ParticleSystem partSys;

    [HideInInspector] public List<GameObject> colliderList = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_delayTilNextAttackTimer = m_delayTilNextAttack;
        gameManager = FindObjectOfType<GameManager>();
        partSys = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (m_isAttackDelayed)
        {
            m_delayTilNextAttackTimer -= Time.deltaTime;
            if (m_delayTilNextAttackTimer <= 0)
            {
                m_isAttackDelayed = false;
                m_delayTilNextAttackTimer = m_delayTilNextAttack;
                m_isAttacking = false;
            }
        }

        float inputX = Input.GetAxisRaw("Horizontal");

        /*// Swap direction of sprite depending on walk direction
        if (!m_isAttacking)
        {
            if (inputX > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }*/

        //Attack
        if (Input.GetMouseButtonDown(0) && !m_isAttackDelayed)
        {
            m_isAttacking = true;
            m_isAttackDelayed = true;
            m_animator.SetTrigger("Attack");
            Instantiate(bossAttack, transform.position, transform.rotation);
        }

        if (m_isAttacking)
        {
            m_body2d.velocity = new Vector2(0, 0);
        }

        // Move
        if (!m_isAttacking && inputX > 0)
        {
            m_body2d.velocity = new Vector2(inputX, m_body2d.velocity.y).normalized * speed;
        }
    }

    /*public void TakeDamage(int damage)
    {
        if (!m_isDead && !m_isBlocking)
        {
            gameManager.playerHealth -= damage;
            if (gameManager.playerHealth <= 0)
            {
                m_animator.SetTrigger("Death");
                m_isDead = true;
            }
            else
            {
                m_animator.SetTrigger("Hurt");
            }
        }
    }*/

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!colliderList.Contains(collider.gameObject) && collider.tag == "Enemy")
        {
            colliderList.Add(collider.gameObject);
            // Debug.Log("Added " + collider.gameObject.name);
            // Debug.Log("GameObjects in list: " + colliderList.Count);
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (!GetComponent<BoxCollider2D>().IsTouching(collider))
        {
            if (colliderList.Contains(collider.gameObject))
            {
                colliderList.Remove(collider.gameObject);
                // Debug.Log("Removed " + collider.gameObject.name);
                // Debug.Log("GameObjects in list: " + colliderList.Count);
            }
        }
    }

    public void DamageAllEnemiesInRange()
    {
        partSys.time = 0f;
        partSys.Play();
        foreach (GameObject enemy in colliderList.ToList())
        {
            AIKnight enemyScript = enemy.GetComponent<AIKnight>();
            enemyScript.TakeDamage(damage);
        }
    }
}
