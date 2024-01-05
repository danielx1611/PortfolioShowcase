/*
 * Copyright 2014, Michael T. Goodrich, Roberto Tamassia, Michael H. Goldwasser
 *
 * Developed for use with the book:
 *
 *    Data Structures and Algorithms in Java, Sixth Edition
 *    Michael T. Goodrich, Roberto Tamassia, and Michael H. Goldwasser
 *    John Wiley & Sons, 2014
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * Modified By: Daniel Acosta
 * 
 * Modifications:
 * Generic K key type should extend comparable to use compareTo method
 * Modified PQEntry to a custom HeapEntry class to keep track of its index in the heap.
 * Modified swap to update the HeapEntry's indices as they swap.
 * Modified HeapPriorityQueue constructors to accomodate new Entry type.
 * Created custom removeAtIndex function to remove a HeapEntry from anywhere in the heap given its index.
 * Created custom FindHighestPriorityKeyVariant function to find the highest priority HeapEntry 
 * among HeapEntries with duplicate Main Keys. Searches for the highest priority Secondary Key.
 * Main Key: Standard priority level indicator, e.g. 1-5.
 * Secondary Key: Alternate attribute to help indicate subpriorities, e.g. earlier times.
 */

//package net.datastructures;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;

/**
 * An implementation of a priority queue using an array-based heap.
 *
 * @author Michael T. Goodrich
 * @author Roberto Tamassia
 * @author Michael H. Goldwasser
 */
public class HeapPriorityQueue<K extends Comparable,V> extends AbstractPriorityQueue<K,V> {
  /** primary collection of priority queue entries */
   protected ArrayList<HeapEntry<K,V>> heap = new ArrayList<>();

  // Custom HeapEntry variant of the PQEntry class used to keep track of an entry's index in the heap
  public class HeapEntry<K extends Comparable, V> extends PQEntry<K,V> {
    // Private int to keep track of the entry's position in the heap
    int heapIndex;

    // Constructor for the entry given a key, value, and index to be positioned at
    public HeapEntry (K key, V value, int _index) {
      // Assign values as done in PQEntry class
      super(key, value);

      // Assign index of entry, should be placed at the end of the heap on construction
      heapIndex = _index;
    }

    // Getter/Setter methods for the entry's index
    public K getKey() { return super.getKey(); }
    public V getValue() { return super.getValue(); }
    public int getHeapIndex() { return heapIndex; }
    public void setHeapIndex(int newIndex) { heapIndex = newIndex; }
    public void updateKey(K newKey) {
      super.setKey(newKey);
      if (heapIndex == 0) {
        downheap(heapIndex);
        return;
      } else {
        int direction = compare(heap.get(heapIndex), heap.get(parent(heapIndex)));
        if (direction > 0) {
          upheap(heapIndex);
        } else {
          downheap(heapIndex);
        }
      }
    }
    public void updateValue(V newValue) {super.setValue(newValue);}
  }

  /** Creates an empty priority queue based on the natural ordering of its keys. */
  public HeapPriorityQueue() { super(); }

  /**
   * Creates an empty priority queue using the given comparator to order keys.
   * @param comp comparator defining the order of keys in the priority queue
   */
  public HeapPriorityQueue(Comparator<K> comp) { super(comp); }

  /**
   * Creates a priority queue initialized with the respective
   * key-value pairs.  The two arrays given will be paired
   * element-by-element. They are presumed to have the same
   * length. (If not, entries will be created only up to the length of
   * the shorter of the arrays)
   * @param keys an array of the initial keys for the priority queue
   * @param values an array of the initial values for the priority queue
   */
  public HeapPriorityQueue(K[] keys, V[] values) {
    super();
    for (int j=0; j < Math.min(keys.length, values.length); j++) {
      HeapEntry<K,V> currEntry = new HeapEntry<K,V>(keys[j], values[j], j);
      heap.add(currEntry);
    }
    heapify();
  }

  // protected utilities
  protected int parent(int j) { return (j-1) / 2; }     // truncating division
  protected int left(int j) { return 2*j + 1; }
  protected int right(int j) { return 2*j + 2; }
  protected boolean hasLeft(int j) { return left(j) < heap.size(); }
  protected boolean hasRight(int j) { return right(j) < heap.size(); }

  /** Exchanges the entries at indices i and j of the array list. */
  protected void swap(int i, int j) {
    HeapEntry<K,V> temp = heap.get(i);
    int tempIndex = temp.getHeapIndex();
    
    heap.get(i).setHeapIndex(heap.get(j).getHeapIndex());
    heap.set(i, heap.get(j));
    heap.get(j).setHeapIndex(tempIndex);
    heap.set(j, temp);
  }

  /** Moves the entry at index j higher, if necessary, to restore the heap property. */
  protected void upheap(int j) {
    while (j > 0) {            // continue until reaching root (or break statement)
      int p = parent(j);
      if (compare(heap.get(j), heap.get(p)) >= 0) break; // heap property verified
      swap(j, p);
      j = p;                                // continue from the parent's location
    }
  }

  /** Moves the entry at index j lower, if necessary, to restore the heap property. */
  protected void downheap(int j) {
    while (hasLeft(j)) {               // continue to bottom (or break statement)
      int leftIndex = left(j);
      int smallChildIndex = leftIndex;     // although right may be smaller
      if (hasRight(j)) {
          int rightIndex = right(j);
          if (compare(heap.get(leftIndex), heap.get(rightIndex)) > 0)
            smallChildIndex = rightIndex;  // right child is smaller
      }
      if (compare(heap.get(smallChildIndex), heap.get(j)) >= 0)
        break;                             // heap property has been restored
      swap(j, smallChildIndex);
      j = smallChildIndex;                 // continue at position of the child
    }
  }

  /** Performs a bottom-up construction of the heap in linear time. */
  protected void heapify() {
    int startIndex = parent(size()-1);    // start at PARENT of last entry
    for (int j=startIndex; j >= 0; j--)   // loop until processing the root
      downheap(j);
  }

  // public methods

  /**
   * Returns the number of items in the priority queue.
   * @return number of items
   */
  @Override
  public int size() { return heap.size(); }

  /**
   * Returns (but does not remove) an entry with minimal key.
   * @return entry having a minimal key (or null if empty)
   */
  @Override
  public HeapEntry<K,V> min() {
    if (heap.isEmpty()) return null;
    return heap.get(0);
  }

  /**
   * Inserts a key-value pair and return the entry created.
   * @param key     the key of the new entry
   * @param value   the associated value of the new entry
   * @return the entry storing the new key-value pair
   * @throws IllegalArgumentException if the key is unacceptable for this queue
   */
  
  @SuppressWarnings("unchecked")
  public HeapEntry<K,V> insert(K key, V value) throws IllegalArgumentException {
    checkKey(key);      // auxiliary key-checking method (could throw exception)
    HeapEntry<K,V> newest = new HeapEntry<K,V>(key, value, heap.size());
    heap.add(newest);                      // add to the end of the list
    upheap(heap.size() - 1);               // upheap newly added entry
    return newest;
  }

  /**
   * Removes and returns an entry with minimal key.
   * @return the removed entry (or null if empty)
   */
  @SuppressWarnings("unchecked")
  @Override
  public HeapEntry<K,V> removeMin() {
    if (heap.isEmpty()) return null;
    HeapEntry<K,V> answer = heap.get(0);
    swap(0, heap.size() - 1);              // put minimum item at the end
    heap.remove(heap.size() - 1);          // and remove it from the list;
    downheap(0);                           // then fix new root
    return answer;
  }

  // Remove an element from the heap at a given index
  @SuppressWarnings("unchecked")
  public HeapEntry<K,V> removeAtIndex(int index) {

    if (index == 0) {
      return removeMin();
    }

    if (index == heap.size() - 1) {
      HeapEntry<K,V> temp = heap.remove(heap.size() - 1);
      return temp;
    }

    // Assign a temporary variable to keep track of the entry that will be removed
    HeapEntry<K,V> temp = heap.get(index);
   
    // If the entry is valid, swap with the last item and remove it from the heap
    if (temp != null && index > 0 && index < heap.size()) {
      // Swap selected entry with last entry in heap
      swap(index, heap.size() - 1);

      // Remove selected entry from end of heap
      heap.remove(heap.size() - 1);

      // Downheap or upheap at the selected entry's original location to restore heap order
      if (compare(heap.get(index), heap.get(parent(index))) > 0) {
         upheap(index);
      } else {
         downheap(index);
      }

      // Return copy of selected entry
      return temp;
    }

    // If no valid entry found, return null
    return null;
  }
  
  // Find the highest priority HeapEntry among HeapEntries with duplicate Main Keys. 
  // Searches for the highest priority Secondary Key.
  // Main Key: Standard priority level indicator, e.g. 1-5.
  // Secondary Key: Alternate attribute to help indicate subpriorities, e.g. earlier times.
  @SuppressWarnings("unchecked")
  public int FindHighestPriorityKeyVariant(K key) {
    // Assign minIndex value to value outside of array in case no matches are found
    int minIndex = -1;

    // For each element in the heap, check if it is the desired entry
    for (int i = 0; i < heap.size(); i++) {
      // Check if the element matches the desired key. If there is already
      // a match, determine which match has a higher priority.
      if (key.equals(heap.get(i).getKey())) {
        if (minIndex == -1) {
          // The first match to be found
          minIndex = i;
        } else if (heap.get(minIndex).getKey().compareTo(heap.get(i).getKey()) > 0) {
          // The new match is of a higher priority than the current match,
          // so replace it with the new match.
          minIndex = i;
        }
      }
    }

    // Return index of highest priority match
    return minIndex;
  }

  /** Used for debugging purposes only */
  private void sanityCheck() {
    for (int j=0; j < heap.size(); j++) {
      int left = left(j);
      int right = right(j);
      if (left < heap.size() && compare(heap.get(left), heap.get(j)) < 0)
        System.out.println("Invalid left child relationship");
      if (right < heap.size() && compare(heap.get(right), heap.get(j)) < 0)
        System.out.println("Invalid right child relationship");
    }
  }
}
