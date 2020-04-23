using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone_Simulation_App.Classes;
using System;

namespace UnitTesting
{
    [TestClass]
    public class DroneTest
    {
        Drone testDrone = new Drone(1111, false);

        [TestMethod]
        public void DroneInitialization()
        {
            Assert.IsNotNull(testDrone, "TestDrone is null");
            Assert.AreEqual(1111, testDrone.Id, "TestDrone ID is wrong. Expected 1111, got " + testDrone.Id);
        }

        [TestMethod]
        public void DroneMove()
        {
            testDrone.Move(new RoutePoint(1, 1, DateTime.Now));
            Assert.IsNotNull(testDrone.CurrentRoutePoint, "Current route point is null");
            Assert.AreEqual(1, testDrone.CurrentRoutePoint.Latitude, "Current point Latitude is wrong. Expected 1, got " + testDrone.CurrentRoutePoint.Latitude);
            Assert.AreEqual(1, testDrone.CurrentRoutePoint.Longitude, "Current point Longitude is wrong. Expected 1, got " + testDrone.CurrentRoutePoint.Longitude);
            Assert.AreEqual(1, testDrone.OriginRoutePoint.Latitude, "Origin point Latitude is wrong. Expected 1, got " + testDrone.OriginRoutePoint.Latitude);
            Assert.AreEqual(1, testDrone.OriginRoutePoint.Longitude, "Origin point Longitude is wrong. Expected 1, got " + testDrone.OriginRoutePoint.Longitude);

            testDrone.Move(new RoutePoint(2, 2, DateTime.Now));
            Assert.IsNotNull(testDrone.LastRoutePoint, "Last route point is null");
            Assert.AreEqual(1, testDrone.LastRoutePoint.Latitude, "Last point Latitude is wrong. Expected 1, got " + testDrone.LastRoutePoint.Latitude);
            Assert.AreEqual(1, testDrone.LastRoutePoint.Longitude, "Last point Longitude is wrong. Expected 1, got " + testDrone.LastRoutePoint.Longitude);
            Assert.AreEqual(2, testDrone.CurrentRoutePoint.Latitude, "Latitude from displaced current point is wrong. Expected 2, got " + testDrone.CurrentRoutePoint.Latitude);
            Assert.AreEqual(2, testDrone.CurrentRoutePoint.Longitude, "Longitude from displaced current point is wrong. Expected 2, got " + testDrone.CurrentRoutePoint.Longitude);

            testDrone.MoveToOrigin();
            Assert.IsNotNull(testDrone.CurrentRoutePoint, "Current route point is null on moving to origin");
            Assert.IsNotNull(testDrone.LastRoutePoint, "Last route point is null on moving to origin");
            Assert.AreEqual(2, testDrone.LastRoutePoint.Latitude, "Latitude from displaced last point is wrong. Expected 2, got " + testDrone.LastRoutePoint.Latitude);
            Assert.AreEqual(2, testDrone.LastRoutePoint.Longitude, "Longitude from displaced last point is wrong. Expected 2, got " + testDrone.LastRoutePoint.Longitude);
            Assert.AreEqual(1, testDrone.CurrentRoutePoint.Latitude, "Current point Latitude is wrong. Expected 1, got " + testDrone.CurrentRoutePoint.Latitude);
            Assert.AreEqual(1, testDrone.CurrentRoutePoint.Longitude, "Current point Longitude is wrong. Expected 1, got " + testDrone.CurrentRoutePoint.Longitude);
        }
    }
}
