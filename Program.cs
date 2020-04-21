using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drone_Simulation_App.Classes;

namespace Drone_Simulation_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("####### Welcome to the drone simulation program #######");
            Console.WriteLine("Please enter an option:");
            Console.WriteLine("1.- Start drone simulation (Press ENTER again to send shut down signal)");
            Console.WriteLine("0.- Exit simulation");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Starting simulation...");
                    Dispatcher dispatcher = new Dispatcher();
                    dispatcher.LoadAllDronesRoute();
                    dispatcher.StartDronesNavigationRoutine();

                    Console.ReadLine();
                    dispatcher.ShutDownDrones();
                    break;
                default:
                    break;
            }

            
            Console.ReadLine();
        }
    }
}
