/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 
This file creates an array that enacts an in place insertion sort
to keep the contents of the array in lexicographical order. Since the
contents are guaranteed to be in order, a binary search can be conducted
in order to find a desired string in log n time. Removal is O(n) because it
has to shift the arrayList indices after removing the desired item.
*/

// Import arraylist to store values
import java.util.ArrayList;

public class InsertSortedHeapEntryArray {

   // Object to bind a string value to an entry for the binary search algorithm
   public class StringToHeapEntryBind implements Comparable {
      private String stringToBind;
      private HeapPriorityQueue.HeapEntry entryToBind;

      // Constructor to make an entry bind given a string and an entry to bind
      public StringToHeapEntryBind(String _stringToBind, HeapPriorityQueue.HeapEntry _entryToBind) {
         stringToBind = _stringToBind;
         entryToBind = _entryToBind;
      }

      // Getter/setters for the string and entry values
      public String getString() {return stringToBind;}
      public HeapPriorityQueue.HeapEntry getEntry() {return entryToBind;}
      public void setEntry(HeapPriorityQueue.HeapEntry newEntry) {entryToBind = newEntry;}

      // CompareTo function for comparing two bindings
      @Override
      public int compareTo(Object obj) {
         StringToHeapEntryBind newObj = (StringToHeapEntryBind) obj;
         return stringToBind.compareTo(newObj.getString());
      }
   }

   // Assign a variable to keep a list of all of the binded entries
   private ArrayList<StringToHeapEntryBind> list;

   // Constructor for the sorted array
   public InsertSortedHeapEntryArray() {
      list = new ArrayList<StringToHeapEntryBind>();
   }

   // Get function to return the entry at position i
   public StringToHeapEntryBind get(int i) {
      if (i < 0) {
         return null;
      }
      return list.get(i);
   }

   // Function to insert a new bounded string and entry lexicographically
   public int insert(String _string, HeapPriorityQueue.HeapEntry _entry) {
      // Make a new binding between the string and entry
      StringToHeapEntryBind entry = new StringToHeapEntryBind(_string, _entry);

      // If the list is empty, add it as the first node
      // and return its index as 0.
      if (list.isEmpty()) {
         list.add(entry);
         return 0;
      }

      // Add entry to the end of list
      list.add(entry);

      // Set the item to check as the newly added entry
      int walkIndex = list.size() - 1;

      // While the newly added entry is lexicographically less than the entry to the left,
      // swap the position of the new entry with the entry to the left.
      while (walkIndex > 0) {
         // Calculate lexiocgraphical comparison of the strings
         int result = list.get(walkIndex).compareTo(list.get(walkIndex - 1));

         // Determine what to do based on result
         if (result > 0) {
            // Entry found
            return walkIndex;
         } else if (result == 0) {
            // No duplicates allowed
            return -1;
         } else {
            // Swap the current index and the index to its left
            StringToHeapEntryBind temp = list.get(walkIndex - 1);
            list.set(walkIndex - 1, entry);
            list.set(walkIndex, temp);

            // Decrement the index to properly point to the new entry
            walkIndex--;
         }
      }

      // New entry must have been shifted all of the way to the left of the array,
      // so it must be as position 0.
      return 0;
   }

   // Remove an item from the list given its index
   public StringToHeapEntryBind removeAtIndex(int index) {
      StringToHeapEntryBind itemToRemove = list.get(index);
      list.remove(index);
      return itemToRemove;
   }

   // Remove an item from the list given its string binding
   public StringToHeapEntryBind remove(String item) {
      // Find the string in log n time
      int result = binarySearchList(item);

      if (result == -1) {
         // Item not in list
         return null;
      }

      // Return the found string entry binding
      return list.get(result);
   }

   // Binary search algorithm to find a given string in log n time
   public int binarySearchList(String itemToFind) {
      // Low and high variables for executing binarySearchList
      int low = 0;
      int high = list.size() - 1;
   
      // Execute the binary search until the item is found
      // or the search fails to find the desired item
      while (low <= high) {
         // Assign mid value to find middle of remaining search area
         int mid = (high + low) / 2;

         // Search for the item and update high/low accordingly
         if (list.get(mid).getString().compareTo(itemToFind) < 0) {
            low = mid + 1;
         } else if (list.get(mid).getString().compareTo(itemToFind) > 0) {
            high = mid - 1;
         }
         else {
            return mid;
         }
      }

      // Item was not found
      return -1;
   }
}