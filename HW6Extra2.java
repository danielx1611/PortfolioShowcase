/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 

- This file is the main file for the hw6 assignment.
- Data is read from an input file depicting a Pacman
- game, and the code prompts a user to move Pacman.
- After Pacman moves, the grid is reprinted and the
- Ghosts' pathways to get to Pacman are printed. Instead
- of using breadth first search to find the path,
- this version uses Dijkstra's algorithm to pathfind.
- A dot-dot connection is 1 weight, dot-empty is 2,
- and empty-empty is 3.

*/

// Import necessary libraries
import java.util.ArrayList;
import java.util.Scanner;
import java.io.File;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;

public class HW6Extra2 {

   @SuppressWarnings("unchecked")
   public static void main(String[] args) throws IOException {
      // Assign the file path for reading tree queries to the second command line argument
      final Path path = Paths.get(args[0]);

      // Create a scanner to read over the query file
      final Scanner fileReader = new Scanner(path, "US-ASCII");
      fileReader.useDelimiter(" |\n");

      // Read the dimensions of the grid on the first line
      String firstLine = fileReader.nextLine();
      String[] coords = firstLine.split(" ");

      // Set width and height variables to set size of play area grid
      int width = Integer.parseInt(coords[1]) - 2;
      int height = Integer.parseInt(coords[0]) - 2;
      if (width <= 0 || height <= 0) {
         System.out.println("Invalid Play Area Dimensions");
         fileReader.close();
         return;
      }

      // Skip number count of grid and list of walls
      fileReader.nextLine();
      fileReader.nextLine();

      // Initialize grid to store all of the data for each square on the board
      DijkstraNode<Character>[][] grid = new DijkstraNode[width][height];

      // Initialize pacPosition to determine where Pacman is on the grid
      int pacPositionX = -1;
      int pacPositionY = -1;

      // Initialize score variables to determine how many point user has or needs to win
      int maxScore = 0;
      int points = 0;

      // Create an arraylist to store all of the ghosts on the grid
      ArrayList<Ghost> ghosts = new ArrayList<Ghost>();

      // Default "infinity" dValue value for doing dijkstra's algorithm
      int defaultInf = Integer.MAX_VALUE;

      // Read every line of input and process the information on each line
      for (int curHeight = 0; curHeight < height; curHeight++) {

         // Read the current line in the input and ensure it is not blank.
         String line = fileReader.nextLine();
         if (line.isEmpty()) {
            fileReader.close();
            return;
         }

         // Read every square, skipping the left and right border X's on the input
         String gridRow = line.substring(2, width + 2);
         
         // Read every square on the line and add it to the grid
         for (int i = 0; i < gridRow.length(); i++) {
            // Add new node and assign it to current position in the grid with dValue of infinity
            grid[i][curHeight] = new DijkstraNode<Character>(gridRow.charAt(i), null, null, null, null, i, curHeight, defaultInf);

            // Boolean to determine if a square is a ghost, for reassignment purposes
            boolean isGhost = false;
            
            // Determine what piece of data is at the current square
            char currChar = gridRow.charAt(i);

            // Choose what to do based on what character is on the square
            if (currChar == 'P') {
               // Must be Pacman, assign his position to current x and y iteration
               pacPositionX = i;
               pacPositionY = curHeight;
            } else if (currChar == '.') {
               // A dot for Pacman to eat, increase score needed to win
               maxScore++;
            } else if (currChar != '#') {
               // Check for ghosts. If not a ghost or obstacle (#), must be an invalid entry on grid
               if (Character.getNumericValue(currChar) >= 10 &&  Character.getNumericValue(currChar) <= 35) {
                  
                  // Add a new ghost, and determine if the ghost should be covering a dot if the letter is capitalized
                  ghosts.add(new Ghost(Character.toUpperCase(currChar), Character.toLowerCase(currChar),
                        Character.toLowerCase(currChar) == currChar ? false : true, i, curHeight));
                  
                  // If ghost is on dot, increase score needed to win
                  if (Character.toUpperCase(currChar) == currChar) maxScore++;

                  // Ghosts act as a obstacle, so they should be visited
                  grid[i][curHeight].setVisited(true);

                  // Mark tile as a ghost and set its proper value on the grid for weighting purposes
                  grid[i][curHeight].setElement(ghosts.get(ghosts.size() - 1).checkForDot() ? '.' : ' ');
                  isGhost = true;
               } else if (currChar != ' ') {
                  // If not an empty space, must be an invalid/illegal character on grid
                  System.out.println("Invalid Character: " + currChar);
                  fileReader.close();
                  return;
               }
            }

            // Assign next, prev, above, and below pointers for current square on grid
            // Also assign weightings based on square-square connection
            if (i > 0) {
               grid[i][curHeight].setPrev(grid[i - 1][curHeight]);
               grid[i - 1][curHeight].setNext(grid[i][curHeight]);

               // Special weighting decision for ghost tiles
               if (grid[i - 1][curHeight].getElement() != '.' && grid[i - 1][curHeight].getElement() != ' '
                     && grid[i - 1][curHeight].getElement() != '#' && grid[i - 1][curHeight].getElement() != 'P') {
                  // Determine weighting for ghosts
                  for (Ghost ghost : ghosts) {
                     if (ghost.getPosX() == i - 1 && ghost.getPosY() == curHeight && grid[i][curHeight].getElement() == '.') {
                        grid[i][curHeight].setWeightPrev(ghost.checkForDot() ? 1 : 2);
                        grid[i - 1][curHeight].setWeightNext(ghost.checkForDot() ? 1 : 2);
                     } else if (ghost.getPosX() == i - 1 && ghost.getPosY() == curHeight && grid[i][curHeight].getElement() == ' ') {
                        grid[i][curHeight].setWeightPrev(ghost.checkForDot() ? 2 : 3);
                        grid[i - 1][curHeight].setWeightNext(ghost.checkForDot() ? 2 : 3);
                     }
                  }
               // Weighting decision for non-ghost tiles
               } else if (grid[i][curHeight].getElement() == '.' && grid[i - 1][curHeight].getElement() == '.') {
                  grid[i][curHeight].setWeightPrev(1);
                  grid[i - 1][curHeight].setWeightNext(1);
               } else if (grid[i][curHeight].getElement() == '.' || grid[i - 1][curHeight].getElement() == '.') {
                  grid[i][curHeight].setWeightPrev(2);
                  grid[i - 1][curHeight].setWeightNext(2);
               } else {
                  grid[i][curHeight].setWeightPrev(3);
                  grid[i - 1][curHeight].setWeightNext(3);
               }
            }
            if (curHeight > 0) {
               grid[i][curHeight].setAbove(grid[i][curHeight - 1]);
               grid[i][curHeight - 1].setBelow(grid[i][curHeight]);

               // Special weighting decision for ghost tiles
               if (grid[i][curHeight - 1].getElement() != '.' && grid[i][curHeight - 1].getElement() != ' '
                     && grid[i][curHeight - 1].getElement() != '#' && grid[i][curHeight - 1].getElement() != 'P') {
                  // Determine weighting for ghosts
                  for (Ghost ghost : ghosts) {
                     if (ghost.getPosX() == i && ghost.getPosY() == curHeight - 1 && grid[i][curHeight].getElement() == '.') {
                        
                        grid[i][curHeight].setWeightAbove(ghost.checkForDot() ? 1 : 2);
                        grid[i][curHeight - 1].setWeightBelow(ghost.checkForDot() ? 1 : 2);
                     } else if (ghost.getPosX() == i && ghost.getPosY() == curHeight - 1 && grid[i][curHeight].getElement() == ' ') {
                        grid[i][curHeight].setWeightAbove(ghost.checkForDot() ? 2 : 3);
                        grid[i][curHeight - 1].setWeightBelow(ghost.checkForDot() ? 2 : 3);
                     }
                  }
               // Weighting decision for non-ghost tiles
               } else if (grid[i][curHeight].getElement() == '.' && grid[i][curHeight - 1].getElement() == '.') {
                  grid[i][curHeight].setWeightAbove(1);
                  grid[i][curHeight - 1].setWeightBelow(1);
               } else if (grid[i][curHeight].getElement() == '.' || grid[i][curHeight - 1].getElement() == '.') {
                  grid[i][curHeight].setWeightAbove(2);
                  grid[i][curHeight - 1].setWeightBelow(2);
               } else {
                  grid[i][curHeight].setWeightAbove(3);
                  grid[i][curHeight - 1].setWeightBelow(3);
               }
            }
            if (grid[i][curHeight].getElement() == '#') {
               // Obstacles should be marked as visited
               grid[i][curHeight].setVisited(true);
            }
            if (isGhost) {
               // Ghost obstacles should be marked as visited
               Ghost curGhost = ghosts.get(ghosts.size() - 1);
               grid[i][curHeight].setElement(curGhost.checkForDot() ? curGhost.getDotName() : curGhost.getNoDotName());
            }
         }
      }
      // Print grid to show user the world before any moves are calculated
      PrintGrid(grid, width, height);
      fileReader.close();

      // Create a new scanner to read user input
      Scanner in = new Scanner(System.in);

      // Initialize boolean to determine if player can move in the given direction
      boolean validMovement = false;

      // Initialize variables to determine where Pacman should move to
      int moveX = 0;
      int moveY = 0;

      // Loop until user inputs a valid/allowed direction
      while (!validMovement) {
         moveX = 0; 
         moveY = 0;

         // Prompt for user input and store it as a String
         System.out.println();
         System.out.print("Please enter your move [u(p), d(own), l(eft), r(ight)]: ");
         String direction = in.nextLine();
         
         // Determine where Pacman should move based on user input
         switch (direction) {
            case "u":
               moveY = -1;
               break;
            case "d":
               moveY = 1;
               break;
            case "l":
               moveX = -1;
               break;
            case "r":
               moveX = 1;
               break;
            default:
               // Not u,d,l,r. Invalid input
               System.out.println("Invalid Direction");
               break;
         }

         // Check if move is out of bounds or if movement is a barrier
         if (CanMoveToLocation(grid, pacPositionX + moveX, pacPositionY + moveY)) {
            // Movement is valid
            validMovement = true;
         } else {
            if (!CanMoveToLocation(grid, pacPositionX, pacPositionY + 1) && !CanMoveToLocation(grid, pacPositionX, pacPositionY - 1)
                  && !CanMoveToLocation(grid, pacPositionX - 1, pacPositionY) && !CanMoveToLocation(grid, pacPositionX + 1, pacPositionY)) {
               
               // Pacman is cornered, end game
               System.out.println();
               System.out.println("Points: " + points);
               System.out.println("Nowhere for Pacman to go. GAME OVER!");
               return;
            } else {
               // There is at least one other direction Pacman can move to
               System.out.println();
               System.out.println("Cannot move in that direction");
            }
         }
      }
      
      // Update grid and calculate points
      if (grid[pacPositionX + moveX][pacPositionY + moveY].getElement() == '.') {
         points++;
      }

      // Move Pacman, making his previous location empty, as he would have eaten any dots present
      grid[pacPositionX][pacPositionY].setElement(' ');
      pacPositionX += moveX;
      pacPositionY += moveY;
      grid[pacPositionX][pacPositionY].setElement('P');

      // Update weightings near Pacman to reflect new square-square weightings
      UpdatePacWeights(grid, pacPositionX, pacPositionY, ghosts);

      // Reset Pacman movement values just in case, safety purposes
      moveX = 0;
      moveY = 0;

      // Sort ghosts into alphabetical (GHOS) order
      ghosts.sort((o1, o2) -> o1.getDotName() - o2.getDotName());

      // Move ghost
      for (Ghost ghost : ghosts) {

         // Create a new instance of the Dijkstra search algorithm to help ghost search for Pacman
         PacmanDijkstra pathFinder = new PacmanDijkstra(grid[ghost.getPosX()][ghost.getPosY()]);
         
         // Calculate shortest path and calculated DValue to Pacman using udlr order
         PacmanDijkstra.DijkstraPathAndDValue pathAndDVal = pathFinder.findShortestPath('P');

         // Extract path from pathAndDVal variable
         ArrayList<DijkstraNode<Character>> pathToPacman = pathAndDVal.path;

         // Assign resulting path and DVal to ghost
         ghost.setPath(pathToPacman);
         ghost.setPathDVal(pathAndDVal.dValue);

         // If there is a path to Pacman, determine which direction the ghost would move in
         if (pathToPacman.size() != 0) {
            if (pathToPacman.get(pathToPacman.size() - 2).getPosX() > ghost.getPosX()) {
               ghost.setLastMovedDirection('r');
            } else if (pathToPacman.get(pathToPacman.size() - 2).getPosX() < ghost.getPosX()) {
               ghost.setLastMovedDirection('l');
            } else if (pathToPacman.get(pathToPacman.size() - 2).getPosY() < ghost.getPosY()) {
               ghost.setLastMovedDirection('u');
            } else {
               ghost.setLastMovedDirection('d');
            }
            
            // Ensure ghost's position is visited so that it acts as obstacle for other ghosts
            grid[ghost.getPosX()][ghost.getPosY()].setVisited(true);

         } else {
            // Ghost does not know where to go, cannot move in any direction
            ghost.setLastMovedDirection(' ');
         }
      }

      // Print updated grid
      PrintGrid(grid, width, height);
      
      // Print # of points
      System.out.println();
      System.out.println("Points: " + points);

      // Print ghost paths
      for (Ghost ghost : ghosts) {
         // If ghost has invalid path, it cannot move, otherwise, print path size
         if (ghost.getLastMovedDirection() == ' ') {
            System.out.print("Ghost " + ghost.getDotName() + ": Cannot move");
         } else {
            System.out.print("Ghost " + ghost.getDotName() + ": " + ghost.getLastMovedDirection() + " " + ghost.getPathDVal() + " ");
         }

         // Print each step of the path
         for (int i = ghost.getPath().size() - 1; i >= 0; i--) {
            ArrayList<DijkstraNode<Character>> ghostPath = ghost.getPath();
            System.out.print("(" + (ghostPath.get(i).getPosY() + 1)  + "," + (ghostPath.get(i).getPosX() + 1) + ") ");
         }
         System.out.println();
      }

      // Pacman wins
      if (points == maxScore) {
         System.out.println("Pac-man is full!");
      }

      in.close();
   }

   // Update weightd around Pacman given the grid and Pacman's current location and the list of ghosts
   private static void UpdatePacWeights(DijkstraNode<Character>[][] grid, int xPos, int yPos, ArrayList<Ghost> ghosts) {
      // Readjust weight for above
      if (yPos - 1 >= 0) {
         if (grid[xPos][yPos - 1].getElement() != '#') {
            int result = CalculatePacWeight(grid[xPos][yPos - 1], ghosts);
            grid[xPos][yPos].setWeightAbove(result);
            grid[xPos][yPos - 1].setWeightBelow(result);
         }
      } 
      // Readjust weight for below
      if (yPos + 1 < grid[0].length) {
         if (grid[xPos][yPos + 1].getElement() != '#') {
            int result = CalculatePacWeight(grid[xPos][yPos + 1], ghosts);
            grid[xPos][yPos].setWeightBelow(result);
            grid[xPos][yPos + 1].setWeightAbove(result);
         }
      } 
      // Readjust weight for previous
      if (xPos - 1 >= 0) {
         if (grid[xPos - 1][yPos].getElement() != '#') {
            int result = CalculatePacWeight(grid[xPos - 1][yPos], ghosts);
            grid[xPos][yPos].setWeightPrev(result);
            grid[xPos - 1][yPos].setWeightNext(result);
         }
      } 
      // Readjust weight for next
      if (xPos + 1 < grid.length) {
         if (grid[xPos + 1][yPos].getElement() != '#') {
            int result = CalculatePacWeight(grid[xPos + 1][yPos], ghosts);
            grid[xPos][yPos].setWeightNext(result);
            grid[xPos + 1][yPos].setWeightPrev(result);
         }
      }
      System.out.println();
   }

   // Condensed function to determine what the new weight of a tile should be
   // given the node and the list of ghosts
   public static int CalculatePacWeight(DijkstraNode<Character> other, ArrayList<Ghost> ghosts) {
      // Pacman-dot weight is 2
      if (other.getElement() == '.') {
         return 2;
      } else {
         // Ghost-square weight dependent on if it is covering a dot
         for (Ghost ghost : ghosts) {
            if (ghost.getPosX() == other.getPosX() && ghost.getPosY() == other.getPosY()) {
               return ghost.checkForDot() ? 2 : 3;
            }
         }
         // Empty-Pacman connection weight is 3
         return 3;
      }
   }

   // Determine if a move is valid
   private static boolean CanMoveToLocation(DijkstraNode<Character>[][] grid, int xPos, int yPos) {
      // Input validation
      if (grid == null || grid.length == 0) {
         return false;
      }
      // Check if move is out of bounds or if movement is a barrier
      if (xPos >= grid.length || xPos < 0) {
         return false;
      } else if (yPos >= grid[0].length || yPos < 0) {
         return false;
      } else if(grid[xPos][yPos].getElement() != '.' &&
               grid[xPos][yPos].getElement() != ' ') {
         return false;
      } else {
         // Movement must be valid
         return true;
      }
   }

   // Print current grid state to console
   private static void PrintGrid(DijkstraNode<Character>[][] grid, int width, int height) {
      // Print column numbers
      System.out.print(" ");
      for (int i = 0; i < width + 2; i++) {
         System.out.print(i);
      }
      System.out.println();

      // Print first top border line
      System.out.print("0");
      for (int i = 0; i < width + 2; i++) {
         System.out.print("#");
      }
      System.out.println();
      
      // Print all lines between top and bottom borders
      for (int j = 1; j <= height; j++) {
         System.out.print(j + "#");
         for (int i = 0; i < width; i++) {
            System.out.print(grid[i][j-1].getElement());
         }
         System.out.println("#");
      }

      // Print bottom border line
      System.out.print(height + 1);
      for (int i = 0; i < width + 2; i++) {
         System.out.print("#");
      }
      System.out.println();
   }

   // Custom ghost class to represent ghosts in the Pacman game
   private static class Ghost {
      char dotName;
      char noDotName;
      boolean isOnDot;
      int posX;
      int posY;
      char lastMovedDirection;
      ArrayList<DijkstraNode<Character>> path;
      int pathDVal;

      public Ghost(char _dotName, char _noDotName, boolean _isOnDot, int _posX, int _posY) {
         dotName = _dotName;
         noDotName = _noDotName;
         isOnDot = _isOnDot;
         posX = _posX;
         posY = _posY;
         lastMovedDirection = ' ';
         path = new ArrayList<DijkstraNode<Character>>();
      }

      // Getters for all attributes
      public char getDotName() { return dotName; }
      public char getNoDotName() { return noDotName; }
      public boolean checkForDot() { return isOnDot; }
      public int getPosX() { return posX; }
      public int getPosY() { return posY; }
      public char getLastMovedDirection() { return lastMovedDirection; }
      public ArrayList<DijkstraNode<Character>> getPath() { return path; }
      public int getPathDVal() { return pathDVal; }

      // Setters for most attributes
      public void setOnDot(boolean isOn) { isOnDot = isOn; }
      public void setX(int x) { posX = x; }
      public void setY(int y) { posY = y; }
      public void setLastMovedDirection(char direction) { lastMovedDirection = direction; }
      public void setPath(ArrayList<DijkstraNode<Character>> newPath) { path = newPath; }
      public void setPathDVal(int dVal) { pathDVal = dVal; }
   }
}