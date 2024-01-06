using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] bossList;

    [SerializeField] GameObject leftBoss;
    [SerializeField] Button leftBossDec;
    [SerializeField] Button leftBossInc;

    [SerializeField] GameObject rightBoss;
    [SerializeField] Button rightBossDec;
    [SerializeField] Button rightBossInc;

    public Troop currentTroop;
    public bool isTroopSelected;
    [HideInInspector] public bool isInPlacementPhase;
    [HideInInspector] public int maxTroopCount;
    [HideInInspector] public int placedTroopCount;
    [HideInInspector] public int friendlyTroopCount;
    [HideInInspector] public int enemyTroopCount;

    [HideInInspector] public int leftBossSelectIndex;
    [HideInInspector] public int rightBossSelectIndex;

    [HideInInspector] public PriorityQueue<Troop> troopPQ;
    public int currentPQMin;
    public Troop[] leftTroops;
    public Troop[] rightTroops;

    [HideInInspector] public bool gameWon;

    Vector3 leftPos;
    Vector3 rightPos;

    // Start is called before the first frame update
    void Start()
    {
        currentTroop = null;
        isTroopSelected = false;
        isInPlacementPhase = true;
        gameWon = false;

        maxTroopCount = 6;
        placedTroopCount = 0;
        enemyTroopCount = maxTroopCount;
        friendlyTroopCount = maxTroopCount;

        troopPQ = new PriorityQueue<Troop>();
        leftTroops = new Troop[maxTroopCount];
        rightTroops = new Troop[maxTroopCount];

        leftBossSelectIndex = 1;
        rightBossSelectIndex = 2;

        leftPos = new Vector3(leftBoss.transform.position.x, leftBoss.transform.position.y, leftBoss.transform.position.z);
        rightPos = new Vector3(rightBoss.transform.position.x, rightBoss.transform.position.y, rightBoss.transform.position.z);
    }

    public void UpdateLeftBossPreview(int change)
    {
        if (leftBossSelectIndex == bossList.Length - 1)
        {
            leftBossInc.interactable = true;
        }
        else if (leftBossSelectIndex == 0)
        {
            leftBossDec.interactable = true;
        }
        leftBossSelectIndex += change;
        Destroy(leftBoss);
        leftBoss = Instantiate(bossList[leftBossSelectIndex], leftPos, Quaternion.identity);
        leftBoss.transform.localScale = new Vector3(-3, 3, 3);
        if (leftBossSelectIndex == bossList.Length - 1)
        {
            leftBossInc.interactable = false;
        } else if (leftBossSelectIndex == 0)
        {
            leftBossDec.interactable = false;
        }
    }

    public void UpdateRightBossPreview(int change)
    {
        if (rightBossSelectIndex == bossList.Length - 1)
        {
            rightBossInc.interactable = true;
        }
        else if (rightBossSelectIndex == 0)
        {
            rightBossDec.interactable = true;
        }
        rightBossSelectIndex += change;
        Destroy(rightBoss);
        rightBoss = Instantiate(bossList[rightBossSelectIndex], rightPos, Quaternion.identity);
        rightBoss.transform.localScale = new Vector3(3, 3, 3);
        if (rightBossSelectIndex == bossList.Length - 1)
        {
            rightBossInc.interactable = false;
        }
        else if (rightBossSelectIndex == 0)
        {
            rightBossDec.interactable = false;
        }
    }

    public void SpawnLeftBoss()
    {
        leftBoss = Instantiate(bossList[leftBossSelectIndex], leftPos, Quaternion.identity);
        leftBoss.transform.localScale = new Vector3(-3, 3, 3);
    }

    public void SpawnRightBoss()
    {
        rightBoss = Instantiate(bossList[rightBossSelectIndex], rightPos, Quaternion.identity);
        rightBoss.transform.localScale = new Vector3(3, 3, 3);
    }

    public void PopulateQueue()
    {
        for (int i = 0; i < maxTroopCount; i++)
        {
            troopPQ.Enqueue(leftTroops[i], leftTroops[i].waitTime);
            troopPQ.Enqueue(rightTroops[i], rightTroops[i].waitTime);
        }
    }

    public void RemoveElementFromQueue(Troop entry)
    {
        troopPQ.RemoveEntryFromQueue(entry);
    }

    public void PrintQueue()
    {
        Debug.Log("Printing pq");
        PriorityQueue<Troop>.Node currNode = troopPQ.root;
        while (currNode != null)
        {
            Troop currTroop = currNode.element;
            Debug.Log(currTroop.team + " " + currNode.data);
            currNode = currNode.next;
        }
        Debug.Log("Done printing");
    }

    public void MoveToNextTroopInQueue()
    {
        PriorityQueue<Troop>.Node currentTroopNode = troopPQ.Dequeue();
        currentTroop = currentTroopNode.element;
        currentPQMin = currentTroopNode.data;
        isTroopSelected = true;
    }

    public void EndGame()
    {
        FadePanel fp = FindObjectOfType<FadePanel>();
        fp.StartSceneTransition();
    }
}

public class PriorityQueue<T>
{
    public class Node
    {
        public Node next;
        public Node prev;
        public Troop element;
        public int data;

        public Node(Troop element, int data, Node next, Node prev)
        {
            this.element = element;
            this.data = data;
            this.next = next;
            this.prev = prev;
        }
    }

    public Node root = null;
    public Node tail = null;
    public int Count = 0;

    public void Enqueue(Troop item, int priority)
    {
        if (root == null)
        {
            root = new Node(item, priority, null, null);
            tail = root;
            Count++;
        } else
        {
            Node currNode = root;
            Node newNode;
            if (Count == 1)
            {
                if (priority < root.data)
                {
                    newNode = new Node(item, priority, root, null);
                    root.prev = newNode;
                    root = newNode;
                } else
                {
                    newNode = new Node(item, priority, null, root);
                    root.next = newNode;
                    tail = newNode;
                }
                Count++;
            } else
            {
                while (currNode.next != null)
                {
                    if (currNode == root)
                    {
                        if (priority < root.data)
                        {
                            newNode = new Node(item, priority, root, null);
                            root.prev = newNode;
                            root = newNode;
                            Count++;
                            return;
                        }
                    } else
                    {
                        if (priority < currNode.data)
                        {
                            newNode = new Node(item, priority, currNode, currNode.prev);
                            newNode.prev.next = newNode;
                            currNode.prev = newNode;
                            Count++;
                            return;
                        }
                    }
                    currNode = currNode.next;
                }
                newNode = new Node(item, priority, null, null);
                if (priority < tail.data)
                {
                    newNode.next = tail;
                    newNode.prev = tail.prev;
                    tail.prev.next = newNode;
                    tail.prev = newNode;
                } else
                {
                    newNode.prev = tail;
                    tail.next = newNode;
                    tail = newNode;
                }
                Count++;
            }
        }
    }

    public Node Dequeue()
    {
        if (root == null)
        {
            throw new InvalidOperationException("Priority queue is empty");
        } else
        {
            Node item = root;
            root = root.next;
            Count--;
            return item;
        }
    }

    public void RemoveEntryFromQueue(Troop entry)
    {
        Node currNode = root;
        if (entry == root.element)
        {
            Debug.Log("Removing root from pq");
            root = root.next;
            Count--;
        } else
        {
            currNode = currNode.next;
            while (currNode.next != null)
            {
                if (currNode.element == entry)
                {
                    Debug.Log("Removing from middle of pq");
                    currNode.prev.next = currNode.next;
                    currNode.next.prev = currNode.prev;
                    Count--;
                    return;
                }
                currNode = currNode.next;
            }
            if (entry == tail.element)
            {
                Debug.Log("Removing tail from pq");
                tail = tail.prev;
                tail.next = null;
                Count--;
            }
        }
    }
}