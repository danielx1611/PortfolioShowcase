using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Horizontal movement speed
    public float jumpForce = 10f; // Force of the jump
    public LayerMask groundLayer; // Define which layer is ground
    public Transform groundCheck; // Point to check if the player is on the ground
    public float groundCheckWidth = 0.2f; // Radius for ground check
    public bool IsDead = false;
    public Transform CameraOffsetTransform;

    [SerializeField] private int health = 0;
    [SerializeField] private int darkModeDamage = 0;
    [SerializeField] private int darkModeUltimateDamage = 0;
    [SerializeField] private int lightProjectileSpeed = 0;
    [SerializeField] private float enemyPullDuration = 0f;
    [SerializeField] private float ultimateRadius = 0f;
    [SerializeField] private Color blessedColor;
    [SerializeField] private Color sinnerColor;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator darkTransitionAnimator;
    [SerializeField] private Animator lightTransitionAnimator;
    [SerializeField] private Animator darkUltimateAnimator;
    [SerializeField] private SpriteRenderer lightTransitionRenderer;
    [SerializeField] private SpriteRenderer darkTransitionRenderer;
    [SerializeField] private SpriteRenderer darkUltimateRenderer;
    [SerializeField] private GameObject passiveDarkFire;
    [SerializeField] private GameObject lightProjectile;
    [SerializeField] private Transform lightProjSpawnPoint;
    [SerializeField] private GameObject lightKnockupProjectile;
    [SerializeField] private GameObject takeDamageEffect;
    [SerializeField] private Transform effectSpawningLocation;
    [SerializeField] private GameObject knockUpEffect;

    [SerializeField] private List<SpriteRenderer> lightHeroRenderers;
    [SerializeField] private List<SpriteRenderer> darkHeroRenderers;
    [SerializeField] private TetherManager tetherManager;

    [SerializeField] private BoxCollider2D enemyDetector;

    private Rigidbody2D rb;
    private Collider2D hitbox;
    private bool isGrounded;
    private bool isBlessedMode = true;

    private List<Enemy> enemyList;

    [SerializeField] private float swapCooldown = 0.4f;
    private float swapTimer = 0;
    [SerializeField] private float attackCooldown = 0.4f;
    private float lightAttackTimer = 0;
    private float darkAttackTimer = 0;
    [SerializeField] private float abilityCooldown = 0.4f;
    private float lightAbilityTimer = 0;
    private float darkAbilityTimer = 0;
    [SerializeField] private float ultimateCooldown = 0.4f;
    private float lightUltimateTimer = 0;
    private float darkUltimateTimer = 0;

    [Space(5)]
    [Header("Sounds")]
    [SerializeField] private GameObject meleeSound;
    [SerializeField] private GameObject magicSound;
    [SerializeField] private GameObject hurtSound;
    [SerializeField] private GameObject deathSound;
    [SerializeField] private GameObject transformationSound;
    [SerializeField] private GameObject explosionSound;

    private float enemyPullTimer = 0;

    private Vector3 mousePosition;

    private BasicEnemy[] currentBasicEnemies;

    private PlayerAbilityIconManager iconManager;
    private GameManager gm;
    private Slider healthBar;

    void Start()
    {
        enemyList = new List<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();

        lightTransitionAnimator.gameObject.SetActive(false);
        darkTransitionAnimator.gameObject.SetActive(false);
        passiveDarkFire.SetActive(false);
        lightTransitionRenderer.sprite = null;
        darkTransitionRenderer.sprite = null;
        darkUltimateRenderer.sprite = null;

        iconManager = FindObjectOfType<PlayerAbilityIconManager>();
        gm = FindObjectOfType<GameManager>();
        healthBar = FindObjectOfType<Slider>();
        healthBar.maxValue = health;
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        healthBar.value = health;

        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            DeathSequence();
            return;
        }

        // Check timers to make sure they aren't less than 0
        UpdateTimers();

        // Update cooldown icons
        if (isBlessedMode)
        {
            iconManager.UpdateCooldownIcons(swapTimer / swapCooldown, lightAttackTimer / attackCooldown, lightAbilityTimer / abilityCooldown, lightUltimateTimer / ultimateCooldown);
        } else
        {
            iconManager.UpdateCooldownIcons(swapTimer / swapCooldown, darkAttackTimer / attackCooldown, darkAbilityTimer / abilityCooldown, darkUltimateTimer / ultimateCooldown);
        }
        
        // Get horizontal input
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Move the player
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Velocity", rb.velocity.magnitude);

        // Set player orientation
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        } else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Check if the player is grounded
        // isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, 0.2f), 0, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Transition
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TransitionPlayerForm();
        } else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            UtilityAbility();
        } else if (Input.GetKeyDown(KeyCode.Q))
        {
            UltimateAbility();
        }
    }

    private void UpdateTimers()
    {
        swapTimer -= Time.deltaTime;
        lightAttackTimer -= Time.deltaTime;
        darkAttackTimer -= Time.deltaTime;
        lightAbilityTimer -= Time.deltaTime;
        darkAbilityTimer -= Time.deltaTime;
        lightUltimateTimer -= Time.deltaTime;
        darkUltimateTimer -= Time.deltaTime;

        enemyPullTimer -= Time.deltaTime;

        swapTimer = swapTimer < 0 ? 0 : swapTimer;
        lightAttackTimer = lightAttackTimer < 0 ? 0 : lightAttackTimer;
        darkAttackTimer = darkAttackTimer < 0 ? 0 : darkAttackTimer;
        lightAbilityTimer = lightAbilityTimer < 0 ? 0 : lightAbilityTimer;
        darkAbilityTimer = darkAbilityTimer < 0 ? 0 : darkAbilityTimer;
        lightUltimateTimer = lightUltimateTimer < 0 ? 0 : lightUltimateTimer;
        darkUltimateTimer = darkUltimateTimer < 0 ? 0 : darkUltimateTimer;

        if (enemyPullTimer <= 0)
        {
            tetherManager.DestroyAllTethers();
            enemyPullTimer = 0;
        } else
        {
            tetherManager.UpdateTethers();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        OnTakeDamage();
    }

    private void OnTakeDamage()
    {
        Instantiate(takeDamageEffect, effectSpawningLocation.position, Quaternion.identity);

        if (health > 0) Instantiate(hurtSound);
    }

    private void DeathSequence()
    {
        animator.SetBool("IsDead", true);
        IsDead = true;

        hitbox.enabled = false;
        enemyDetector.enabled = false;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        Instantiate(deathSound);

        gm.DoLoadLevelSequence(sceneName: SceneManager.GetActiveScene().name);
    }

    private void UpdateMousePosition()
    {
        // Get the mouse position in screen space
        Vector3 screenPosition = Input.mousePosition;

        // Convert the screen position to world space
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
    }

    private void UtilityAbility()
    {
        UpdateMousePosition();

        if (isBlessedMode)
        {
            // Check blessed cooldown
            if (lightAbilityTimer > 0) return;
            lightAbilityTimer = abilityCooldown;
        } else
        {
            // Check sinner cooldown
            if (darkAbilityTimer > 0) return;
            darkAbilityTimer = abilityCooldown;
        }

        animator.SetBool("IsLightMode", isBlessedMode);
        animator.SetTrigger("Ability");

        if (isBlessedMode)
        {
            Instantiate(magicSound);
        }
        else
        {
            Instantiate(meleeSound);
        }
    }

    // Called by animator
    private void DoBlessedAbility()
    {
        // Light mode ability
        Vector2 direction = (mousePosition - lightProjSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject spawnedProjectile = Instantiate(lightKnockupProjectile, lightProjSpawnPoint.position, Quaternion.Euler(0, 0, angle));

        spawnedProjectile.GetComponent<Rigidbody2D>().velocity = direction.normalized * lightProjectileSpeed;
    }

    // Called by animator
    private void DoSinnerAbility()
    {
        // Dark mode ability
        currentBasicEnemies = FindObjectsOfType<BasicEnemy>();

        bool chainedAnyEnemies = false;
        foreach (BasicEnemy enemy in currentBasicEnemies)
        {
            if (enemy.CheckKnockedUp())
            {
                enemy.PullTowards(gameObject, enemyPullDuration);
                tetherManager.CreateNewTether(enemy.gameObject);

                chainedAnyEnemies |= true;
            }
        }

        if (chainedAnyEnemies)
        {
            enemyPullTimer = enemyPullDuration;
        }
    }

    private void UltimateAbility()
    {
        UpdateMousePosition();

        if (isBlessedMode)
        {
            // Check cooldown for blessed ultimate
            if (lightUltimateTimer > 0) return;
            else lightUltimateTimer = ultimateCooldown;
        } else
        {
            // Check cooldown for sinner ultimate
            if (darkUltimateTimer > 0) return;
            else darkUltimateTimer = ultimateCooldown;
        }

        animator.SetBool("IsLightMode", isBlessedMode);
        animator.SetTrigger("Ultimate");

        if (isBlessedMode)
        {
            Instantiate(magicSound);
        }
        else
        {
            Instantiate(explosionSound);
        }
    }

    // Called by animator
    private void DoBlessedUltimate()
    {
        currentBasicEnemies = FindObjectsOfType<BasicEnemy>();

        Camera mainCamera = Camera.main;

        // Light mode ultimate
        foreach (BasicEnemy enemy in currentBasicEnemies)
        {
            if (IsEnemyOnScreen(mainCamera, enemy.transform.position))
            {
                enemy.TryKnockup(knockUpEffect);
            }
        }
    }

    // Called by animator
    private void DoSinnerUltimate()
    {
        // Ensure dark ultimate effect resets
        darkUltimateRenderer.sprite = null;

        // Trigger dark ultimate animation
        darkUltimateAnimator.SetTrigger("AnimateUltimate");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);

        foreach(Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.TakeDamage(darkModeUltimateDamage);
            }
        }
    }

    private void Attack()
    {
        UpdateMousePosition();

        if (isBlessedMode)
        {
            // Check cooldown for blessed ultimate
            if (lightAttackTimer > 0) return;
            else lightAttackTimer = attackCooldown;
        }
        else
        {
            // Check cooldown for sinner ultimate
            if (darkAttackTimer > 0) return;
            else darkAttackTimer = attackCooldown;
        }

        animator.SetBool("IsLightMode", isBlessedMode);
        animator.SetTrigger("Attack");

        if (isBlessedMode)
        {
            Instantiate(magicSound);
        } else
        {
            Instantiate(meleeSound);
        }
    }

    public void DoBlessedAttack()
    {
        Vector2 direction = (mousePosition - lightProjSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject spawnedProjectile = Instantiate(lightProjectile, lightProjSpawnPoint.position, Quaternion.Euler(0, 0, angle));

        spawnedProjectile.GetComponent<Rigidbody2D>().velocity = direction.normalized * lightProjectileSpeed;
    }

    public void DoSinnerAttack()
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.TakeDamage(darkModeDamage);
        }
    }

    private void TransitionPlayerForm()
    {
        if (swapTimer  > 0) return;

        if (isBlessedMode)
        {
            // Switching to sinner mode
            foreach (Renderer renderer in darkHeroRenderers)
            {
                renderer.gameObject.SetActive(true);
            }
            foreach (Renderer renderer in lightHeroRenderers)
            {
                renderer.gameObject.SetActive(false);
            }

            // Ensure light transition effect resets
            lightTransitionRenderer.sprite = null;

            lightTransitionAnimator.gameObject.SetActive(false);
            darkTransitionAnimator.gameObject.SetActive(true);
            passiveDarkFire.SetActive(true);
            darkTransitionAnimator.SetTrigger("Transition");

            // Toggle to dark mode icons
            iconManager.SetDarkIcons();
        } else
        {
            // Switching to blessed mode
            foreach (Renderer renderer in darkHeroRenderers)
            {
                renderer.gameObject.SetActive(false);
            }
            foreach (Renderer renderer in lightHeroRenderers)
            {
                renderer.gameObject.SetActive(true);
            }

            // Ensure dark transition effect resets
            darkTransitionRenderer.sprite = null;

            lightTransitionAnimator.gameObject.SetActive(true);
            darkTransitionAnimator.gameObject.SetActive(false);
            passiveDarkFire.SetActive(false);
            lightTransitionAnimator.SetTrigger("Transition");

            // Toggle to light mode icons
            iconManager.SetLightIcons();
        }

        isBlessedMode = !isBlessedMode;
        swapTimer = swapCooldown;

        Instantiate(transformationSound);
    }

    bool IsEnemyOnScreen(Camera cam, Vector3 position)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(position);

        // Check if within screen bounds (0-1 in viewport space)
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z > 0; // Ensure it's in front of the camera
    }

    private void OnDrawGizmos()
    {
        // Visualize the ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, 0.2f, 0));

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ultimateRadius);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();

        if (enemyScript)
        {
            enemyList.Add(enemyScript);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();

        if (enemyScript && enemyList.Contains(enemyScript))
        {
            enemyList.Remove(enemyScript);
        }
    }
}