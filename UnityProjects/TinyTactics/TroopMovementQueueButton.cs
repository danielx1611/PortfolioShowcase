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

    // Spawn blue ring on grid indicating which troop this queue button represents
    public void ShowLocationIcon()
    {
        spawnedLocationIndicator = Instantiate(locationIndicator, troop.transform.position, troop.transform.rotation, troop.transform);
    }

    // Hide blue ring on grid
    public void HideLocationIcon()
    {
        Destroy(spawnedLocationIndicator);
    }

}
