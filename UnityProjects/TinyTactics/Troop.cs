using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Troop : MonoBehaviour
{
    public int health;
    public int damage;
    public int waitTime;
    public int maxTravelDistance;
    public int posX;
    public int posY;
    public string team = "left";

    protected PlayGrid pg;
    protected GameManager gm;
    public Slider healthBar;
    public string troopName;
    public GameObject[] troopType;
    public GameObject[] unitSlots;

    [HideInInspector] public TroopPosition idealAIMovement;
    [HideInInspector] public TroopPosition idealAIAttackLocation;

    [SerializeField] Button attackButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button moveButton;

    TroopPlacement tp;

    public virtual void OnMouseDown()
    {
        // Enable turn control options for current troop when clicked if it is a player controlled troop, and it is this troops turn
        if (team == "left")
        {
            if (this == gm.currentTroop)
            {
                attackButton.gameObject.SetActive(true);
                cancelButton.gameObject.SetActive(true);
                moveButton.gameObject.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Assign references
        pg = FindObjectOfType<PlayGrid>();
        gm = FindObjectOfType<GameManager>();
        tp = FindObjectOfType<TroopPlacement>();
        healthBar = GetComponentInChildren<Slider>();

        // Update healthbar values based on starting health amount
        healthBar.maxValue = health;
        healthBar.value = health;

        // Spawn all left and right troops
        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (team == "left")
            {
                // Spawn correct player troop from the boss's faction and scale the troop appropriately
                GameObject spawned = Instantiate(troopType[gm.leftBossSelectIndex], unitSlots[i].transform.position, unitSlots[i].transform.rotation, unitSlots[i].transform);
                spawned.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            } else if (team == "right")
            {
                // Spawn correct enemy troop from the boss's faction and scale the troop appropriately
                GameObject spawned = Instantiate(troopType[gm.rightBossSelectIndex], unitSlots[i].transform.position, unitSlots[i].transform.rotation, unitSlots[i].transform);
                spawned.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        }
    }

    // Recursive algorithm to highlight all movement options that can be reached within the given maxDistance
    public virtual void ShowMovementOptions(int maxDistance, int x, int y)
    {
        TroopPosition curPos = pg.grid[x].row[y];
        curPos.visit();
        if (maxDistance >= 1)
        {
            // Check each neighbor to see if it is a possible movement route
            List<TroopPosition> neighborList = curPos.getNeighbors();
            for (int i = 0; i < neighborList.Count; i++)
            {
                if (!neighborList[i].checkVisited() && !neighborList[i].checkOccupied())
                {
                    // Neighbor hasn't been visited and is not obstructed, highlight this as a movement option
                    neighborList[i].gameObject.SetActive(true);
                    ShowMovementOptions(maxDistance - 1, neighborList[i].posX, neighborList[i].posY);
                }
                else if (team == "right" && neighborList[i].checkOccupied() && neighborList[i].getCurrentTroop().team != team && !neighborList[i].checkVisited() && maxDistance < maxTravelDistance)
                {
                    // Special instance to check for when calculating AI movement, if it finds a target enemy along its movement path, set this location to be its movement target
                    idealAIMovement = curPos;
                    idealAIAttackLocation = neighborList[i];
                }
            }
        }

        // Calculate a possible attack direction for AI opponents if they have reached the end of their movement searching
        if (team == "right")
        {
            if (maxDistance == 0)
            {
                // Mark a position as the ideal AI movement if no ideal movement has been found yet
                if (idealAIMovement == null)
                {
                    idealAIMovement = curPos;
                }

                // Check all of the neighbors to find an ideal attack location and position to move to
                List<TroopPosition> neighborList = curPos.getNeighbors();
                for (int i = 0; i < neighborList.Count; i++)
                {
                    if (neighborList[i].checkOccupied() && !neighborList[i].checkVisited() && neighborList[i].getCurrentTroop().team != team)
                    {
                        idealAIMovement = curPos;
                        idealAIAttackLocation = neighborList[i];
                        break;
                    }
                }
            }
        }
    }

    // Recursive function to clear the visibility of the movement options from the board to prevent misclicks and confusion
    public virtual void ClearMovementOptions(int maxDistance, int x, int y)
    {
        
        TroopPosition curPos = pg.grid[x].row[y];
        if (maxDistance >= 1)
        {
            // Check all of the neighbors for any visible tiles and hide them
            List<TroopPosition> neighborList = curPos.getNeighbors();
            for (int i = 0; i < neighborList.Count; i++)
            {
                if (neighborList[i].checkVisited() && !neighborList[i].checkOccupied())
                {
                    // First check for further neighbors, then hide this neighbor
                    ClearMovementOptions(maxDistance - 1, neighborList[i].posX, neighborList[i].posY);
                    neighborList[i].gameObject.SetActive(false);
                }
            }
        }

        // Unvisit this location, as it has no longer been visited by the troop movement search
        curPos.unvisit();
    }

    // Check if up down left right positions contain enemies. If they do, show them as an attackable tile
    public virtual void ShowAttackOptions(int maxDistance, int x, int y)
    {
        /*

        For each sequence:
        Get troop position, then check if its up, down, left, or right neighbor is a valid position on the grid.
        If the position is valid, check if it contains an enemy.
        If there is an enemy present, highlight it with a bright red circle

        */
    
        TroopPosition pos;
        if (posX + 1 >= 0 && posX + 1 < pg.grid.Length && pg.grid[posX + 1].row[posY] != null && pg.grid[posX + 1].row[posY].checkOccupied())
        {
            pos = pg.grid[posX + 1].row[posY];
            if (pos.getCurrentTroop().team != team)
            {
                pos.gameObject.SetActive(true);
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = false;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);
            }
        }
        if (posX - 1 >= 0 && posX - 1 < pg.grid.Length && pg.grid[posX - 1].row[posY] != null && pg.grid[posX - 1].row[posY].checkOccupied())
        {
            pos = pg.grid[posX - 1].row[posY];
            if (pos.getCurrentTroop().team != team)
            {
                pos.gameObject.SetActive(true);
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = false;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);
            }
        }
        if (posY + 1 >= 0 && posY + 1 < pg.grid[posX].row.Length && pg.grid[posX].row[posY + 1] != null && pg.grid[posX].row[posY + 1].checkOccupied())
        {
            pos = pg.grid[posX].row[posY + 1];
            if (pos.getCurrentTroop().team != team)
            {
                pos.gameObject.SetActive(true);
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = false;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);
            }
        }
        if (posY - 1 >= 0 && posY - 1 < pg.grid[posX].row.Length && pg.grid[posX].row[posY - 1] != null && pg.grid[posX].row[posY - 1].checkOccupied())
        {
            pos = pg.grid[posX].row[posY - 1];
            if (pos.getCurrentTroop().team != team)
            {
                pos.gameObject.SetActive(true);
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = false;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);
            }
        }
    }

    // Check if up down left right positions contain enemies. If they do, hide the highlighted circle and reset the circle color to white
    public virtual void ClearAttackOptions(int maxDistance, int x, int y)
    {

        /*

        For each sequence:
        Get troop position, then check if its up, down, left, or right neighbor is a valid position on the grid.
        If the position is valid, check if it contains an enemy.
        If there is an enemy present, change the highlighted circle color to white then hide it from view

        */
    
        TroopPosition pos;
        if (posX + 1 >= 0 && posX + 1 < pg.grid.Length && pg.grid[posX + 1].row[posY] != null && pg.grid[posX + 1].row[posY].GetComponent<SpriteRenderer>().color.g == 0)
        {
            pos = pg.grid[posX + 1].row[posY];
            if (pos.getCurrentTroop().team != team)
            {
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = true;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                pos.gameObject.SetActive(false);
            }
        }
        if (posX - 1 >= 0 && posX - 1 < pg.grid.Length && pg.grid[posX - 1].row[posY] != null && pg.grid[posX - 1].row[posY].GetComponent<SpriteRenderer>().color.g == 0)
        {
            pos = pg.grid[posX - 1].row[posY];
            if (pos.getCurrentTroop().team != team)
            {
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = true;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                pos.gameObject.SetActive(false);
            }
        }
        if (posY + 1 >= 0 && posY + 1 < pg.grid[posX].row.Length && pg.grid[posX].row[posY + 1] != null && pg.grid[posX].row[posY + 1].GetComponent<SpriteRenderer>().color.g == 0)
        {
            pos = pg.grid[posX].row[posY + 1];
            if (pos.getCurrentTroop().team != team)
            {
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = true;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                pos.gameObject.SetActive(false);
            }
        }
        if (posY - 1 >= 0 && posY - 1 < pg.grid[posX].row.Length && pg.grid[posX].row[posY - 1] != null && pg.grid[posX].row[posY - 1].GetComponent<SpriteRenderer>().color.g == 0)
        {
            pos = pg.grid[posX].row[posY - 1];
            if (pos.getCurrentTroop().team != team)
            {
                pos.getCurrentTroop().GetComponent<BoxCollider2D>().enabled = true;
                pos.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                pos.gameObject.SetActive(false);
            }
        }
    }

    // Call for OnButtonClick for attack button
    public virtual void RunAttackButton()
    {
        ShowAttackOptions(maxTravelDistance, posX, posY);
    }

    // Call for OnButtonClick for cancel button
    public virtual void RunCancelButton()
    {
        // Hide all troop buttons
        attackButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);

        // Hide any highlighted circles on screen
        ClearMovementOptions(maxTravelDistance, posX, posY);
        ClearAttackOptions(maxTravelDistance, posX, posY);
    }

    // Call for OnButtonClick for move button
    public virtual void RunMoveButton()
    {
        ShowMovementOptions(maxTravelDistance, posX, posY);
    }

    // Disable option to click move button (i.e. already moved this turn)
    public virtual void DisableMoveButton()
    {
        moveButton.interactable = false;
    }

    // Disable option to click attack button (i.e. already attacked this turn)
    public virtual void DisableAttackButton()
    {
        attackButton.interactable = false;
    }

    // Hide all troop buttons
    public virtual void HideAllButtons()
    {
        attackButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
    }

    // Show all troop buttons
    public virtual void ShowAllButtons()
    {
        attackButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        moveButton.gameObject.SetActive(true);
    }

    // Reenable all troop buttons
    public virtual void EnableAllButtons()
    {
        moveButton.interactable = true;
        attackButton.interactable = true;
        cancelButton.interactable = true;
    }

    // Flip direction of sprite for rightward facing opponents
    public virtual void setRightwardOrientation()
    {
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            // Clear grid position upon death
            pg.grid[posX].row[posY].unoccupy();
            pg.grid[posX].row[posY].setCurrentTroop(null);
            pg.grid[posX].row[posY].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);

            // Remove troop from queue, it is dead and cannot attack
            gm.RemoveElementFromQueue(this);

            // Clear visual attack options from screen
            ClearAttackOptions(maxTravelDistance, posX, posY);

            // Remove troop from respective list. If one team runs out of troops, end game
            if (team == "left")
            {
                gm.friendlyTroopCount--;
                if (gm.friendlyTroopCount == 0)
                {
                    gm.gameWon = false;
                    gm.EndGame();
                }
            } else if (team == "right")
            {
                gm.enemyTroopCount--;
                if (gm.enemyTroopCount == 0)
                {
                    gm.gameWon = true;
                    gm.EndGame();
                }
            }

            // Destroy this object as it has been killed
            Destroy(gameObject);
        }
    }

    public virtual void RunAIDecision()
    {
        StartCoroutine(runAISequence());
    }

    IEnumerator runAISequence()
    {
        // Get all neighbors of AI troop to see if it can attack any of its immediate neighbors
        TroopPosition curPos = pg.grid[posX].row[posY];
        List<TroopPosition> neighborList = curPos.getNeighbors();
        for (int i = 0; i < neighborList.Count; i++)
        {
            if (neighborList[i].checkOccupied() && !neighborList[i].checkVisited() && neighborList[i].getCurrentTroop().team != team)
            {
                idealAIAttackLocation = neighborList[i];
                break;
            }
        }
        // AI has an immediate neighbor enemy. Attack it
        if (idealAIAttackLocation != null)
        {
            // Slow down AI movement to show player AI's decision making process
            ShowAttackOptions(maxTravelDistance, posX, posY);
            yield return new WaitForSeconds(1f);

            // Run attack code
            idealAIAttackLocation.runAIMovement();
        }
        else
        {
            // Find ideal AI movement location
            ShowMovementOptions(maxTravelDistance, posX, posY);
            if (idealAIMovement != null)
            {
                // Slow down AI movement to show player AI's decision making process
                yield return new WaitForSeconds(1f);
                idealAIMovement.runAIMovement();

                // Check one last time for any neighbors that could be attacked before ending the turn
                for (int i = 0; i < neighborList.Count; i++)
                {
                    if (neighborList[i].checkOccupied() && !neighborList[i].checkVisited() && neighborList[i].getCurrentTroop().team != team)
                    {
                        idealAIAttackLocation = neighborList[i];
                        break;
                    }
                }

                // If there is something to attack, attack it
                if (idealAIAttackLocation != null)
                {
                    // Slow down AI movement to show player AI's decision making process
                    ShowAttackOptions(maxTravelDistance, posX, posY);
                    yield return new WaitForSeconds(1f);

                    // Run attack code
                    idealAIAttackLocation.runAIMovement();
                }
            }
        }

        // Reset this AI troop's target position data and end its turn
        idealAIMovement = null;
        idealAIAttackLocation = null;
        tp.EndTurn();
    }
}
