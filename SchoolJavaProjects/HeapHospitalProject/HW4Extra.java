/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 
This file is the main file used to solve the assignment given in
HW4Extra, where the goal is to use a heap to manage the patients
in a hospital to determine which patients should be operated on
first by doctors or treated by nurses depending on the severity
of their injuries and the time at which the patient arrived.

EXTRA CREDIT:
Add a function updatePatientESI that can update the position of a patient
in the heap and change their esi severity with a new time of arrival.

Question 1: 
Explain why your additional data structure can help
UpdatePatientESI become faster than O(N) with an
analysis of the time complexity of UpdatePatientESI.
- The additional data structure is an ordered list that contains
- an object binding a given patient name string to an entry in
- the heap. Since the list is ordered, it can be navigated by using
- a binary search, which is O(log n) time. Once the correct entry is
- found, the program determines in O(1) time if the entry needs be
- upheaped or downheaped. Both upheap and downheap are O(log n) functions.
- In a worst case scenario, where the target node is the root, the process
- will take 2 log n operations, which simplifies to O(log n).

Question 2:
When UpdatePatientESI does not remove the entry
with the lowest ESI, discuss how the heap (priority
queue) needs to be adjusted.
- If the function does not remove the entry at the root,
- A.K.A. the lowest ESI, then nothing should need to change.
- The root will be updated to the proper priority, and then the program
- commands the node to undergo downheap. Since the root node is
- still a higher priority than all of the nodes below it, nothing would
- need to change to mainain the heap. The root node's left and right
- children would remain the same, only the value of the heap entry
- would be changed.

*/

// Import necessary libraries
import java.util.Scanner;
import java.io.File;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;

public class HW4Extra {

   public static void main(String[] args) throws IOException {
      // Assign the file path for reading tree queries to the second command line argument
      final Path path = Paths.get(args[0]);

      // Create a scanner to read over the query file
      final Scanner fileReader = new Scanner(path, "US-ASCII");
      fileReader.useDelimiter(" |\n");

      // Instantiate a new HeapHospital to calculate the treatment of patients
      HeapHospital hospital = new HeapHospital(0);

      // While there is input left in the file, read and process it.
      while (fileReader.hasNextLine()) {

         // Read the current line in the input and ensure it is not blank.
         String line = fileReader.nextLine();
         if (line.isEmpty()) {
            fileReader.close();
            return;
         }

         // Split the input up into each of its words to evaluate the data
         // and call the correct function.
         String[] curLine = line.split(" ");

         // Switch cases for the different input possibilities.
         switch(curLine[0]) {
            case "PatientArrives":
               // Progress the day until the patient arrives
               hospital.ProgressDay(Integer.parseInt(curLine[1]), curLine[2], Integer.parseInt(curLine[3]));
               break;
            case "DoctorArrives":
               // Add a new doctor to the hospital's roster
               hospital.AddNewDoctor(Integer.parseInt(curLine[1]), curLine[2]);
               break;
            case "UpdatePatientESI":
               // Update a patient to their new ESI
               hospital.UpdatePatientESI(Integer.parseInt(curLine[1]), curLine[2], Integer.parseInt(curLine[3]));
               break;
         }
      }

      // Cycle through the remaining patients in the heap until
      // there are no more patients to be treated in the hospital.
      hospital.FinishDay();
      
   }
}