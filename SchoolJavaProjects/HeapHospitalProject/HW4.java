/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 
This file is the main file used to solve the assignment given in
HW4, where the goal is to use a heap to manage the patients
in a hospital to determine which patients should be operated on
first by doctors or treated by nurses depending on the severity
of their injuries and the time at which the patient arrived.
*/

// Import necessary libraries
import java.util.Scanner;
import java.io.File;
import java.io.IOException;
import java.nio.file.Path;
import java.nio.file.Paths;

public class HW4 {

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
         }
      }

      // Cycle through the remaining patients in the heap until
      // there are no more patients to be treated in the hospital.
      hospital.FinishDay();
      
   }
}