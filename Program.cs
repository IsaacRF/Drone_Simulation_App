using System;
using System.IO;
using Drone_Simulation_App.Classes;

namespace Drone_Simulation_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("####### Welcome to the drone simulation program #######");
            Console.WriteLine("NOTE: Simulation ends automatically when whichever drone reach a point with time 8:10. Press ENTER again anytime to send shut down signal manually");
            Console.WriteLine("#######################################################");
            Console.WriteLine("Please enter an option:");
            Console.WriteLine("1.- Start drone simulation (Real time speed)");
            Console.WriteLine("2.- Start drone simulation (Speed Up)");
            Console.WriteLine("0.- Exit simulation");
            Console.WriteLine("#######################################################");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                case "2":
                    Console.WriteLine("Starting simulation...");
                    try
                    {
                        bool speedUp = (option == "2");
                        Dispatcher dispatcher = new Dispatcher(speedUp);
                        dispatcher.LoadAllDronesRoute();
                        dispatcher.StartDronesNavigationRoutine();

                        Console.ReadLine();
                        dispatcher.ShutDownDrones();
                    }
                    catch (InvalidDataException exc)
                    {
                        Console.WriteLine("Error initializing simulation, program will be terminated. Error info:");
                        Console.WriteLine(exc.Message);
                        Console.ReadLine();
                        return;
                    }                    
                    break;
                default:
                    break;
            }

            
            Console.ReadLine();
        }
    }
}
