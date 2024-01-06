using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopPlacement : MonoBehaviour
{

    [SerializeField] SoldierTroop soldierTroop;
    [SerializeField] TankTroop tankTroop;

    [SerializeField] Canvas troopPlacementCanvas;
    [SerializeField] Button soldierButton;
    [SerializeField] Button tankButton;
    [SerializeField] Button startButton;

    [SerializeField] TMP_Text troopsToPlace;

    [SerializeField] Canvas troopQueueCanvas;
    [SerializeField] QueueIconArray queueUI;

    [SerializeField] Button endTurnButton;

    GameManager gm;
    PlayGrid pg;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        pg = FindObjectOfType<PlayGrid>();
        troopsToPlace.text = "place " + (gm.maxTroopCount) + " more troops";
    }

    public void ShowAvailableLocations()
    {
        for (int i = 0; i < pg.grid.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (pg.grid[i].row[j] != null && !pg.grid[i].row[j].checkOccupied())
                {
                    pg.grid[i].row[j].gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideAllLocations()
    {
        for (int i = 0; i < pg.grid.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (pg.grid[i].row[j] != null)
                {
                    pg.grid[i].row[j].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DecreaseTroopsToPlace()
    {
        troopsToPlace.text = "place " + (gm.maxTroopCount - gm.placedTroopCount) + " more troops";
    }

    public void DisableTroopPlacementButtons()
    {
        soldierButton.interactable = false;
        tankButton.interactable = false;
        startButton.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        gm.isInPlacementPhase = false;
        gm.currentTroop = null;
        troopPlacementCanvas.gameObject.SetActive(false);
        troopQueueCanvas.gameObject.SetActive(true);
        PopulateQueueCanvas();
        gm.MoveToNextTroopInQueue();
        if (gm.currentTroop.team == "left")
        {
            endTurnButton.gameObject.SetActive(true);
            gm.currentTroop.EnableAllButtons();
            gm.currentTroop.ShowAllButtons();
        }
        else if (gm.currentTroop.team == "right")
        {
            endTurnButton.gameObject.SetActive(false);
            gm.currentTroop.RunAIDecision();
        }
    }

    public void EndTurn()
    {
        gm.currentTroop.HideAllButtons();
        gm.currentTroop.ClearMovementOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
        gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
        gm.troopPQ.Enqueue(gm.currentTroop, gm.currentPQMin + gm.currentTroop.waitTime);
        PopulateQueueCanvas();
        gm.MoveToNextTroopInQueue();
        if (gm.currentTroop.team == "left")
        {
            endTurnButton.gameObject.SetActive(true);
            gm.currentTroop.EnableAllButtons();
            gm.currentTroop.ShowAllButtons();
        }
        else if (gm.currentTroop.team == "right")
        {
            endTurnButton.gameObject.SetActive(false);
            gm.currentTroop.RunAIDecision();
        }
    }

    void PopulateQueueCanvas()
    {
        PriorityQueue<Troop> pq = gm.troopPQ;
        PriorityQueue<Troop>.Node currNode = pq.root;
        for (int i = 0; i < gm.maxTroopCount*2; i++)
        {
            TroopMovementQueueButton currIcon = queueUI.array[i];
            if (currNode != null)
            {
                currIcon.teamText.text = currNode.element.team == "left" ? "you" : "enemy";
                currIcon.troopText.text = currNode.element.troopName;
                currIcon.healthText.text = "" + currNode.element.health;
                currIcon.troop = currNode.element;
                currNode = currNode.next;
            } else
            {
                currIcon.gameObject.SetActive(false);
            }
        }
    }

    public void SetGMTroopSoldier() { gm.currentTroop = soldierTroop; }

    public void SetGMTroopTank() { gm.currentTroop = tankTroop; }
}
