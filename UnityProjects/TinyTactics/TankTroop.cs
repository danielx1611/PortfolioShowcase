using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankTroop : Troop
{
    // Uses basic troop commands, all commands refer to base behavior

    public override void OnMouseDown()
    {
        base.OnMouseDown();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void ShowMovementOptions(int maxDistance, int x, int y)
    {
        base.ShowMovementOptions(maxDistance, x, y);
    }

    public override void ClearMovementOptions(int maxDistance, int x, int y)
    {
        base.ClearMovementOptions(maxDistance, x, y);
    }

    public override void ShowAttackOptions(int maxDistance, int x, int y)
    {
        base.ShowAttackOptions(maxDistance, x, y);
    }

    public override void ClearAttackOptions(int maxDistance, int x, int y)
    {
        base.ClearAttackOptions(maxDistance, x, y);
    }

    public override void RunAttackButton()
    {
        base.RunAttackButton();
    }

    public override void RunCancelButton()
    {
        base.RunCancelButton();
    }

    public override void RunMoveButton()
    {
        base.RunMoveButton();
    }

    public override void DisableMoveButton()
    {
        base.DisableMoveButton();
    }

    public override void DisableAttackButton()
    {
        base.DisableAttackButton();
    }

    public override void HideAllButtons()
    {
        base.HideAllButtons();
    }

    public override void ShowAllButtons()
    {
        base.ShowAllButtons();
    }

    public override void EnableAllButtons()
    {
        base.EnableAllButtons();
    }

    public override void setRightwardOrientation()
    {
        base.setRightwardOrientation();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void RunAIDecision()
    {
        base.RunAIDecision();
    }

    // AI Sequence commented out, same as the base AI Sequence, left here for ease of modification

    /*IEnumerator runAISequence()
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
        } else
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
    }*/
}
