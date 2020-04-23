using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public List<RoutePoint> Route { get; set; }
        public RoutePoint OriginRoutePoint { get; set; }
        public RoutePoint CurrentRoutePoint { get; set; }
        public RoutePoint LastRoutePoint { get; set; }
        public bool IsSpedUp { get; set; }

        public enum TrafficConditions
        {
            HEAVY, LIGHT, MODERATE
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="id">Drone id</param>
        /// <param name="isSpedUp">If true, ignores delays between points to sped up console</param>
        public Drone(int id, bool isSpedUp)
        {
            Id = id;
            IsSpedUp = isSpedUp;
            Route = new List<RoutePoint>();
        }

        /// <summary>
        /// Move drone to specified position (Simulated with delay depending on drone speed)
        /// </summary>
        /// <param name="routePoint">Position to move</param>
        public void Move(RoutePoint routePoint)
        {
            //NOTE: Velocity to reach a point is based on point time, but sped up to accelerate the results coming up in console
            int time = 0;
            if (LastRoutePoint != null && CurrentRoutePoint != null)
            {
                time = (int)(CurrentRoutePoint.Time - LastRoutePoint.Time).TotalSeconds * 1000;
            }
            Thread.Sleep(IsSpedUp ? time / 10 : time);

            if (OriginRoutePoint == null)
            {
                OriginRoutePoint = routePoint;
            }
            LastRoutePoint = CurrentRoutePoint;
            CurrentRoutePoint = routePoint;
        }

        /// <summary>
        /// Move to dron to origin position
        /// </summary>
        public void MoveToOrigin()
        {
            Move(OriginRoutePoint);
        }

        /// <summary>
        /// Gets traffic conditions in current position
        /// </summary>
        /// <returns>Simulated state of traffic. HEAVY, LIGHT or MODERATE</returns>
        public string GetTrafficConditionsInCurrentPosition()
        {
            Random rnd = new Random();
            TrafficConditions trafficConditions = (TrafficConditions)rnd.Next(0, 2);
            return Enum.GetName(typeof(TrafficConditions), trafficConditions);
        }
    }
}
