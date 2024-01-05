/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 

- This file pathfinds the graph of nodes from the HW6 assignment
- using a breadth first search to navigate unweighted graphs. Given
- an unweighted graph, the findShortestPath function will return
- each step along the path to Pacman.

*/

// Import necessary libraries
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Queue;

public class PacmanBreadthFirstTree {

   // Custom tree node for storing the necessary data, and the parent and child variables of each node
   public static class TreeNode {
 
      /** The element stored at this node */
      private Node<Character> element;            // reference to the element stored at this node

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
      public TreeNode(Node<Character> e, TreeNode p) {
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
      public Node<Character> getElement() { return element; }

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

   // Reference to the root node of the tree that
   // will be formed from the graph
   public TreeNode root;

   // Constructor for breadth first tree given a root node
   public PacmanBreadthFirstTree(Node<Character> rootValue) {
      root = new TreeNode(rootValue, null);
   }

   // Function to find shortest path to a given target
   @SuppressWarnings("unchecked")
   public ArrayList<Node<Character>> findShortestPath(char target) {
      // Initialize a list to store the pathway to the target
      ArrayList<Node<Character>> path = new ArrayList<Node<Character>>();
      
      // Set up a queue to store and prioritize which nodes should be checked next
      Queue<TreeNode> navigator = new LinkedList<TreeNode>();

      // Insert the root as it is the first element to be checked
      navigator.add(root);

      // Assign variable to end search once target is found.
      // Breadth first search always finds the shortest path once the target is found.
      boolean targetFound = false;

      // Assign variable to identify target node
      TreeNode targetNode = null;

      // Keep looping until there is no steps left in path or target is found
      while (navigator.size() > 0 && !targetFound) {
         // Take the next node to search from the queue
         TreeNode currNode = navigator.remove();

         // Mark current node as visited
         currNode.getElement().setVisited(true);

         // Attempt to get up, down, left, and right nodes as possible path choices
         Node[] choices = {currNode.getElement().above(), currNode.getElement().below(), currNode.getElement().getPrev(), currNode.getElement().getNext()};
         
         // Check if each possible choice can be taken and add them to the heap
         // after processing the data stored in the grid
         for (int i = 0; i < choices.length; i++) {

            // Assign a reference to the current choice and mark it as visited if it
            // isn't null or already marked
            Node<Character> currChoice = null;
            if (choices[i] != null && !choices[i].isVisited()) {
               currChoice = (Node<Character>) choices[i];
               currChoice.setVisited(true);
            }

            // If choice is valid, add it to the tree
            if (currChoice != null) {
               TreeNode newNode = new TreeNode(currChoice, currNode);
               currNode.addChild(newNode);
               // Add it to the queue
               navigator.add(newNode); 

               // If current choice is the target, mark target node and target found
               if (currChoice.getElement().charValue() == target) {
                  targetNode = newNode;
                  targetFound = true;
               }
            }
         }
      }

      // Get reference to target to begin path trace
      TreeNode walk = targetNode;

      // Store each part of path into the list
      while (walk != null) {
         path.add(walk.getElement());
         walk = walk.getParent();
      }

      // Unmark all visited nodes
      ClearVisitedStatus(root);

      // Return resulting path
      return path;
   }

   // Function to clear visited nodes for next search
   @SuppressWarnings("unchecked")
   private void ClearVisitedStatus (TreeNode root) {
      // Create queue to keep track of all visited nodes
      Queue<TreeNode> navigator = new LinkedList<TreeNode>();
      // Add root node to unvisited tree
      navigator.add(root);

      // Keep clearing visited nodes until all proper nodes are uncleared
      while (navigator.size() > 0) {
         // Remove next queue element from list
         TreeNode currNode = navigator.remove();

         // Mark current node as unvisited
         currNode.getElement().setVisited(false);

         // Attempt to get up, down, left, and right nodes as possible path choices
         Node[] choices = {currNode.getElement().above(), currNode.getElement().below(), currNode.getElement().getPrev(), currNode.getElement().getNext()};
         
         // Check if each possible choice can be taken and add them to the heap
         // after processing the data stored in the grid
         for (int i = 0; i < choices.length; i++) {

            // Assign a reference to the current choice and mark it as unvisited if it
            // isn't null or already unmarked
            Node<Character> currChoice = null;
            if (choices[i] != null && choices[i].isVisited()) {
               currChoice = (Node<Character>) choices[i];

               // Set desired grid values to false
               if (currChoice.getElement() == ' ' || currChoice.getElement() == '.' || currChoice.getElement() == 'P') {
                  currChoice.setVisited(false);
               } else {
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
