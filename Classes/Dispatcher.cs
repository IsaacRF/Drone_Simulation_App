using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drone_Simulation_App.Classes
{
    /// <summary>
    /// Drone dispatcher representation
    /// </summary>
    public class Dispatcher
    {
        List<Drone> Drones { get; set; }
        List<CancellationTokenSource> DroneNavigationRoutinesActive { get; set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        public Dispatcher()
        {
            LoadDrones();
            DroneNavigationRoutinesActive = new List<CancellationTokenSource>();
        }

        /// <summary>
        /// Load drones from data source
        /// </summary>
        public void LoadDrones()
        {
            //Populate test drones. 
            //NOTE: This could be improved taking the drone list for this dispatcher from a DB, giving an ID to every Dispatcher
            Drones = new List<Drone>
            {
                new Drone(5937, 51.476105, -0.100224, 200),
                new Drone(6043, 51.474579, -0.171834, 100)
            };

            //TODO: Load tube stations locations
        }

        /// <summary>
        /// Load route points for every drone on the list
        /// </summary>
        public void LoadAllDronesRoute()
        {
            foreach (Drone drone in Drones)
            {
                List<Tuple<double, double>> routePoints = ReadRouteFile(drone.Id);
                drone.Route = routePoints;
            }
        }

        /// <summary>
        /// Power on all dispatcher's drones and starts their navigation routines
        /// </summary>
        public void StartDronesNavigationRoutine()
        {
            foreach (Drone drone in Drones)
            {
                var droneShutdownCancellationSource = new CancellationTokenSource();
                var droneShutdownToken = droneShutdownCancellationSource.Token;
                DroneNavigationRoutinesActive.Add(droneShutdownCancellationSource);

                Task.Run(() => PerformNavigationRoutine(drone, droneShutdownToken), droneShutdownToken);
            }
        }

        /// <summary>
        /// Sent shut down signal to all drones
        /// </summary>
        public void ShutDownDrones()
        {
            foreach (CancellationTokenSource shutdownToken in DroneNavigationRoutinesActive)
            {
                shutdownToken.Cancel();
            }
            DroneNavigationRoutinesActive.Clear();
        }

        /// <summary>
        /// Searchs for a csv file named after the drone ID and loads route points info 
        /// </summary>
        /// <param name="droneId">Drone ID</param>
        /// <returns>List of latitude, longitude of all the successfully parsed (right format) route points</returns>
        private List<Tuple<double, double>> ReadRouteFile(int droneId)
        {
            List<Tuple<double, double>> routePoints = new List<Tuple<double, double>>();

            string routeFilePath = Path.Combine(AppContext.BaseDirectory, "Data", droneId.ToString() + ".csv");
            if (File.Exists(routeFilePath))
            {
                List<string> routeFileLines = File.ReadAllLines(routeFilePath).ToList();
                foreach (string routeFileLine in routeFileLines)
                {
                    try
                    {
                        string[] route = routeFileLine.Replace("\"", "").Split(',');
                        routePoints.Add(new Tuple<double, double>(double.Parse(route[1]), double.Parse(route[2])));
                    }
                    catch (Exception exc)
                    {
                        //Ignore invalid points. 
                        //NOTE: Could be changed to stop drone iniatialization if wrong formatted points exist
                    }
                }
            }

            return routePoints;
        }

        /// <summary>
        /// Moves the specified drone across all its navigation points
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="droneShutdownToken"></param>
        private void PerformNavigationRoutine(Drone drone, CancellationToken droneShutdownToken)
        {
            foreach (Tuple<double, double> routePoint in drone.Route)
            {
                //If shutdown signal sent, send drone to origin point and shut it down
                if (droneShutdownToken.IsCancellationRequested)
                {
                    drone.MoveToOrigin();
                    Console.WriteLine("Navigation Routine for drone " + drone.Id + " canceled");
                    return;
                }

                drone.Move(routePoint.Item1, routePoint.Item2);
                Console.WriteLine("Drone " + drone.Id + " moved to " + "lat=" + drone.Latitude + ", lng=" + drone.Longitude);

                //TODO: Check if tube station near and report to console
            }
        }
    }
}
