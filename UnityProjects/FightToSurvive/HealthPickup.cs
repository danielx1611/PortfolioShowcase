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
            stats = FindObjectOfType<PlayerStats>();
            image.sprite = healthIcon;
            costDisplay.text = "$ " + cost.ToString();
            Destroy(graphics.transform.GetChild(0).gameObject);
            Instantiate(healthGraphics, graphics);
            base.Start();
        }
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
                stats.Heal(healAmount);
            }
        }
    }
}