using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private GameObject DoorToClose;

    private Rigidbody2D rb;
    private BossEnemy boss;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            boss = FindObjectOfType<BossEnemy>();
            boss.InitiateBossFight();
            DoorToClose.SetActive(true);

            Destroy(gameObject);
        }
    }
}
