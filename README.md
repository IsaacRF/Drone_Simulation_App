# DRONE SIMULATION APP
- **Author:** Isaac R.F. (isaacrf239@gmail.com)
- **Test owner company:** Derivco
- **Language:** C#, .Net
- **Test Specifications:** [Link](https://github.com/IsaacRF/Drone_Simulation_App/blob/master/Drone_Simulation_Specifications.docx)

## Description
Multithread Console Application simulating traffic control drones navigating through a setted route, and controlled by a central dispatcher. Each drone runs on a separate thread, and reports the zone traffic conditions if a Tube Station (csv list provided) is near.

## Project parts
1. Executable is located on bin/Release/Drone_Simulation_App.exe along a folder called Data, where all the csv files are located.
2. Unit tests are located under UnitTesting folder.

## Execution
1. The simulation can be ran in real world speed (1) or accelerated speed (2) to see results on console 10 times
faster.
2. Simulation automatically ends at 8:10 (relative time from route points)
3. Pressing ENTER again manually sends a shut down signal to all drones. Pressing ENTER another time closes the application
