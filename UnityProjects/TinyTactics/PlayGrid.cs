using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGrid : MonoBehaviour
{

    public GridRow[] grid;
    [SerializeField] SoldierTroop soldierTroop;
    [SerializeField] TankTroop tankTroop;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        // Assign references
        gm = FindObjectOfType<GameManager>();

        // Spawn left and right bosses on screen
        gm.SpawnLeftBoss();
        gm.SpawnRightBoss();

        // Find grid positions and populate their neighbor lists to form a graph
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].row.Length; j++)
            {
                if (grid[i].row[j] != null)
                {
                    FindNeighbors(grid[i].row[j], i, j);
                    grid[i].row[j].posX = i;
                    grid[i].row[j].posY = j;
                }
            }
        }

        // Spawn enemies on grid after grid is built
        SpawnEnemies();
    }

    // Check up, down, left, and right tiles to see if they are possible neighbors of the current graph node
    void FindNeighbors(TroopPosition troop, int iPos, int jPos)
    {
        if (jPos - 1 >= 0 && jPos - 1 < grid[iPos].row.Length && grid[iPos].row[jPos - 1] != null)
            troop.addNeighbor(grid[iPos].row[jPos - 1]);
        if (iPos + 1 >= 0 && iPos + 1 < grid.Length && grid[iPos + 1].row[jPos] != null)
            troop.addNeighbor(grid[iPos + 1].row[jPos]);
        if (iPos - 1 >= 0 && iPos - 1 < grid.Length && grid[iPos - 1].row[jPos] != null)
            troop.addNeighbor(grid[iPos - 1].row[jPos]);
        if (jPos + 1 >= 0 && jPos + 1 < grid[iPos].row.Length && grid[iPos].row[jPos + 1] != null)
            troop.addNeighbor(grid[iPos].row[jPos + 1]);
    }

    void SpawnEnemies()
    {
        /*

        For each spawn sequence:
        Store the position the troop will be spawned at, then spawn the troop there.
        Store the spawned troop in the grid tile, and set the grid tile team to "right" (enemy team)
        Tell the spawned troop what position it is at, then add the troop to the rightTroops list (enemy troop list)

        */
    
        TroopPosition tPos = grid[0].row[6];
        tPos.occupy();
        Troop spawned = Instantiate(soldierTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 0;
        tPos.getCurrentTroop().posY = 6;
        gm.rightTroops[0] = spawned;

        tPos = grid[1].row[8];
        tPos.occupy();
        spawned = Instantiate(tankTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 1;
        tPos.getCurrentTroop().posY = 8;
        gm.rightTroops[1] = spawned;

        tPos = grid[2].row[7];
        tPos.occupy();
        spawned = Instantiate(tankTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 2;
        tPos.getCurrentTroop().posY = 7;
        gm.rightTroops[2] = spawned;

        tPos = grid[3].row[11];
        tPos.occupy();
        spawned = Instantiate(soldierTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 3;
        tPos.getCurrentTroop().posY = 11;
        gm.rightTroops[3] = spawned;

        tPos = grid[4].row[10];
        tPos.occupy();
        spawned = Instantiate(soldierTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 4;
        tPos.getCurrentTroop().posY = 10;
        gm.rightTroops[4] = spawned;

        tPos = grid[5].row[7];
        tPos.occupy();
        spawned = Instantiate(tankTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
        tPos.setCurrentTroop(spawned);
        tPos.getCurrentTroop().team = "right";
        tPos.getCurrentTroop().posX = 5;
        tPos.getCurrentTroop().posY = 7;
        gm.rightTroops[5] = spawned;
    }
}
