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
        // Assign references
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        partSys = GetComponentInChildren<ParticleSystem>();

        // Set timer to starting timer value
        m_delayTilNextAttackTimer = m_delayTilNextAttack;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            // Do nothing, game is over
            return;
        }

        if (m_isAttackDelayed)
        {
            m_delayTilNextAttackTimer -= Time.deltaTime;
            if (m_delayTilNextAttackTimer <= 0)
            {
                // Countdown time until player boss can attack again
                m_isAttackDelayed = false;
                m_delayTilNextAttackTimer = m_delayTilNextAttack;
                m_isAttacking = false;
            }
        }

        // Get user input on x axis to move forward
        float inputX = Input.GetAxisRaw("Horizontal");

        //Attack
        if (Input.GetMouseButtonDown(0) && !m_isAttackDelayed)
        {
            // Initiate attack sequence
            m_isAttacking = true;
            m_isAttackDelayed = true;
            m_animator.SetTrigger("Attack");

            // Spawn sound block to play boss attack sound
            Instantiate(bossAttack, transform.position, transform.rotation);
        }

        // Stop moving if attacking
        if (m_isAttacking)
        {
            m_body2d.velocity = new Vector2(0, 0);
        }

        // Move
        if (!m_isAttacking && inputX > 0)
        {
            // Move if not attacking and player is moving forward
            m_body2d.velocity = new Vector2(inputX, m_body2d.velocity.y).normalized * speed;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!colliderList.Contains(collider.gameObject) && collider.tag == "Enemy")
        {
            // Add enemies to list of Area of Effect attack targets when in range of player boss
            colliderList.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (!GetComponent<BoxCollider2D>().IsTouching(collider))
        {
            if (colliderList.Contains(collider.gameObject))
            {
                // Remove enemies from list of Area of Effect attack targets when no longer in range of player boss
                colliderList.Remove(collider.gameObject);
            }
        }
    }

    public void DamageAllEnemiesInRange()
    {
        // Reset particle effect and play it to show attack has an Area of Effect
        partSys.time = 0f;
        partSys.Play();

        // Damage all of the enemies in range of the player boss
        foreach (GameObject enemy in colliderList.ToList())
        {
            AIKnight enemyScript = enemy.GetComponent<AIKnight>();
            enemyScript.TakeDamage(damage);
        }
    }
}
