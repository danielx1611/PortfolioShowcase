using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPosition : MonoBehaviour
{

    public List<TroopPosition> neighboringTiles = new List<TroopPosition>();

    bool isVisited;
    public bool isOccupied;
    Troop currentTroop;
    public int posX;
    public int posY;

    GameManager gm;
    PlayGrid pg;
    TroopPlacement tp;

    // Start is called before the first frame update
    void Start()
    {
        // Assign references
        gm = FindObjectOfType<GameManager>();
        pg = FindObjectOfType<PlayGrid>();
        tp = FindObjectOfType<TroopPlacement>();

        // Reset troop position data and hide from view
        isVisited = false;
        gameObject.SetActive(false);
    }

    public void OnMouseDown()
    {
        // Placement phase mechanics
        if (gm.isInPlacementPhase)
        {
            // Place troop on selected troop position and update 
            TroopPosition tPos = pg.grid[posX].row[posY];
            tPos.occupy();
            Troop spawned = Instantiate(gm.currentTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
            tPos.setCurrentTroop(spawned);

            // Add newly spawned troop to left troop list
            gm.leftTroops[gm.placedTroopCount] = spawned;

            // Update spawned troop data to match placed position, then make it face rightwards
            tPos.getCurrentTroop().posX = posX;
            tPos.getCurrentTroop().posY = posY;
            tPos.getCurrentTroop().setRightwardOrientation();

            // Hide all of the placement locations
            tp.HideAllLocations();

            // Increment the number of placed troops and decrease the amount of troops left to place
            gm.placedTroopCount++;
            tp.DecreaseTroopsToPlace();

            // If all troops have been placed, stop placement sequence and begin populating the queue
            if (gm.placedTroopCount == 6)
            {
                tp.DisableTroopPlacementButtons();
                gm.PopulateQueue();
            }
        } else // No longer in placement phase
        {
            if (!isOccupied)
            {
                // Set current troop to the troop controlled by the game manager, and clear any movement or attack circles nearby
                currentTroop = gm.currentTroop;
                currentTroop.ClearMovementOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);
                currentTroop.ClearAttackOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);

                // Set the troop's current troop position to be empty and unoccupy the area
                pg.grid[currentTroop.posX].row[currentTroop.posY].setCurrentTroop(null);
                pg.grid[currentTroop.posX].row[currentTroop.posY].unoccupy();

                // Update the troop's position to this troop position's location
                currentTroop.posX = posX;
                currentTroop.posY = posY;

                // Phyiscally move the troop game object to this location and occupy this troop position
                currentTroop.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                occupy();

                // Disable move button so that player cannot keep moving on this turn
                currentTroop.DisableMoveButton();
            }
            else
            {
                // Clear attack options and attack the target
                gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
                currentTroop.TakeDamage(gm.currentTroop.damage);

                // Disable the attack button so the troop can no longer attack for this turn
                gm.currentTroop.DisableAttackButton();
            }
        }
        
    }

    // Getter and setter methods for various troop position attributes
    public void addNeighbor(TroopPosition neighbor) { neighboringTiles.Add(neighbor); }
    public List<TroopPosition> getNeighbors() { return neighboringTiles; }
    public void visit() { isVisited = true; }
    public void unvisit() { isVisited = false; }
    public bool checkVisited() { return isVisited; }
    public void occupy() { isOccupied = true; }
    public void unoccupy() { isOccupied = false; }
    public bool checkOccupied() { return isOccupied; }
    public Troop getCurrentTroop() { return currentTroop; }
    public void setCurrentTroop(Troop troop) { currentTroop = troop; }

    public void runAIMovement()
    {
        if (!isOccupied)
        {
             // Set current troop to the troop controlled by the game manager, and clear any movement circles nearby
            currentTroop = gm.currentTroop;
            currentTroop.ClearMovementOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);

            // Set the troop's current troop position to be empty and unoccupy the area
            pg.grid[currentTroop.posX].row[currentTroop.posY].setCurrentTroop(null);
            pg.grid[currentTroop.posX].row[currentTroop.posY].unoccupy();

            // Update the troop's position to this troop position's location
            currentTroop.posX = posX;
            currentTroop.posY = posY;

            // Phyiscally move the troop game object to this location and occupy this troop position
            currentTroop.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            occupy();
        }
        else
        {
            // Clear attack options and attack the target
            gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
            currentTroop.TakeDamage(gm.currentTroop.damage);
        }
    }
}
