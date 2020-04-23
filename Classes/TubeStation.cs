namespace Drone_Simulation_App.Classes
{
    /// <summary>
    /// A representation of a Tube Station
    /// </summary>
    public class TubeStation
    {
        public string StreetName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public TubeStation(string streetName, double latitude, double longitude)
        {
            StreetName = streetName;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
