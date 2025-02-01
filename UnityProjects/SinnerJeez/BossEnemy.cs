using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : Enemy
{
    [SerializeField] private EntitySpawnPoint[] minionSpawnPoints;
    [SerializeField] private GameObject bossText;
    [SerializeField] private GameObject bossHealthBarContainer;
    [SerializeField] private Slider bossHealthSlider;

    private bool isPlayerLockedIntoArena = false;
    private BasicEnemy[] currentMinions;
    private float timerAttack = 0f;
    private PlayerController player;

    [Space(5)]
    [Header("Sounds")]
    [SerializeField] private GameObject attackSound;
    [SerializeField] private GameObject hurtSound;
    [SerializeField] private GameObject deathSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        bossHealthSlider.maxValue = health;
        bossHealthBarContainer.SetActive(false);
        bossText.SetActive(false);

        currentMinions = new BasicEnemy[minionSpawnPoints.Length];
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isDead || isPlayerLockedIntoArena == false) return;

        bossHealthSlider.value = health;

        if (health <= 0)
        {
            DeathSequence();
            return;
        }

        timerAttack -= Time.deltaTime;
        if (timerAttack < 0) timerAttack = 0;

        if (isAttacking)
        {
            if (timerAttack <= 0)
            {
                Attack();
            }
        }
    }

    public void InitiateBossFight()
    {
        isPlayerLockedIntoArena = true;
        bossHealthBarContainer.SetActive(true);
        bossText.SetActive(true);
        Attack();
    }

    private void Attack()
    {
        if (player.IsDead)
        {
            player = null;
            return;
        }

        animator.SetInteger("AttackType", (int)attackType);
        animator.SetTrigger("Attack");
        isAttacking = true;

        timerAttack = attackCooldown;

        Instantiate(attackSound);
    }

    private void SpawnNewMinions()
    {
        for (int i = 0; i <  minionSpawnPoints.Length; i++)
        {
            if (currentMinions[i] != null)
            {
                if (currentMinions[i].IsDead())
                {
                    currentMinions[i] = null;
                }
            }

            if (currentMinions[i] == null)
            {
                currentMinions[i] = minionSpawnPoints[i].SpawnEntity().GetComponent<BasicEnemy>();
            }
        }
    }

    private void DeathSequence()
    {
        animator.SetBool("IsDead", true);
        isDead = true;
        hitbox.enabled = false;
        playerDetector.enabled = false;
        whenDeadHitbox.enabled = true;

        bossHealthBarContainer.SetActive(false);
        bossText.SetActive(false);

        foreach (BasicEnemy minion in currentMinions)
        {
            if (minion && minion.IsDead() == false)
            {
                minion.TakeDamage(500);
            }
        }

        Instantiate(deathSound);

        Destroy(gameObject, 3);
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();

        if (health > 0) Instantiate(hurtSound);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (hitPlayer)
        {
            player = hitPlayer;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (hitPlayer)
        {
            player = hitPlayer;
            isAttacking = false;
        }
    }
}
