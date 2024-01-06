using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopMovementQueueButton : MonoBehaviour
{
    public TMP_Text teamText;
    public TMP_Text troopText;
    public TMP_Text healthText;

    public Troop troop;

    public GameObject locationIndicator;

    GameObject spawnedLocationIndicator;

    public void ShowLocationIcon()
    {
        spawnedLocationIndicator = Instantiate(locationIndicator, troop.transform.position, troop.transform.rotation, troop.transform);
    }

    public void HideLocationIcon()
    {
        Destroy(spawnedLocationIndicator);
    }

}
