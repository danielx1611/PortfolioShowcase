using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using cowsins;
using TMPro;

public class Zombie : EnemyHealth
{

    [HideInInspector] public Transform target;

    private NavMeshAgent agent;
    public bool isTargetRangeZombie;

    private Animator anim;
    public Collider[] initialColliders;

    public Collider[] bodyPartsColliders;
    public Rigidbody[] bodyPartsRBs;

    public float attackDistance;

    Destroyer destroyer;

    private PlayerStats playerStats;
    public float damage;

    public GameObject[] bodies;
    public GameObject[] legs;
    public GameObject[] heads;

    public GameObject deathEffect;
    public Transform head;

    public float minSpeed;
    public float maxSpeed;

    public bool isBigZombie;

    public Transform[] spawnedZombiesPositions;
    public GameObject zombie;

    public int minGold;
    public int maxGold;
    private int gold;

    public GameObject goldPopUp;
    public float yOffset = 0;

    public int scoreValue;

    private RandomizeSound sound;

    public GameObject attackSound;
    public GameObject footstepSound;

    GameObject footstepInstantiated;

    public GameObject targetZombieSpawner;

    bool playAttackSoundOnce;
    
    WaveManager wm;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        wm = FindObjectOfType<WaveManager>();

        sound = GetComponent<RandomizeSound>();

        gold = Random.Range(minGold, maxGold);

        anim = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        destroyer = GetComponent<Destroyer>();

        if (!isTargetRangeZombie)
        {
            agent = GetComponent<NavMeshAgent>();
            float speed = Random.Range(minSpeed, maxSpeed);
            agent.speed = speed;
        }

        playerStats = FindAnyObjectByType<PlayerStats>();        

        foreach (Collider c in bodyPartsColliders)
        {
            c.enabled = false;
        }

        foreach (Rigidbody r in bodyPartsRBs)
        {
            r.isKinematic = true;
        }

        int randBody = Random.Range(0, bodies.Length);
        int randLegs = Random.Range(0, legs.Length);
        int randHeads = Random.Range(0, heads.Length);

        bodies[randBody].SetActive(true);
        legs[randLegs].SetActive(true);
        heads[randHeads].SetActive(true);

        if (!isTargetRangeZombie)
        {
            footstepInstantiated = Instantiate(footstepSound, transform.position, Quaternion.identity, gameObject.transform);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!isTargetRangeZombie)
        {
            if (target != null && !isDead && !anim.GetBool("Attack"))
            {
                if (Vector3.Distance(transform.position, target.position) < attackDistance && !playAttackSoundOnce)
                {
                    Instantiate(attackSound);
                    anim.SetTrigger("Attack");
                    playAttackSoundOnce = true;
                }
                else
                {
                    agent.SetDestination(target.position);
                }
            }
        }
    }
    
    public override void Die()
    {
        base.Die();

        if (!isTargetRangeZombie)
        {
            Destroy(footstepInstantiated.gameObject);
            if (isBigZombie)
            {
                wm.aliveZombiesCount += 4;
            }
            wm.aliveZombiesCount--;
        } else
        {
            Instantiate(targetZombieSpawner, transform.position, transform.rotation);
        }

        if (isBigZombie)
        {
            for (int i = 0; i < spawnedZombiesPositions.Length; i++)
            {
                Instantiate(zombie, spawnedZombiesPositions[i].position, Quaternion.identity);
            }
        }

        Instantiate(deathEffect, head.position, Quaternion.identity);
        
        if (!isTargetRangeZombie)
        {
            GM.instance.gold += gold;

            GM.instance.totalScore += scoreValue * GM.instance.scoreMultiplier;
            GM.instance.scoreDisplay.text = GM.instance.totalScore.ToString();
            GM.instance.cooldown = GM.instance.timeLimit;
            GM.instance.scoreMultiplier++;
            GM.instance.scoreMultiplierDisplay.text = "X" + GM.instance.scoreMultiplier.ToString();

            GameObject instance = Instantiate(goldPopUp, transform.position + Vector3.up * yOffset, transform.rotation);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "+$" + gold.ToString();
        }

        destroyer.enabled = true;
        anim.enabled = false;
        if (!isTargetRangeZombie)
        {
            agent.enabled = false;
        }
        foreach(Collider c in initialColliders)
        {
            c.enabled = false;
        }

        foreach(Collider c in bodyPartsColliders)
        {
            c.enabled = true;
        }

        foreach (Rigidbody r in bodyPartsRBs)
        {
            r.isKinematic = false;
        }

        
    }

    public void DamagePlayer()
    {

        playAttackSoundOnce = false;

        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            CamShake.instance.ShootShake(.15f);
            playerStats.Damage(damage);
        }
    }

    public override void Damage(float _damage)
    {
        sound.PlaySound();

        base.Damage(_damage);

        anim.SetTrigger("damage");
    }
}
