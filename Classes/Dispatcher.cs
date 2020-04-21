using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drone_Simulation_App.Classes
{
    /// <summary>
    /// Drone dispatcher representation
    /// </summary>
    public class Dispatcher
    {
        List<Drone> Drones { get; set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        public Dispatcher()
        {
            LoadDrones();            
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
                new Drone(6043, 51.474579, -0.171834, 150)
            };
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
        /// Power on all dispatcher's drones and starts their routes
        /// </summary>
        public void StartDrones()
        {
            //TODO: Develop drones routines
            Console.Write("TEST");
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
    }
}
