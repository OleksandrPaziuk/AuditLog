using AuditLog.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuditLog
{
    static class Program
    {
        static void Main()
        {
            Route uploadFile = default, originalFile = default;

            ReadWriteFiles(ref uploadFile, ref originalFile);
            CompareJsonFiles(originalFile, uploadFile);


            Console.ReadKey();
        }

        private static void ReadWriteFiles(ref Route uploadFile, ref Route originalFile)
        {
            using (StreamReader r =
                new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\Input\Original.json"))))
            {
                string json = r.ReadToEnd();
                originalFile = JsonConvert.DeserializeObject<Route>(json);
            }

            using (StreamReader r =
                new StreamReader(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\Input\Updated.json"))))
            {
                string json = r.ReadToEnd();
                uploadFile = JsonConvert.DeserializeObject<Route>(json);
            }
        }

        //todo
        private static void CompareJsonFiles(Route originalFile, Route uploadFile)
        {
            //todo cancelled
            CompareRideList(originalFile.Rides, uploadFile.Rides);
        }

        private static void CompareRideList(IList<Ride> originalRideList, IList<Ride> uploadRideList)
        {
            foreach (var originalRide in originalRideList)
            {
                CheckStartTimeRide(originalRide, uploadRideList);
                CheckRideDriver(originalRide, uploadRideList);
            }
        }

        private static void CheckStartTimeRide(Ride originalRide, IList<Ride> uploadRideList) // todo rename
        {
            var uploadRide = uploadRideList.FirstOrDefault(x => x.DateRide == originalRide.DateRide); //todo

            if (uploadRide == null)
            {
                // uploadRide does not exit   //unplan
                return;
            }

            if (originalRide.PlannedStartTime != uploadRide.PlannedStartTime)
            {
                //plan
            }
            if (uploadRide.PlannedStartTime != uploadRide.StartTime)
            {
                //unplan
            }

            CheckStationOrderAndPassengers(originalRide.Stations, uploadRide.Stations);
        }

        private static void CheckStationOrderAndPassengers(IList<Station> originalStations, IList<Station> uploadStations)
        {
            foreach (var originalStation in originalStations)
            {
                var uploadStation = uploadStations.FirstOrDefault(x =>
                    x.Name == originalStation.Name && x.Address == originalStation.Address);

                if (uploadStation == null)
                {
                    // uploadStation does not exit   //unplan
                    return; //todo
                }

                if (originalStation.PlannedOrder != uploadStation.PlannedOrder)
                {
                    // PlannedOrder change   //plan
                }

                if (originalStation.PlannedOrder != originalStation.Order)
                {
                    // PlannedOrder not eq Order   //unplan
                }

                CheckStationPassengers(originalStation.Passengers, uploadStation.Passengers);
            }
        }

        //todo add check active person on this station
        private static void CheckStationPassengers(IList<Passenger> originalStationPassengers, IList<Passenger> uploadStationPassengers)
        {
            //todo check if list difference 
            foreach (var originalStationPassenger in originalStationPassengers)
            {
                var uploadStationPassenger = uploadStationPassengers.FirstOrDefault(x => x.Person.FirstName == originalStationPassenger.Person.FirstName && x.Person.LastName == originalStationPassenger.Person.LastName);

                if (uploadStationPassenger == null)
                {
                    // uploadStationPassenger does not exit   //unplan
                    return; //todo
                }

                //todo check if isActive
                // if (uploadStationPassenger.PlannedOrder != uploadStation.PlannedOrder)
                // {
                //     // PlannedOrder change   //plan
                // }
                //
                // if (originalStation.PlannedOrder != originalStation.Order)
                // {
                //     // PlannedOrder not eq Order   //unplan
                // }
            }
        }

        // todo check change driver and exist driver
        private static void CheckRideDriver(Ride originalRide, IList<Ride> uploadRideList)
        {
            //todo add field for filter (LicenseNumber can not unique)
            var uploadRide = uploadRideList.FirstOrDefault(x => x.PlannedDriver.LicenseNumber == originalRide.PlannedDriver.LicenseNumber);

            if (uploadRide == null)
            {
                // uploadRide does not exit   //plan
                return;
            }

            if (originalRide.PlannedDriver != uploadRide.PlannedDriver)
            {
                // plan change driver case
            }

            //todo compare 2 object
            if (uploadRide.PlannedDriver != uploadRide.Driver)
            {
                //unplan
            }
            //todo
        }

        // private static void GenerateAuditLog()
        // {

        // }

        // private static void ComparePlanedValue()
        // {
        //
        // }
    }
}