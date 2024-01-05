/*

Authors (group members): Daniel Acosta, Luka Miodrag Starcevic, Jon Ayuco
Email addresses of group members: dacosta2022@my.fit.edu ; lstarcevic2022@my.fit.edu ; jayuco2022@my.fit.edu
Group name: Better than C.A.M.E.R.O.N.

Course: CSE2010
Section: 12

Description of the overall algorithm and key data structures:
Trie Data Structure:
- The trie is a simple, uncompressed trie where each node points to
- its first child, if it has one, as well as its next sibling, if it has one.
- This structure is built as a singly linked list. In BogglePlayer, to avoid
- needing to traverse all 26 letters, an array is used to instantly determine
- which trie to go to. Instantly choosing the correct trie based on what letter
- is the root saves a significant amount of time on the search algorithm.
*/

// Import necessary libraries
import java.util.ArrayList;

public class Trie {
   // Embedded Node class to represent nodes in the trie
   public static class Node {
      private final char data;
      private Node firstChild;
      private Node nextSibling;
      private int height;
      private boolean isFinalLetter;
      
      // Constructor for a Node given the character that the node should represent
      public Node (final char data) {
         this.data = Character.toUpperCase(data);
         this.isFinalLetter = false;
      }

      // Getter function to get character stored in node
      public char getData () {
         return this.data;
      }

      // Check if the node represents the end of a word
      public boolean isFinalLetter () {
         return this.isFinalLetter;
      }

      public Node addChild (final char child) {
         Node current = firstChild;
         while (current != null) {
            if (current.data == child) {
               return current;
            }
            current = current.nextSibling;
         }

         final Node childNode = new Node(child);
         childNode.nextSibling = firstChild;
         firstChild = childNode;

         return childNode;
      }

      // Mark this node as the end of a word
      public void makeFinal () {
         this.isFinalLetter = true;
      }

      // Mark this node as not being the end of a word
      public void makeNotFinal () {
         this.isFinalLetter = false;
      }
      
      // Return the first child of this node
      public Node getFirstChild () {
         return this.firstChild;
      }
      
      // Return the next sibling of this node
      public Node next () {
         return this.nextSibling;
      }

      // Return a list of all of the children of this node
      public ArrayList<Node> getChildren () {
         final ArrayList<Node> children = new ArrayList<>();
         
         Node current = this.firstChild;
         while (current != null) {
            children.add(current);
            current = current.nextSibling;
         }

         return children;
      }
   }

   // Get a reference to the root node
   private Node root;

   // Constructor for the trie given the character that is desired to be the root
   public Trie (final char root) {
      this.root = new Node(root);
      this.root.height = 0;
   }

   // Basic function to add a child to the root given only a char value
   public void addChild (final char newChild) {
      addChild(root, newChild);
   }

   // Add a child to a given parent, given the char value that will be
   // the data of the new child.
   public Node addChild (final Node parent, final char newChild) {
      final Node childNode = new Node(newChild);
      return addChild(parent, childNode);
   }

   // Test code for future improvements
   // public Node addChildWithHeight (final Node parent, final char newChild, final int childHeight) {
   //    final Node childNode = new Node(newChild);
   //    return addChildWithHeight(parent, childNode, childHeight);
   // }

   // Add a given node as a child of a given parent node
   public Node addChild (final Node parent, final Node newChild) {
      // Test code for future improvements
      // newChild.height = parent.height + 1;

      // Always add the new node to be the first node to avoid traversal
      newChild.nextSibling = parent.firstChild;
      parent.firstChild = newChild;

      return newChild;
   }

   // Test code for future improvements
   // public Node addChildWithHeight(final Node parent, final Node newChild, final int childHeight) {
   //    newChild.height = childHeight;

   //    if (newChild.height + 1 > parent.height) {
   //       parent.height = newChild.height + 1;
   //    }

   //    if (parent.firstChild == null) {
   //       newChild.nextSibling = parent.firstChild;
   //       parent.firstChild = newChild;
   //    } else {
   //       Node current = parent.firstChild;
   //       if (parent.firstChild.height > childHeight) {
   //          parent.firstChild.nextSibling = newChild;
   //       } else {
   //          while (current.nextSibling != null) {
   //             if (current.nextSibling.height < newChild.height) {
   //                break;
   //             }
   //          }
   //          newChild.nextSibling = current.nextSibling;
   //       }
   //    }

   //    return newChild;
   // }

   // Getter function to get reference to root node
   public Node getRoot () {
      return this.root;
   }

   // Setter function to set root to a different node if necessary
   public void setRoot (final char newRoot) {
      this.root = new Node(newRoot);
   }

   // Function to add an entire word to the trie given a string
   public void addWord (final String word) {
      // Start placing the new word from the root downward
      Node current = root;

      int letterNum = root.data == 'Q' ? 2 : 1; // Skip letter U
      char letter;

      // Add each letter of the word to the trie one by one
      while(letterNum < word.length()) {
         letter = word.charAt(letterNum);
         if (letter == 'Q') letterNum++; // Skip letter U
         boolean skip = false;
         for (final Node i : current.getChildren()) {
            if (i.data == letter) {
               // Move to this node to avoid creating a duplicate child node
               current = i;
               letterNum++;
               skip = true;
               break;
            }
         }

         // Don't create a new child node if letter already exists as a child
         if (skip) continue;

         // final Node newChild = addChildWithHeight(current, letter, word.length() - letterNum);
         final Node newChild = addChild(current, letter);
         current = newChild;
         letterNum++;

         // Mark the last letter as the end of the word in the trie
         if (letterNum == word.length()) {
            newChild.makeFinal();
         }
      }
   }
}
