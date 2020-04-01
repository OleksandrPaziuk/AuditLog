using AuditLog.Enums;
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
        private static List<AuditLogEntry> auditLogEntries = new List<AuditLogEntry>();
        static void Main()
        {
            Route updatedFile = default, originalFile = default;

            ReadFiles(ref updatedFile, ref originalFile);
            CompareJsonFiles(originalFile, updatedFile);


            string json = JsonConvert.SerializeObject(auditLogEntries, Formatting.Indented);
            Console.WriteLine(json);

            Console.ReadKey();
        }

        private static void ReadFiles(ref Route updatedFile, ref Route originalFile)
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
                updatedFile = JsonConvert.DeserializeObject<Route>(json);
            }
        }

        //todo
        private static void CompareJsonFiles(Route originalFile, Route updatedFile)
        {
            //todo cancelled
            CompareRideList(originalFile.Rides, updatedFile.Rides);
        }

        private static void CompareRideList(IList<Ride> originalRideList, IList<Ride> updatedRideList)
        {
            foreach (var originalRide in originalRideList)
            {
                CheckStartTimeRide(originalRide, updatedRideList);
                CheckRideDriver(originalRide, updatedRideList);
            }
        }

        private static void CheckStartTimeRide(Ride originalRide, IList<Ride> updatedRideList) // todo rename
        {
            var updatedRide = updatedRideList.FirstOrDefault(x => x.DateRide == originalRide.DateRide); //todo

            // ComparePlanedValue<Ride>(originalRide, updatedRide, nameof(originalRide.PlannedStartTime), nameof(originalRide.StartTime));

            if (updatedRide == null)
            {
                // uploadRide does not exit   //unplan
                return;
            }

            if (originalRide.PlannedStartTime != updatedRide.PlannedStartTime)
            {
                //plan
            }
            if (updatedRide.PlannedStartTime != updatedRide.StartTime)
            {
                //unplan
            }

            CheckStationOrderAndPassengers(originalRide.Stations, updatedRide.Stations);
        }

        private static void CheckStationOrderAndPassengers(IList<Station> originalStations, IList<Station> updatedStations)
        {
            //isActive
            foreach (var originalStation in originalStations)
            {
                var updatedStation = updatedStations.FirstOrDefault(x =>
                    x.Name == originalStation.Name && x.Address == originalStation.Address);

                // ComparePlanedValue<Station>(originalStation, updatedStation, nameof(originalStation.PlannedOrder), nameof(originalStation.Order));

                if (updatedStation == null)
                {
                    // uploadStation does not exit   //unplan
                    return; //todo
                }

                if (originalStation.PlannedOrder != updatedStation.PlannedOrder)
                {
                    // PlannedOrder change   //plan
                }

                if (originalStation.PlannedOrder != originalStation.Order)
                {
                    // PlannedOrder not eq Order   //unplan
                }

                CheckStationPassengers(originalStation, updatedStation);
            }
        }

        //todo
        private static void CheckStationPassengers(Station originalStation, Station updatedStationPassengers)
        {
            //todo check if list difference 
            foreach (var originalPassenger in originalStation.Passengers)
            {
                var updatedPassenger = updatedStationPassengers.Passengers.FirstOrDefault(x =>
                    x.FirstName == originalPassenger.FirstName &&
                    x.LastName == originalPassenger.LastName);

                if (updatedPassenger == null)
                {
                    // updatedPassenger does not exit   //unplan
                    return; //todo
                }

                if ((originalPassenger.DestinationStation.Order < updatedPassenger.DestinationStation.Order) && !updatedPassenger.IsActive)
                {
                    // the person did not reach the point  change   //unplan
                }

                if ((originalPassenger.DestinationStation.Order > updatedPassenger.DestinationStation.Order) && updatedPassenger.IsActive)
                {
                    // person drove the point   //unplan
                }
            }
        }

        // todo check change driver and exist driver
        private static void CheckRideDriver(Ride originalRide, IList<Ride> updatedRideList)
        {
            //todo add field for filter (LicenseNumber can not unique)
            var updatedRide = updatedRideList.FirstOrDefault(x => x.PlannedDriver.LicenseNumber == originalRide.PlannedDriver.LicenseNumber);

            // ComparePlanedValue<Ride>(originalRide, updatedRide, nameof(originalRide.PlannedDriver), nameof(originalRide.Driver));

            //todo 
            if (updatedRide == null)
            {
                // updatedRide does not exit   //plan
                return;
            }

            if (originalRide.PlannedDriver != updatedRide.PlannedDriver)
            {
                // plan change driver case
            }

            //todo compare 2 object
            if (updatedRide.PlannedDriver != updatedRide.Driver)
            {
                //unplan
            }
        }

        private static void ComparePlanedValue<T>(T originalObject, T updatedObject, string firstProperty, string secondProperty)
        {
            // todo does work with sub object
            Type type = typeof(T);
            var originalPropertyValue = type.GetProperty(firstProperty).GetValue(originalObject);
            var updatedFirstValue = type.GetProperty(secondProperty).GetValue(updatedObject);
            var updatedSecondValue = type.GetProperty(secondProperty).GetValue(updatedObject);

            if (updatedObject == null)
            {
                Console.WriteLine('-');
                return;
            }

            if (!originalPropertyValue.Equals(updatedFirstValue))
            {
                GenerateAuditLog(TypeChange.Planned, true, originalPropertyValue.ToString(), updatedFirstValue.ToString());
            }

            if (!originalPropertyValue.Equals(updatedSecondValue))
            {
                GenerateAuditLog(TypeChange.Unplanned, false, originalPropertyValue.ToString(), updatedSecondValue.ToString());
            }
        }

        private static void GenerateAuditLog(TypeChange typeChange, bool planned, string originalValue, string newValue)
        {
            var s = new Approval()
            {
                Approved = true,
                Driver = new Driver()
            };
            var auditLogEntry = new AuditLogEntry()
            {
                TypeChange = typeChange.ToString(),
                Planned = planned,
                OriginalValue = originalValue,
                NewValue = newValue,
                Approvals = new List<Approval>() { s }

            };

            auditLogEntries.Add(auditLogEntry);
        }
    }
}