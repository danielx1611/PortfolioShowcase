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
        // Assign references
        gm = FindObjectOfType<GameManager>();
        pg = FindObjectOfType<PlayGrid>();

        // Update troops to place text to the max number of troops to be placed, as no troops have been placed yet
        troopsToPlace.text = "place " + (gm.maxTroopCount) + " more troops";
    }

    public void ShowAvailableLocations()
    {
        // Show all available positions to place the currently selected troop (on player half of the grid)
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

    // Hide all positions on the player half of the grid
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

    // Update troop to place text to accurately represent how many troops still need to be placed
    public void DecreaseTroopsToPlace()
    {
        troopsToPlace.text = "place " + (gm.maxTroopCount - gm.placedTroopCount) + " more troops";
    }

    public void DisableTroopPlacementButtons()
    {
        // Disable the placement buttons when the max number of troops have been placed
        soldierButton.interactable = false;
        tankButton.interactable = false;

        // Make the start button visible once all troops have been placed
        startButton.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        // GameManager no longer in placement phase, current troop not yet selected
        gm.isInPlacementPhase = false;
        gm.currentTroop = null;

        // Hide placement canvas and show the queue
        troopPlacementCanvas.gameObject.SetActive(false);
        troopQueueCanvas.gameObject.SetActive(true);

        // Populate the queue with the troops on screen, and move to the next troop in queue
        PopulateQueueCanvas();
        gm.MoveToNextTroopInQueue();

        // If the troop selected is left team, do player actions.
        // If the troop selected is right team, do AI actions.
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
        // Clear all buttons from board
        gm.currentTroop.HideAllButtons();
        gm.currentTroop.ClearMovementOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
        gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);

        // Requeue the current troop as it is not dead and might get another turn. 
        gm.troopPQ.Enqueue(gm.currentTroop, gm.currentPQMin + gm.currentTroop.waitTime);

        // Update queue on screen to reflect new turn order and move to the next troop in queue
        PopulateQueueCanvas();
        gm.MoveToNextTroopInQueue();

        // If the troop selected is left team, do player actions.
        // If the troop selected is right team, do AI actions.
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
        // Get references to the priority queue and the root node to begin insertions
        PriorityQueue<Troop> pq = gm.troopPQ;
        PriorityQueue<Troop>.Node currNode = pq.root;

        
        for (int i = 0; i < gm.maxTroopCount*2; i++)
        {
            TroopMovementQueueButton currIcon = queueUI.array[i];
            if (currNode != null)
            {
                // Update current queue icon's relevant information about the troop in the queue it represents
                currIcon.teamText.text = currNode.element.team == "left" ? "you" : "enemy";
                currIcon.troopText.text = currNode.element.troopName;
                currIcon.healthText.text = "" + currNode.element.health;
                currIcon.troop = currNode.element;

                // Move on to next element in priority queue
                currNode = currNode.next;
            } else
            {
                // There are less troops alive, this queue button is no longer needed and can be hidden
                currIcon.gameObject.SetActive(false);
            }
        }
    }

    // Setter methods for deciding which type of troop to place in placement phase
    public void SetGMTroopSoldier() { gm.currentTroop = soldierTroop; }

    public void SetGMTroopTank() { gm.currentTroop = tankTroop; }
}
