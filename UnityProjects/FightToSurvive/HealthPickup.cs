using TMPro;
using UnityEngine;

namespace cowsins
{
    public class HealthPickup : Pickeable
    {

        [SerializeField] private Sprite healthIcon;

        [SerializeField] private GameObject healthGraphics;

        public int cost;

        public GameObject healthDropWaitTime;

        public TextMeshProUGUI costDisplay;

        PlayerStats stats;
        public float healAmount;

        public override void Start()
        {
            // Initialize health pickup variables
            stats = FindObjectOfType<PlayerStats>();
            image.sprite = healthIcon;

            // Show price to buy health pickup based on cost assigned to pickup
            costDisplay.text = "$ " + cost.ToString();

            // Remove old parent graphic and add new health pickup graphic
            Destroy(graphics.transform.GetChild(0).gameObject);
            Instantiate(healthGraphics, graphics);

            // Do the normal pickeable start methods
            base.Start();
        }

        // If player can buy pickup, heal the player and subtract the gold cost from
        // the player's gold count. Then destroy the pickup after creating a respawner.
        public override void Interact()
        {
            if (cost > 0)
            {
                if (GM.instance.gold >= cost)
                {
                    stats.Heal(healAmount);
                    GM.instance.gold -= cost;
                    Instantiate(healthDropWaitTime, transform.position, transform.rotation);
                    Destroy(this.gameObject);
                }
            } else if (cost == 0)
            {
                // The pickup is free, just heal the player and continue
                stats.Heal(healAmount);
            }
        }
    }
}
