/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 

- This file pathfinds the graph of nodes from the HW6 assignment
- using Dijkstra's algorithm to navigate weighted graphs. Given
- a weighted graph, the findShortestPath function will return
- the weight of the path and each step along the path to Pacman.

*/

// Import necessary libraries
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Queue;

public class PacmanDijkstra {

   // Custom tree node for storing the necessary data, and the parent and child variables of each node
   public static class TreeNode {
 
      /** The element stored at this node */
      private DijkstraNode<Character> element;            // reference to the element stored at this node

      /** A reference to the parent node */
      private TreeNode parent;

      /** A reference to the child nodes */
      private ArrayList<TreeNode> children;

      /** A reference to the number of child nodes */
      private int numChildren;
  
      /**
       * Creates a node with the given element and next node.
       *
       * @param e  the element to be stored
       * @param p  reference to the parent node
       * 
       * Also asigns the child to null, as it has no children by default,
       * and declares the number of children to be 0, as it has no children
       */
      public TreeNode(DijkstraNode<Character> e, TreeNode p) {
         element = e;
         parent = p;
         children = new ArrayList<TreeNode>();
         numChildren = 0;
      }
  
      // Accessor methods
      /**
       * Returns the element stored at the node.
       * @return the element stored at the node
       */
      public DijkstraNode<Character> getElement() { return element; }

      /**
       * Returns the child node (or null if no such node).
       * @return the child node
       */
      public TreeNode getParent() { return parent; }

      // Modifier methods
      /**
       * Sets the node's child reference to point to Node child.
       * @param c    the child node
       */
      public void setParent(TreeNode p) { parent = p; }

      /**
       * Returns the child node (or null if no such node).
       * @return the child node
       */
      public ArrayList<TreeNode> getChildren() { return children; }
  
      // Modifier methods
      /**
       * Sets the node's child reference to point to Node child.
       * @param c    the child node
       */
      public void addChild(TreeNode c) { children.add(c); numChildren++; }

      /**
       * Removes a child from the list of children.
       * @param index    the index of the child to remove from
       */
      public void removeChild(int index) { children.remove(index); numChildren--; }

      /**
       * Returns the number of child nodes
       * @return the number of child nodes
       */
      public int getNumOfChild() { return numChildren; }
   } //----------- end of nested Node class -----------

   // Custom Object for returning a DValue weight and the path to Pacman
   public class DijkstraPathAndDValue {
      public int dValue;
      public ArrayList<DijkstraNode<Character>> path;

      // Constructor given a dValue and a path
      public DijkstraPathAndDValue(int _dValue, ArrayList<DijkstraNode<Character>> _path) {
         dValue = _dValue;
         path = _path;
      }
   }

   // Custom key to be used in heap data structure.
   // Enforces udlr precedence when choosing where to move.
   public class PacKey implements Comparable {
      int dValue;
      String pathTaken;

      // Constructor given an initial dValue and String to store path
      public PacKey(int dVal, String path) {
         dValue = dVal;
         pathTaken = path;
      }

      // Getter methods
      public int getDValue() { return dValue; }
      public String getPathTaken() { return pathTaken; }

      // Setter methods
      public void setDValue(int value) { dValue = value; }
      public void appendPathTaken(String element) { pathTaken = pathTaken + element; }
      public void setPathTaken(String path) { pathTaken = path; }

      // CompareTo method for assortment in the heap
      @Override
      public int compareTo(Object otherKey) {
         PacKey other = (PacKey) otherKey;
         if (dValue < other.getDValue()) {
            return -1;
         } else if (dValue > other.getDValue()) {
            return 1;
         } else {
            return pathTaken.compareTo(other.getPathTaken());
         }
      }
   }

   // Reference to the root node of the tree that
   // will be formed from the graph
   public TreeNode root;

   // Constructor for Dijkstra tree given a root node
   public PacmanDijkstra(DijkstraNode<Character> rootValue) {
      root = new TreeNode(rootValue, null);
      root.getElement().setDValue(0);
   }

   // Function to find shortest path to a given target
   @SuppressWarnings("unchecked")
   public DijkstraPathAndDValue findShortestPath(char target) {
      // Initialize a list to store the pathway to the target
      ArrayList<DijkstraNode<Character>> path = new ArrayList<DijkstraNode<Character>>();
      
      // Set up a heap to store and prioritize which nodes should be taken next
      HeapPriorityQueue<PacKey, TreeNode> navigator = new HeapPriorityQueue<PacKey, TreeNode>();

      // Insert the root as it is the first element to be checked
      navigator.insert(new PacKey(0, ""), root);

      // Assign variable to identify target node
      DijkstraNode targetNode = null;

      // Keep looping until every possible path has been checked
      while (navigator.size() > 0) {
         // Take the next node to search from the heap
         HeapPriorityQueue.HeapEntry minEntry = navigator.removeMin();

         // Retrieve the TreeNode from the heap entry value and mark it as visited
         TreeNode currNode = (TreeNode) minEntry.getValue();
         currNode.getElement().setVisited(true);

         // Attempt to get up, down, left, and right nodes as possible path choices
         DijkstraNode[] choices = {currNode.getElement().above(), currNode.getElement().below(), currNode.getElement().getPrev(), currNode.getElement().getNext()};
         
         // Check if each possible choice can be taken and add them to the heap
         // after processing the data stored in the grid
         for (int i = 0; i < choices.length; i++) {

            // Assign a reference to the current choice and mark it as visited if it
            // isn't null or already marked
            DijkstraNode<Character> currChoice = null;
            if (choices[i] != null && !choices[i].isVisited()) {
               currChoice = (DijkstraNode<Character>) choices[i];
               currChoice.setVisited(true);
            }

            // If choice is valid, calculte weight and add to tree
            if (currChoice != null) {
               // Create tree node to add to tree
               TreeNode newNode = new TreeNode(currChoice, currNode);

               // Create variables to store dValue and movement direction of current step
               int newDValue = -1;
               String newDirection = "";

               // 1 = up, 2 = down, 3 = left, 4 = right
               // Assign node dValue to parent dValue plus the weight between parent and node
               switch (i) {
                  case 0:
                     newDValue = currNode.getElement().getDValue() + currChoice.getWeightBelow();
                     newDirection = "1";
                     break;
                  case 1:
                     newDValue = currNode.getElement().getDValue() + currChoice.getWeightAbove();
                     newDirection = "2";
                     break;
                  case 2:
                     newDValue = currNode.getElement().getDValue() + currChoice.getWeightNext();
                     newDirection = "3";
                     break;
                  case 3:
                     newDValue = currNode.getElement().getDValue() + currChoice.getWeightPrev();
                     newDirection = "4";
                     break;
                  default:
                     break;
               }

               // Reassign dValue if new dValue is less than node's current value
               if (currChoice.getDValue() > newDValue) {
                  currChoice.setDValue(newDValue);
                  currChoice.setParent(currNode.getElement());
               }

               // If current node is the target, mark the target node as found
               if (targetNode == null && currChoice.getElement() == target) {
                  targetNode = currChoice;
               }

               // Get a reference to the PacKey stored from the heap entry
               PacKey pk = (PacKey) minEntry.getKey();

               // Insert a new entry into the heap with the new dValue and a path
               // adding the new movement direction to the parent's path
               navigator.insert(new PacKey(currChoice.getDValue(), pk.getPathTaken() + newDirection), newNode);
            }
         }
      }

      // Get reference to target to begin path trace
      DijkstraNode<Character> walk = targetNode;

      // Assign value to keep track of the dValue of target node
      int minDVal = 0;

      // If target was found, store its dValue
      if (walk != null) minDVal = walk.getDValue();

      // Store each part of path into the list
      while (walk != null) {
         path.add(walk);
         walk = walk.parent();
      }

      // Unmark all visited nodes
      ClearVisitedStatus(root);

      // Return resulting path and dValue
      return new DijkstraPathAndDValue(minDVal, path);
   }

   // Function to clear visited nodes for next search
   @SuppressWarnings("unchecked")
   public void ClearVisitedStatus (TreeNode root) {
      // Create queue to keep track of all visited nodes
      Queue<TreeNode> navigator = new LinkedList<TreeNode>();
      // Add root node to unvisited tree
      navigator.add(root);

      // Reset parent and dValue values of root
      root.getElement().setParent(null);
      root.getElement().setDValue(Integer.MAX_VALUE);

      // Keep clearing visited nodes until all proper nodes are uncleared
      while (navigator.size() > 0) {
         // Remove next queue element from list
         TreeNode currNode = navigator.remove();

         // Mark current node as unvisited
         currNode.getElement().setVisited(false);

         // Attempt to get up, down, left, and right nodes as possible path choices
         DijkstraNode[] choices = {currNode.getElement().above(), currNode.getElement().below(), currNode.getElement().getPrev(), currNode.getElement().getNext()};
         
         // Check if each possible choice can be taken and add them to the heap
         // after processing the data stored in the grid
         for (int i = 0; i < choices.length; i++) {

            // Assign a reference to the current choice and mark it as unvisited if it
            // isn't null or already unmarked
            DijkstraNode<Character> currChoice = null;
            if (choices[i] != null && choices[i].isVisited()) {
               currChoice = (DijkstraNode<Character>) choices[i];

               // Reset parent and dValue values of root
               currChoice.setParent(null);
               currChoice.setDValue(Integer.MAX_VALUE);

               // Set desired grid values to false
               if (currChoice.getElement() == ' ' || currChoice.getElement() == '.' || currChoice.getElement() == 'P') {
                  currChoice.setVisited(false);
               } 
               else {
                  // Mark choice as null, do not want to add this node to the tree
                  currChoice = null;
               }
            }
            // If choice is valid then add it to the tree and add it to the queue
            if (currChoice != null) {
               TreeNode newNode = new TreeNode(currChoice, currNode);
               currNode.addChild(newNode);
               navigator.add(newNode);
            }
         }
      }
   }
}
