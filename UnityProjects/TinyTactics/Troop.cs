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
        // Enable turn control options for current troop if it is a player controlled troop, and it is this troops turn
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
        pg = FindObjectOfType<PlayGrid>();
        gm = FindObjectOfType<GameManager>();
        healthBar = GetComponentInChildren<Slider>();
        healthBar.maxValue = health;
        healthBar.value = health;

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (team == "left")
            {
                GameObject spawned = Instantiate(troopType[gm.leftBossSelectIndex], unitSlots[i].transform.position, unitSlots[i].transform.rotation, unitSlots[i].transform);
                spawned.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            } else if (team == "right")
            {
                GameObject spawned = Instantiate(troopType[gm.rightBossSelectIndex], unitSlots[i].transform.position, unitSlots[i].transform.rotation, unitSlots[i].transform);
                spawned.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        }

        pg = FindObjectOfType<PlayGrid>();
        gm = FindObjectOfType<GameManager>();
        tp = FindObjectOfType<TroopPlacement>();
    }

    public virtual void ShowMovementOptions(int maxDistance, int x, int y)
    {
        TroopPosition curPos = pg.grid[x].row[y];
        curPos.visit();
        if (maxDistance >= 1)
        {
            List<TroopPosition> neighborList = curPos.getNeighbors();
            for (int i = 0; i < neighborList.Count; i++)
            {
                if (!neighborList[i].checkVisited() && !neighborList[i].checkOccupied())
                {
                    neighborList[i].gameObject.SetActive(true);
                    ShowMovementOptions(maxDistance - 1, neighborList[i].posX, neighborList[i].posY);
                }
                else if (team == "right" && neighborList[i].checkOccupied() && neighborList[i].getCurrentTroop().team != team && !neighborList[i].checkVisited() && maxDistance < maxTravelDistance)
                {
                    idealAIMovement = curPos;
                    idealAIAttackLocation = neighborList[i];
                }
            }
        }
        if (team == "right")
        {
            if (maxDistance == 0)
            {
                if (idealAIMovement == null)
                {
                    idealAIMovement = curPos;
                }

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

    public virtual void ClearMovementOptions(int maxDistance, int x, int y)
    {
        TroopPosition curPos = pg.grid[x].row[y];
        if (maxDistance >= 1)
        {
            List<TroopPosition> neighborList = curPos.getNeighbors();
            for (int i = 0; i < neighborList.Count; i++)
            {
                if (neighborList[i].checkVisited() && !neighborList[i].checkOccupied())
                {
                    ClearMovementOptions(maxDistance - 1, neighborList[i].posX, neighborList[i].posY);
                    neighborList[i].gameObject.SetActive(false);
                }
            }
        }

        curPos.unvisit();
    }

    public virtual void ShowAttackOptions(int maxDistance, int x, int y)
    {
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

    public virtual void ClearAttackOptions(int maxDistance, int x, int y)
    {
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

    public virtual void RunAttackButton()
    {
        ShowAttackOptions(maxTravelDistance, posX, posY);
    }

    public virtual void RunCancelButton()
    {
        attackButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
        ClearMovementOptions(maxTravelDistance, posX, posY);
        ClearAttackOptions(maxTravelDistance, posX, posY);
    }

    public virtual void RunMoveButton()
    {
        ShowMovementOptions(maxTravelDistance, posX, posY);
    }

    public virtual void DisableMoveButton()
    {
        moveButton.interactable = false;
    }

    public virtual void DisableAttackButton()
    {
        attackButton.interactable = false;
    }

    public virtual void HideAllButtons()
    {
        attackButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
    }

    public virtual void ShowAllButtons()
    {
        attackButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        moveButton.gameObject.SetActive(true);
    }

    public virtual void EnableAllButtons()
    {
        moveButton.interactable = true;
        attackButton.interactable = true;
        cancelButton.interactable = true;
    }

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
            pg.grid[posX].row[posY].unoccupy();
            pg.grid[posX].row[posY].setCurrentTroop(null);
            pg.grid[posX].row[posY].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
            
            gm.RemoveElementFromQueue(this);
            ClearAttackOptions(maxTravelDistance, posX, posY);
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
            Destroy(gameObject);
        }
    }

    public virtual void RunAIDecision()
    {
        StartCoroutine(runAISequence());
    }

    IEnumerator runAISequence()
    {
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
        if (idealAIAttackLocation != null)
        {
            ShowAttackOptions(maxTravelDistance, posX, posY);
            yield return new WaitForSeconds(1f);
            idealAIAttackLocation.runAIMovement();
        }
        else
        {
            ShowMovementOptions(maxTravelDistance, posX, posY);
            if (idealAIMovement != null)
            {
                yield return new WaitForSeconds(1f);
                idealAIMovement.runAIMovement();
                for (int i = 0; i < neighborList.Count; i++)
                {
                    if (neighborList[i].checkOccupied() && !neighborList[i].checkVisited() && neighborList[i].getCurrentTroop().team != team)
                    {
                        idealAIAttackLocation = neighborList[i];
                        break;
                    }
                }
                if (idealAIAttackLocation != null)
                {
                    ShowAttackOptions(maxTravelDistance, posX, posY);
                    yield return new WaitForSeconds(1f);
                    idealAIAttackLocation.runAIMovement();
                }
            }
        }
        idealAIMovement = null;
        idealAIAttackLocation = null;
        tp.EndTurn();
    }
}
