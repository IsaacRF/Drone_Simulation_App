using System;
using System.Device.Location;

namespace Drone_Simulation_App.Classes
{
    public class RoutePoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Time { get; set; }

        public RoutePoint(double latitude, double longitude, DateTime time)
        {
            Latitude = latitude;
            Longitude = longitude;
            Time = time;
        }

        /// <summary>
        /// Gets GeoCoordinate from latitude and longitude
        /// </summary>
        /// <returns>GeoCoordinate</returns>
        public GeoCoordinate GetGeoCoordinate()
        {
            return new GeoCoordinate(Latitude, Longitude);
        }
    }
}
