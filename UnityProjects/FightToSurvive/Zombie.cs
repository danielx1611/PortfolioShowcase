using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using cowsins;
using TMPro;

public class Zombie : EnemyHealth
{

    // Get reference to target transform
    [HideInInspector] public Transform target;

    // Setup NavMeshAgent and boolean to control zombie AI
    private NavMeshAgent agent;
    public bool isTargetRangeZombie;

    // Zombie actor elements
    private Animator anim;
    public Collider[] initialColliders;
    public Collider[] bodyPartsColliders;
    public Rigidbody[] bodyPartsRBs;

    // Values to control zombie attack
    public float attackDistance;
    public float damage;

    // Destroyer object to remove zombie body after death
    Destroyer destroyer;

    // Reference to PlayerStats script
    private PlayerStats playerStats;
    
    // List references to all of the variations of the zombie's body, leg and head parts
    public GameObject[] bodies;
    public GameObject[] legs;
    public GameObject[] heads;

    // Particle effect to be spawned on zombie death
    public GameObject deathEffect;

    // Reference to transform of head of zombie
    public Transform head;

    // Values to control speed of zombie
    public float minSpeed;
    public float maxSpeed;

    // Indicate if zombie is a large variant
    public bool isBigZombie;

    // References to zombie spawns and small zombie instance for big zombie to spawn on death
    public Transform[] spawnedZombiesPositions;
    public GameObject zombie;

    // Variables to determine what range of gold the zombie is worth
    public int minGold;
    public int maxGold;
    private int gold;

    // Reference to gold popup UI that spawns on zombie death and how far off the yOffset above the zombie will be when it spawns
    public GameObject goldPopUp;
    public float yOffset = 0;

    // Reference to how many points the zombie is worth when killed
    public int scoreValue;

    // Reference to script to randomize zombie sounds
    private RandomizeSound sound;

    // References to zombie attack and footstep sounds
    public GameObject attackSound;
    public GameObject footstepSound;

    // Reference to the spawned footstep sound object
    GameObject footstepInstantiated;

    // Reference to the zombie spawner
    public GameObject targetZombieSpawner;

    // Control if the attack sound should only be played once
    bool playAttackSoundOnce;

    // Reference to WaveManageer object
    WaveManager wm;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // Populate references
        wm = FindObjectOfType<WaveManager>();
        sound = GetComponent<RandomizeSound>();
        gold = Random.Range(minGold, maxGold);
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        destroyer = GetComponent<Destroyer>();
        playerStats = FindAnyObjectByType<PlayerStats>();     

        // If this is a moving zombie, setup the AI movement with NavMeshAgent
        if (!isTargetRangeZombie)
        {
            agent = GetComponent<NavMeshAgent>();
            
            // Initialize and assign speed to random variable speed
            float speed = Random.Range(minSpeed, maxSpeed);
            agent.speed = speed;
        }

        // Disable colliders temporarily
        foreach (Collider c in bodyPartsColliders)
        {
            c.enabled = false;
        }

        // Temporarily make zombie body kinematic
        foreach (Rigidbody r in bodyPartsRBs)
        {
            r.isKinematic = true;
        }

        // Calculate random zombie appearance
        int randBody = Random.Range(0, bodies.Length);
        int randLegs = Random.Range(0, legs.Length);
        int randHeads = Random.Range(0, heads.Length);

        // Make the appropriate body parts visible
        bodies[randBody].SetActive(true);
        legs[randLegs].SetActive(true);
        heads[randHeads].SetActive(true);

        // If it is a moving zombie, spawn a footstep sound maker
        if (!isTargetRangeZombie)
        {
            footstepInstantiated = Instantiate(footstepSound, transform.position, Quaternion.identity, gameObject.transform);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        // If it is a moving zombie, and it has a valid window to attack, then attack the player
        if (!isTargetRangeZombie)
        {
            if (target != null && !isDead && !anim.GetBool("Attack"))
            {
                // If in attack range, attack
                if (Vector3.Distance(transform.position, target.position) < attackDistance && !playAttackSoundOnce)
                {
                    Instantiate(attackSound);
                    anim.SetTrigger("Attack");
                    playAttackSoundOnce = true;
                }
                else
                {
                    // Move towards player target
                    agent.SetDestination(target.position);
                }
            }
        }
    }
    
    public override void Die()
    {
        base.Die();

        // If it is a moving zombie, destroy footstep sound maker
        if (!isTargetRangeZombie)
        {
            Destroy(footstepInstantiated.gameObject);
            // If it is a big zombie, spawn 4 child zombies and update alive zombies count
            if (isBigZombie)
            {
                wm.aliveZombiesCount += 4;
            }
            wm.aliveZombiesCount--;
        } else
        {
            // Target range zombie, spawn a new one
            Instantiate(targetZombieSpawner, transform.position, transform.rotation);
        }

        // If it is a big zombie, spawn its four children at its spawned zombie positions
        if (isBigZombie)
        {
            for (int i = 0; i < spawnedZombiesPositions.Length; i++)
            {
                Instantiate(zombie, spawnedZombiesPositions[i].position, Quaternion.identity);
            }
        }

        // Instantiate death effect at zombie's head position
        Instantiate(deathEffect, head.position, Quaternion.identity);

        // If it is a real zombie, update the relevant GameManager data
        if (!isTargetRangeZombie)
        {
            // Update player gold count
            GM.instance.gold += gold;

            // Update player total score
            GM.instance.totalScore += scoreValue * GM.instance.scoreMultiplier;
            GM.instance.scoreDisplay.text = GM.instance.totalScore.ToString();
            GM.instance.cooldown = GM.instance.timeLimit;

            // Update score multiplier
            GM.instance.scoreMultiplier++;
            GM.instance.scoreMultiplierDisplay.text = "X" + GM.instance.scoreMultiplier.ToString();

            // Instantiate popup showing how much gold the player received for killing the zombie
            GameObject instance = Instantiate(goldPopUp, transform.position + Vector3.up * yOffset, transform.rotation);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "+$" + gold.ToString();
        }

        // Enable destroyer to remove zombie and stop animating the zombie
        destroyer.enabled = true;
        anim.enabled = false;

        // If it is a real zombie, disable the AI navmesh to prevent errors
        if (!isTargetRangeZombie)
        {
            agent.enabled = false;
        }

        // Disable colliders to stop player from running into or shooting corpse
        foreach(Collider c in initialColliders)
        {
            c.enabled = false;
        }

        // Enable collider to prevent zombie corpse from falling through world
        foreach(Collider c in bodyPartsColliders)
        {
            c.enabled = true;
        }

        // Make body kinematic for ragdoll purposes
        foreach (Rigidbody r in bodyPartsRBs)
        {
            r.isKinematic = false;
        }

    }

    public void DamagePlayer()
    {

        // Update play attack sound to enable zombie to attack again (prevents zombie from making attack audio multiple times)
        playAttackSoundOnce = false;

        // If the zombie is still within attacking distance at current point in animation, damage player and shake camera
        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            CamShake.instance.ShootShake(.15f);
            playerStats.Damage(damage);
        }
    }

    public override void Damage(float _damage)
    {
        // Play zombie taking damage sound
        sound.PlaySound();

        // Zombie takes damage
        base.Damage(_damage);

        // Play animation indicating zombie taking damage
        anim.SetTrigger("damage");
    }
}
