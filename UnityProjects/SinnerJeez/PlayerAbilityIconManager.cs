using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilityIconManager : MonoBehaviour
{
    [SerializeField] private GameObject[] lightIcons;
    [SerializeField] private GameObject[] darkIcons;

    [SerializeField] private Image swapCooldownIcon;
    [SerializeField] private Image attackCooldownIcon;
    [SerializeField] private Image abilityCooldownIcon;
    [SerializeField] private Image ultimateCooldownIcon;

    public void SetLightIcons()
    {
        foreach (GameObject icon in lightIcons)
        {
            icon.SetActive(true);
        }

        foreach (GameObject icon in darkIcons)
        {
            icon.SetActive(false);
        }
    }

    public void SetDarkIcons()
    {
        foreach (GameObject icon in lightIcons)
        {
            icon.SetActive(false);
        }

        foreach (GameObject icon in darkIcons)
        {
            icon.SetActive(true);
        }
    }

    public void UpdateCooldownIcons(float swapFillAmount, float attackFillAmount, float abilityFillAmount, float ultimateFillAmount)
    {
        swapCooldownIcon.fillAmount = swapFillAmount;
        attackCooldownIcon.fillAmount = attackFillAmount;
        abilityCooldownIcon.fillAmount = abilityFillAmount;
        ultimateCooldownIcon.fillAmount = ultimateFillAmount;
    }
}
