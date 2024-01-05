/*
Author: Daniel Acosta
Email: dacosta2022@my.fit.edu
Course: CSE2010
Section: 2
Description of this file: 
This file is used to store all of the Doctor, Nurse, and 
Patient objects used to construct and modify the heap. It
also performs all of the calculations necessary to determine
which doctor/nurse should treat a patient as well as when
they will finish their treatments. This file has built in
functions to manage the HHMM time format used in the input files.
*/

// Import necessary libraries
import java.util.ArrayList;
import java.util.HashMap;

public class HeapHospital {

   // Create int to keep track of the time in the hospital
   private int currentGlobalTime;

   // Create a HeapPriorityQueue to store the patients in a heap, relying on the
   // custom PatientHeapKey key and the Patient class as a value
   private HeapPriorityQueue<PatientHeapKey, Patient> patientHeap;

   // Set up ArrayLists to store the available/busy doctors and nurses
   private ArrayList<Doctor> availableDoctors = new ArrayList<Doctor>();
   private ArrayList<Doctor> busyDoctors = new ArrayList<Doctor>();
   private ArrayList<Nurse> availableNurses = new ArrayList<Nurse>();
   private ArrayList<Nurse> busyNurses = new ArrayList<Nurse>();
   
   // Array to map patient name values in heap to their given entry to reduce search time
   private InsertSortedHeapEntryArray stringMapper = new InsertSortedHeapEntryArray();

   // Constructor to instantiate a new hospital given a time indicating the start of the day
   public HeapHospital(int startTime) {
      currentGlobalTime = startTime;
      patientHeap = new HeapPriorityQueue<PatientHeapKey, Patient>();
      availableDoctors.add(new Doctor(startTime, "Alice"));
      availableDoctors.add(new Doctor(startTime, "Bob"));
      availableNurses.add(new Nurse(""));
      availableNurses.add(new Nurse(""));
   }

   // Doctor class to keep track of when a doctor arrived, their name,
   // the time left until they finish their operation, and their patient
   public class Doctor {
      private int timeArrived;
      private String name;
      private int operationTime;
      private Patient patient;

      // Constructor for a doctor given when they arrived and their name
      public Doctor(int _timeArrived, String _name) {
         timeArrived = _timeArrived;
         name = _name;
      }

      // Getter/setters for various Doctor attributes
      public int getTimeArrived() {return timeArrived;}

      public String getName() {return name;}

      public int getOperationTime() { return operationTime;}

      public void setOperationTime(int newTime) {operationTime = newTime;}

      public void reduceOperationTime(int timeToReduce) {operationTime -= timeToReduce;}

      public Patient getPatient() {return patient;}

      public void setPatient(Patient newPatient) {patient = newPatient;}
   }

   // Nurse class to keep track of who their patient is and how much time
   // is left until they are done treating the patient
   public class Nurse {
      private String patientName;
      private int operationTime;

      // Constructor for a nurse given the patient's name
      public Nurse(String _patientName) {patientName = _patientName;}

      // Getter/setters for various Nurse attributes
      public String getPatientName() {return patientName;}

      public void SetPatientName(String newName) {patientName = newName;}

      public int getOperationTime() {return operationTime;}

      public void setOperationTime(int newTime) {operationTime = newTime;}

      public void reduceOperationTime(int timeToReduce) {operationTime -= timeToReduce;}
   }

   // Patient class to keep track of their name which is unmodifiable
   public class Patient {
      private String name;

      // Constructor for a patient given a name
      public Patient(String _name) {name = _name;}

      // Getter to return the patient's name, no setters allowed
      public String getName() {return name;}
   }

   // Custom key class used as a key for the entries in the HeapPriorityQueue
   public class PatientHeapKey implements Comparable {
      private int esi;
      private int timeArrived;
      
      // Constructor for the PatientHeapKey given a patient's ESI level and
      // the time they arrived to the hospital
      public PatientHeapKey(int _esi, int _timeArrived) {
         esi = _esi;
         timeArrived = _timeArrived;
      }

      // Getter setters for various PatientHeapKey attributes
      public int getESI() {return esi;}

      public void setESI(int newESI) {esi = newESI;}

      public int getTimeArrived() {return timeArrived;}

      public void setTimeArrived(int newTime) {timeArrived = newTime;}

      // compareTo method that enables the keys to be sorted in the HeapPQ,
      // where a lower ESI is higher priority and an earlier time is a higher priority.
      @Override
      public int compareTo(Object otherKey) {
         PatientHeapKey other = (PatientHeapKey) otherKey;
         if (esi < other.getESI()) {
            return -1;
         } else if (esi > other.getESI()) {
            return 1;
         } else {
            if (timeArrived < other.getTimeArrived()) {
               return -1;
            } else if (timeArrived > other.getTimeArrived()) {
               return 1;
            } else {
               return 0;
            }
         }
      }

      // Special case to find a patient with a given esi.
      // Since every timestamp is unique, this will return true
      // as long as the two patients are of the same esi.
      @Override
      public boolean equals(Object obj) {
         PatientHeapKey other = (PatientHeapKey) obj;
         return esi == other.getESI();
      }
   }

   // Continue the day until the given time, then add the new patient with their given esi to the heap
   public void ProgressDay(int nextTime, String patientName, int esi) {

      // Calculate all of the doctor and nurse treatments up until the patient's arrival
      CalculatePatientTreatments(nextTime, false);

      // The new patient has arrived.
      // Create a new entry for the heap and insert it. Then assign a hashvalue to keep track of which patient's
      // name belongs to the newly created entry in the heap for quick retrieval.
      HeapPriorityQueue.HeapEntry entry = patientHeap.insert(new PatientHeapKey(esi, nextTime), new Patient(patientName));
      stringMapper.insert(patientName, entry);

      // Print to console that the new patient has arrived.
      System.out.println("PatientArrives " + FormatTime(currentGlobalTime) + " " + patientName + " " + esi);
   }

   // Calculate the remaining doctor/nurse treatments of the patients still in the heap after there is
   // no more input being received.
   public void FinishDay() {
      CalculatePatientTreatments(0, true);
   }

   // Process to calculate Doctor/Nurse treatment of patients up until a given time,
   // with a boolean to determine if the heap should instead be fully emptied and independent of said time
   private void CalculatePatientTreatments(int nextTime, boolean shouldEmptyHeap) {
      // While the process depends on a later time, keep looping until the currentGlobalTime has reached the desired time.
      // Otherwise, keep looping until the heap is empty and there are no busy doctors/nurses
      while (shouldEmptyHeap ? patientHeap.size() > 0 || busyDoctors.size() > 0 || busyNurses.size() > 0
            : currentGlobalTime < nextTime) {

         // While there are available doctors and patients to treat,
         // set the doctor to begin treating the patient
         while (availableDoctors.size() > 0 && patientHeap.size() > 0) {
            // Get next available doctor (like a stack structure)
            Doctor nextAvailableDoctor = availableDoctors.get(availableDoctors.size() - 1);

            // Take the highest priority patient from the list
            Entry<PatientHeapKey, Patient> nextPatient = patientHeap.removeMin();

            // Remove the patient from the mapper as they are no longer in the heap
            stringMapper.remove(nextPatient.getValue().getName());

            // Set the patient of the doctor to the entry's patient value
            nextAvailableDoctor.setPatient(nextPatient.getValue());

            // Set the operation time of the doctor to 2^(7-esi) seconds
            nextAvailableDoctor.setOperationTime((int)Math.pow(2, 7 - nextPatient.getKey().getESI()));

            // Print to console that the doctor has started treating the given patient
            System.out.println("DoctorStartsTreatingPatient " + FormatTime(currentGlobalTime) + " " + nextAvailableDoctor.getName() + " " + nextPatient.getValue().getName());

            // Add the doctor to the list of busy doctors, and remove them from the list of available doctors
            busyDoctors.add(nextAvailableDoctor);
            availableDoctors.remove(nextAvailableDoctor);
         }

         // While there are available nurses and esi 5 patients to treat,
         // set the nurse to begin treating the patient
         while (availableNurses.size() > 0 && patientHeap.size() > 0) {
            // Get next available nurse (like a stack structure)
            Nurse nextAvailableNurse = availableNurses.get(availableNurses.size() - 1);

            // Create a new key to search for an ESI 5 patient
            PatientHeapKey esi5key = new PatientHeapKey(5, 0);

            // Find the index of the highest priority ESI 5 patient
            int result = patientHeap.FindHighestPriorityKeyVariant(esi5key);

            // If there is an ESI 5 patient, have them get treated by a nurse
            if (result != -1) {
               // Take the highest priority ESI 5 patient from the heap
               Entry<PatientHeapKey, Patient> nextPatient = patientHeap.removeAtIndex(result);

               // Remove the patient from the mapper as they are no longer in the heap
               stringMapper.remove(nextPatient.getValue().getName());

               // Set the nurse's patient to the patient that was found in the heap
               nextAvailableNurse.SetPatientName(nextPatient.getValue().getName());

               // Set the nurse's operation time to 10 minutes, as that is the standard
               // time required for a nurse to treat an ESI 5 patient
               nextAvailableNurse.setOperationTime(10);

               // Add the nurse to list of busy nurses and remove them from the list of available nurses.
               busyNurses.add(nextAvailableNurse);
               availableNurses.remove(nextAvailableNurse);
            } else {
               // If there are no ESI 5 patients, end the while loop
               break;
            }
         }

         // If there is at least one busy doctor or nurse, calculate how much time should pass until
         // either they finish their operation or the nextTime limit has been reached
         if (busyDoctors.size() > 0 || busyNurses.size() > 0) {

            // Set the initial value of time to pass to the operation time of 
            // the first doctor or the first nurse if there is no doctor.
            int timeToPass = busyDoctors.size() > 0 ? busyDoctors.get(0).getOperationTime() : busyNurses.get(0).getOperationTime();

            // For each busy doctor, calculate which one has the minimum amount of time needed to finish their operation
            for (Doctor doctor : busyDoctors) {
               int opTime = doctor.getOperationTime();
               if (opTime < timeToPass) {
                  timeToPass = opTime;
               }
            }

            // For each busy nurse, calculate which one has the minimum amount of time needed to finish their operation
            for (Nurse nurse : busyNurses) {
               int opTime = nurse.getOperationTime();
               if (opTime < timeToPass) {
                  timeToPass = opTime;
               }
            }

            // If the minimum operation time is longer than the amount of time until the nextTime limit is reached,
            // set the minimum operation time to the amount of time until the nextTime limit is reached.
            if (timeToPass > SubtractTimes(nextTime, currentGlobalTime) && !shouldEmptyHeap) {
               timeToPass = SubtractTimes(nextTime, currentGlobalTime);
            }

            // For each doctor, determine if they will complete their surgery after the set amount
            // of time passes. Then, add them to a list of doctors that should be freed up
            ArrayList<Doctor> freeDoctors = new ArrayList<Doctor>();
            for (Doctor doctor : busyDoctors) {
               // Lower their operation time by the amount of time that should pass
               doctor.reduceOperationTime(timeToPass);

               // If a doctor finishes their operation, add them to the list of available doctors
               // and mark them as a doctor that should be freed up.
               if (doctor.getOperationTime() == 0) {
                  availableDoctors.add(doctor);
                  freeDoctors.add(doctor);
               }
            }

            // For each nurse, determine if they will complete their treatment after the set amount
            // of time passes. Then, add them to a list of nurses that should be freed up
            ArrayList<Nurse> freeNurses = new ArrayList<Nurse>();
            for (Nurse nurse : busyNurses) {
               // Lower their operation time by the amount of time that should pass
               nurse.reduceOperationTime(timeToPass);

               // If a nurse finishes their operation, add them to the list of available nurses
               // and mark them as a nurse that should be freed up.
               if (nurse.getOperationTime() == 0) {
                  availableNurses.add(nurse);
                  freeNurses.add(nurse);
               }
            }

            // Add the time that has passed to the currentGlobalTime variable and accomodate for
            // integer overflows of the time
            currentGlobalTime = CorrectTimeOverflows(currentGlobalTime, timeToPass);

            // For each freed up doctor, indicate that they have finished treatment,
            // then remove them from the list of busy doctors.
            for (Doctor doctor : freeDoctors) {
               System.out.println("DoctorFinishesTreatmentAndPatientDeparts " + FormatTime(currentGlobalTime) + " " + doctor.getName() + " " + doctor.getPatient().getName());

               // Pick up a new patient
               if (patientHeap.size() > 0) {
                  // Take the highest priority patient from the list
                  Entry<PatientHeapKey, Patient> nextPatient = patientHeap.removeMin();

                  // Doctor is no longer available
                  availableDoctors.remove(doctor);

                  // Remove the patient from the mapper as they are no longer in the heap
                  stringMapper.remove(nextPatient.getValue().getName());

                  // Set the patient of the doctor to the entry's patient value
                  doctor.setPatient(nextPatient.getValue());

                  // Set the operation time of the doctor to 2^(7-esi) seconds
                  doctor.setOperationTime((int)Math.pow(2, 7 - nextPatient.getKey().getESI()));

                  // Print to console that the doctor has started treating the given patient
                  System.out.println("DoctorStartsTreatingPatient " + FormatTime(currentGlobalTime) + " " + doctor.getName() + " " + nextPatient.getValue().getName());
               } else {
                  busyDoctors.remove(doctor);
               }
            }

            // For each freed up nurse, indicate that they have finished treatment,
            // then remove them from the list of busy nurses.
            for (Nurse nurse : freeNurses) {
               System.out.println("PatientDepartsAfterNurseTreatment " + FormatTime(currentGlobalTime) + " " + nurse.getPatientName());
               busyNurses.remove(nurse);
            }

         } else if (busyDoctors.size() == 0 && busyNurses.size() == 0) {
            // If there are no busy doctors/nurses, then skip ahead to the indicated
            // nextTimeLimit, as no operations are currently occurring
            currentGlobalTime = nextTime;
         }
      }
   }

   // Update the patient's ESI and timeArrived values 
   public void UpdatePatientESI(int nextTime, String patientName, int newESI) {

      // Calculate any other treatments that should occur leading up to when the patient's
      // ESI needs to be updated.
      CalculatePatientTreatments(nextTime, false);

      // Find the patient based on their name from the mapper
      InsertSortedHeapEntryArray.StringToHeapEntryBind entry = stringMapper.get(stringMapper.binarySearchList(patientName));

      // Update key of the patient to new ESI
      entry.getEntry().updateKey(new PatientHeapKey(newESI, nextTime));

      // Print to console indicating that the patient's ESI has been updated
      System.out.println("UpdatePatientESI " + FormatTime(nextTime) + " " + patientName + " " + newESI);
   }

   // Add a new doctor to the list of doctors
   public void AddNewDoctor(int arrivalTime, String doctorName) {
      // Calculate all treatments leading up to the point the doctor arrives
      CalculatePatientTreatments(arrivalTime, false);

      // Add the new doctor to the list of available doctors
      availableDoctors.add(new Doctor(arrivalTime, doctorName));

      // Print to the console indicating a doctor has arrived
      System.out.println("DoctorArrives " + FormatTime(arrivalTime) + " " + doctorName);
   }

   // Function to add a given amount of minutes to the clock
   // and accomodate for any integer overflows that could occur
   private int CorrectTimeOverflows(int originalTime, int timeToAdd) {
      // Get original hours and minutes of clock
      int hours = originalTime / 100;
      int minutes = originalTime % 100;

      // Increase hours and minutes by the time to add, and use modulo
      // to keep the hours/minutes in their respective 24/60 values
      hours += ((minutes + timeToAdd) / 60);
      hours = hours % 24;
      minutes = (minutes + timeToAdd) % 60;

      // Return the new time in HHMM format
      return hours * 100 + minutes;
   }

   // Subtract two clock values to calculate the amount of minutes
   // between the two times
   private int SubtractTimes(int originalTime, int timeToSubtract) {
      // Gather the total number of minutes from both times
      int originalTimeTotal = 60 * (originalTime / 100) + originalTime % 100;
      int timeToSubtractTotal = 60 * (timeToSubtract / 100) + timeToSubtract % 100;

      // Return the difference between the two times in minutes
      // If the original time is less than the time to subtract, i.e. 1:00 vs 23:00 in HHMM format,
      // then add 24 hours worth of minutes to the original time to properly calculate the number of minutes between them
      if (timeToSubtractTotal > originalTimeTotal) {
         return (originalTimeTotal + 60 * 24) - timeToSubtractTotal;
      } else {
         return originalTimeTotal - timeToSubtractTotal;
      }
   }

   // Format the time into HHMM format given an integer time
   private String FormatTime(int time) {
      // Calculate number of leading zeroes that need to be added to fit the HHMM format
      if (time / 10 < 1) {
         return "000" + time;
      }
      if (time / 100 < 1) {
         return "00" + time;
      }
      if (time / 1000 < 1) {
         return "0" + time;
      }

      // Return the formatted time as a String
      return "" + time;
   }

}