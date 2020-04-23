using System;
using System.Collections.Generic;
using System.Device.Location;
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
        private List<Drone> Drones { get; set; }
        private List<CancellationTokenSource> DroneNavigationRoutinesActive { get; set; }
        private List<TubeStation> TubeStations { get; set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="speedUp">If true, speeds up the simulation to see results faster on console</param>
        public Dispatcher(bool speedUp)
        {
            LoadDrones(speedUp);
            DroneNavigationRoutinesActive = new List<CancellationTokenSource>();
        }

        /// <summary>
        /// Load drones from data source
        /// </summary>
        /// <param name="speedUp">If true, speeds up the simulation to see results faster on console</param>
        public void LoadDrones(bool speedUp)
        {
            //Populate test drones. 
            //NOTE: This could be improved taking the drone list for this dispatcher from a DB, giving an ID to every Dispatcher
            Drones = new List<Drone>
            {
                new Drone(5937, speedUp),
                new Drone(6043, speedUp)
            };

            //Load tube stations data
            TubeStations = ReadTubeStationsFile();
            if (TubeStations == null)
            {
                throw new InvalidDataException("Error loading tube stations data");
            }
        }

        /// <summary>
        /// Load route points for every drone on the list
        /// </summary>
        public void LoadAllDronesRoute()
        {
            foreach (Drone drone in Drones)
            {
                List<RoutePoint> routePoints = ReadRouteFile(drone.Id);
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
        /// Checks if there is a tube station near the specified coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public TubeStation FirstTubeStationNearLocation(double latitude, double longitude)
        {
            //NOTE: This could be improved letting the user introduce the distance meters on console
            IEnumerable<TubeStation> tubeStationsNear = TubeStations.Where(tubeStation => GetLocationsDistance(latitude, longitude, tubeStation.Latitude, tubeStation.Longitude) < 350);
            
            if (tubeStationsNear != null && tubeStationsNear.Count() > 0)
            {
                return tubeStationsNear.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates distances between 2 specified locations
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public double GetLocationsDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var coord1 = new GeoCoordinate(latitude1, longitude1);
            var coord2 = new GeoCoordinate(latitude2, longitude2);

            return coord1.GetDistanceTo(coord2);
        }

        /// <summary>
        /// Moves the specified drone across all its navigation points
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="droneShutdownToken"></param>
        private void PerformNavigationRoutine(Drone drone, CancellationToken droneShutdownToken)
        {
            foreach (RoutePoint routePoint in drone.Route)
            {
                //If shutdown signal sent, send drone to origin point and shut it down
                if (droneShutdownToken.IsCancellationRequested)
                {
                    drone.MoveToOrigin();
                    Console.WriteLine("Navigation Routine for drone " + drone.Id + " canceled");
                    return;
                }

                drone.Move(routePoint);
                Console.WriteLine("[" + routePoint.Time.ToLongTimeString() + "] Drone " + drone.Id + " moved to lat=" + drone.CurrentRoutePoint.Latitude + ", lng=" + drone.CurrentRoutePoint.Longitude);

                //Report traffic conditions if tube station near
                TubeStation tubeStationNear = FirstTubeStationNearLocation(drone.CurrentRoutePoint.Latitude, drone.CurrentRoutePoint.Longitude);
                if (tubeStationNear != null)
                {
                    ReportTrafficInfo(drone, tubeStationNear);
                }

                //Send shut down signal if 8:10 or later
                if (routePoint.Time.Hour >= 8 && routePoint.Time.Minute >= 10)
                {
                    ShutDownDrones();
                }
            }
        }

        /// <summary>
        /// Reports the current position traffic info
        /// </summary>
        private void ReportTrafficInfo(Drone drone, TubeStation tubeStation)
        {
            double distanceTraveled = drone.LastRoutePoint.GetGeoCoordinate().GetDistanceTo(drone.CurrentRoutePoint.GetGeoCoordinate());
            double speed = distanceTraveled / (drone.CurrentRoutePoint.Time - drone.LastRoutePoint.Time).TotalSeconds;

            Console.WriteLine("[TRAFFIC REPORT BY DRONE " + drone.Id + "] " +
                "Location: " + tubeStation.StreetName +
                ", Time: " + drone.CurrentRoutePoint.Time.ToShortTimeString() +
                ", Speed: " + Math.Round(speed, 2) + "m/s" +
                ", Traffic: " + drone.GetTrafficConditionsInCurrentPosition());
        }

        /// <summary>
        /// Searchs for a csv file named after the drone ID and loads route points info 
        /// </summary>
        /// <param name="droneId">Drone ID</param>
        /// <returns>List of latitude, longitude of all the successfully parsed (right format) route points</returns>
        private List<RoutePoint> ReadRouteFile(int droneId)
        {
            List<RoutePoint> routePoints = new List<RoutePoint>();

            string routeFilePath = Path.Combine(AppContext.BaseDirectory, "Data", droneId.ToString() + ".csv");
            if (File.Exists(routeFilePath))
            {
                List<string> routeFileLines = File.ReadAllLines(routeFilePath).ToList();
                foreach (string routeFileLine in routeFileLines)
                {
                    try
                    {
                        string[] routePoint = routeFileLine.Replace("\"", "").Split(',');
                        routePoints.Add(new RoutePoint(double.Parse(routePoint[1].Replace(".", ",")), double.Parse(routePoint[2].Replace(".", ",")), DateTime.Parse(routePoint[3])));
                    }
                    catch (Exception exc)
                    {
                        //Ignore invalid points. 
                        //NOTE: Could be changed to stop drone initialization if wrong formatted points exist
                    }
                }
            }

            return routePoints;
        }

        /// <summary>
        /// Searchs for a csv file named tube.csv on Data folder, and loads all the data into the Dispatcher
        /// </summary>
        /// <returns>List of Tube Stations</returns>
        private List<TubeStation> ReadTubeStationsFile()
        {
            List<TubeStation> tubeStations = new List<TubeStation>();

            string tubeStationsFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "tube.csv");
            if (File.Exists(tubeStationsFilePath))
            {
                List<string> routeFileLines = File.ReadAllLines(tubeStationsFilePath).ToList();
                foreach (string routeFileLine in routeFileLines)
                {
                    try
                    {
                        string[] route = routeFileLine.Replace("\"", "").Split(',');
                        tubeStations.Add(new TubeStation(route[0], double.Parse(route[1].Replace(".", ",")), double.Parse(route[2].Replace(".", ","))));
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Error loading tube stations info (Wrong Format). Error info:");
                        Console.WriteLine(exc.Message);
                        return null;
                    }
                }
            }

            return tubeStations;
        }
    }
}
