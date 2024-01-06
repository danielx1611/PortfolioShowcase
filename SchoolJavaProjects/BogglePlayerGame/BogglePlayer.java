/*

Authors (group members): Daniel Acosta, Luka Miodrag Starcevic
Email addresses of group members: dacosta2022@my.fit.edu ; lstarcevic2022@my.fit.edu
Group name: Better than C.A.M.E.R.O.N.

Course: CSE2010
Section: 12

Description of the overall algorithm and key data structures:
Algorithms:
- BogglePlayer constructor: reads word line by line from input file and sorts
- words into a trie. Also creates a grid to keep track of which possible neighbors
- you can travel to at any position on the board.

- getWords: Does a DFS of all 16 tiles on the Boggle board. During the DFS,
- the algorithm goes letter by letter down the trie until there is no feasible
- path to a word (there are no children of the node). If it comes across a word
- in the dictionary (marked in the trie as a final letter boolean), then it adds
- the found word to a heap that is limited to 20 entries. After the search finishes
- in all 16 tiles, the 20 highest value words that were found are returned to
- EvalBogglePlayer. The algorithm is very optimized and has many exit cases to
- prevent unecessary calculations.

- Currently runs on code01 in 3.3ms on average on the sample test case.

*/

// Import necessary libraries
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.stream.Stream;

import java.lang.management.ManagementFactory;
import java.lang.management.MemoryPoolMXBean;
import java.lang.management.MemoryType;

import java.util.ArrayList;
import java.util.List;

public class BogglePlayer {

   // Set up global variables to be referenced throughout class.
   // Declare array of tries and grid of board nodes for later initialization.
   final static int MIN_LENGTH = 3;
   final static int MAX_LENGTH = 8;
   final Trie[] dict = new Trie[26];
   final BoggleBoardNode[][] boardNodes = new BoggleBoardNode[4][4];

   // initialize BogglePlayer with a file of English words
   public BogglePlayer(String wordFile) throws IOException {
      // Initialize all the 26 tries, one for each letter of the alphabet
      // with the alphabet letter as the respective root node
      for (char i = 'a'; i <= 'z'; i++) {
         dict[i - 'a'] = new Trie(i);
      }

      // Initialize the Boggle board nodes for determining what neighbors
      // can be visited during the word search
      for (int i = 0; i < 4; i++) {
         for (int j = 0; j < 4; j++) {
            boardNodes[i][j] = new BoggleBoardNode(i, j);
         }
      }

      // Populate the lists of neighbors for each tile
      for (int i = 0; i < 4; i++) {
         for (int j = 0; j < 4; j++) {
            boardNodes[i][j].setNeighbors(findValidNeighbors(i, j));
         }
      }

      try (Stream<String> lines = Files.lines(Paths.get(wordFile))) {
         // Process each line as needed
         lines.forEach(line -> {
            if (line.length() >= MIN_LENGTH && line.length() <= MAX_LENGTH) {
               dict[Character.toUpperCase(line.charAt(0)) - 'A'].addWord(line.toUpperCase());
            }
         });
      } catch (IOException e) {
         e.printStackTrace();
      }
   }

   // based on the board, find valid words
   //
   // board: 4x4 board, each element is a letter, 'Q' represents "QU", 
   //    first dimension is row, second dimension is column
   //    ie, board[row][col]     
   //
   // Return at most 20 valid words in UPPERCASE and 
   //    their paths of locations on the board in myWords;
   //    Use null if fewer than 20 words.
   //
   // See Word.java for details of the Word class and
   //     Location.java for details of the Location class

   final static char[] word = new char[16];
   static int length = 0;

   public Word[] getWords(char[][] board) {

      Word[] myWords = new Word[20];

      // Initialize heap used to store the 20 highest value words
      final HeapPriorityQueue<Integer, Word> longestWords = new HeapPriorityQueue<>();

      // Run the DFS algorithm on each of the 16 Boggle board tiles
      for (int i = 0; i < 4; i++) {
         for (int j = 0; j < 4; j++) {
            search(longestWords, dict[board[i][j] - 'A'].getRoot(),
                    new ArrayList<Location>(), board, i, j);
         }
      }

      // Submit all 20 words back to EvalBogglePlayer,
      // replacing any instances of 'Q' with 'QU' to follow Boggle standards
      int i = 0;
      for (final Word wrd : longestWords.getValues()) {
         if (wrd.getWord().indexOf('Q') != -1) {
            wrd.setWord(fixQ(wrd.getWord()));
         }

         myWords[i] = wrd;
         i++;
      }

      return myWords;
   }

   // Declare variable to limit which words are allowed to be put into the heap
   // to save calculation time (e.g. if all words are 4+ letters long, do not
   // waste time trying to add a 3 letter word to the list)
   public static int minLength = 2;

   public void search (final HeapPriorityQueue<Integer, Word> longestWords, final Trie.Node start,
                       final ArrayList<Location> path, final char[][] board, final int x, final int y) {

      // Mark current position as visited so that the search does
      // not go over the same location twice
      boardNodes[x][y].visit();

      // Add the letter on the new board tile to the word for examination
      word[length] = board[x][y];
      length++;

      // Add the current location to the path of where the search currently is
      path.add(new Location(x, y));

      // If a word from the dictionary is encountered, determine if it should
      // be added to the heap. If so, add it to the heap
      if (start.isFinalLetter()) {
         // Prevent duplicates, word cannot trigger this function again
         start.makeNotFinal();

         // If there are less than 20 words in heap, then add current word
         // Otherwise, if the word is greater than the minimum length required
         // to enter the heap, then add it to the heap
         if (length > minLength || longestWords.size() < 20) {

            final ArrayList<Location> newPath = new ArrayList<>(path);

            final Word newWord = new Word(new String(word, 0, length));
            newWord.setPath(newPath);

            // Add to heap, and if over 20 entries, remove min as that is the lowest point word in the list
            longestWords.insert(length, newWord);
            if (longestWords.size() > 20) {
               minLength = longestWords.removeMin().getValue().getWord().length();
            }
         }
      }

      // For each neighbor that hasn't been visited, check if the letter at that
      // tile extends a branch of the trie. If it does, continue the DFS at that tile
      if (start.getFirstChild() != null) {
         for (BoggleBoardNode neighbor : boardNodes[x][y].getNeighbors()) {
            if (neighbor.visited()) continue;

            // Get letter from neighbor's board tile
            final char letter = board[neighbor.getX()][neighbor.getY()];

            // Check if letter is a child of current trie node. If so,
            // continue the DFS search at that child node
            Trie.Node current = start.getFirstChild();
            while (current != null) {
               if (current.getData() == letter) {
                  search(longestWords, current, path, board, neighbor.getX(), neighbor.getY());
                  break;
               }
               current = current.next();
            }
         }
      }

      // Unvisit the board node, as this tile will no longer be visited
      // in the composition of the recursive call
      boardNodes[x][y].unvisit();

      // Remove the last letter of the word so that the word will be the same
      // as it was in the composition (e.g. parent search was eval, this one was evalu,
      // set word back to eval so that the parent search is using the correct word)
      length--;

      // Remove this location from the path, as the search is no longer moving in this direction
      path.remove(path.size() - 1);
   }

   private ArrayList<BoggleBoardNode> findValidNeighbors(int x, int y) {
      // Create list of neighbors to assign to the board node
      ArrayList<BoggleBoardNode> neighbors = new ArrayList<>(8);

      // Really ugly way of checking all possible 8 positions around the current location
      for (int i = -1; i <= 1; i++) {
         for (int j = -1; j <= 1; j++) {
            if (x + i < 0 || x + i >= 4 || y + j < 0 || y + j >= 4 || (i == 0 && j == 0)) {
               continue;
            } else {
               // Valid position, add it to the list of available neighbors
               neighbors.add(boardNodes[x + i][y + j]);
            }
         }
      }

      // Return a list of all valid neighbors
      return neighbors;
   }

   // Fix the letter 'Q' to be 'QU' in every word
   // as 'QU' is represented as 'Q' in Boggle
   public static String fixQ (final String original) {
      String result = "";
      // Add each letter to the resultant string, modifying 'Q' to be 'QU' 
      // if it is encountered in the original word
      for (int i = 0; i < original.length(); i++) {
         result += original.charAt(i) == 'Q' ? "QU" : original.charAt(i);
      }

      // Return fixed string
      return result;
   }

   public static void clearMemory () {
      Runtime run = Runtime.getRuntime();
      run.gc();

      List<MemoryPoolMXBean> pools = ManagementFactory.getMemoryPoolMXBeans();
      for (MemoryPoolMXBean memoryPoolMXBean : pools)
      {
         if (memoryPoolMXBean.getType() == MemoryType.HEAP)
         {
            memoryPoolMXBean.resetPeakUsage();
         }
      }

      run.gc();
   }
}

// Class for representing each node on the 4x4 Boggle grid
class BoggleBoardNode {
   private int x;
   private int y;
   private ArrayList<BoggleBoardNode> neighbors = new ArrayList<>(8);
   private boolean visited;

   // Constructor, assigning the board node's position
   public BoggleBoardNode(int xPos, int yPos) {
      x = xPos;
      y = yPos;
   }

   // Getter for the list of neighbors of the board
   public ArrayList<BoggleBoardNode> getNeighbors() {
      return neighbors;
   }

   // Setter to set the list of neighbors of the board
   public void setNeighbors(ArrayList<BoggleBoardNode> neighborsToUse) {
      neighbors = neighborsToUse;
   }

   // Functions to visit, unvisit, and check if the tile is visited
   public boolean visited() { return visited; }
   public void visit() { visited = true; }
   public void unvisit() { visited = false; }

   // Getters for position of board node
   public int getX () { return x; }
   public int getY () { return y; }
}
