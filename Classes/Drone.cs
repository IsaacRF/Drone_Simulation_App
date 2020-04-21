using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Drone_Simulation_App.Classes
{
    /// <summary>
    /// A drone representation
    /// </summary>
    public class Drone
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Tuple<double, double> OriginPosition { get; set; }
        public int Speed { get; set; }
        public List<Tuple<double, double>> Route { get; set; }


        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="id">Drone id</param>
        /// <param name="latitude">Initial position latitude</param>
        /// <param name="longitude">Initial position longitude</param>
        /// <param name="speed">Drone km/h speed</param>
        public Drone(int id, double latitude, double longitude, int speed)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            OriginPosition = new Tuple<double, double>(latitude, longitude);
            Speed = speed;
            Route = new List<Tuple<double, double>>();
        }

        /// <summary>
        /// Move drone to specified position (Simulated with delay depending on drone speed)
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void Move(double latitude, double longitude)
        {
            Thread.Sleep(3000 - Speed * 10);
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Move to dron to origin position
        /// </summary>
        public void MoveToOrigin()
        {
            Move(OriginPosition.Item1, OriginPosition.Item2);
        }
    }
}
