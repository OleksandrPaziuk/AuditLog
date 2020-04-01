using System;
using System.Collections.Generic;
using AuditLog.Enums;

namespace AuditLog.Models
{
    public class Route
    {
        public string Name { get; set; }
        public IList<DaysBitMask> ActiveDays { get; set; }  //todo bitmask delete enum
        public IList<Ride> Rides { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class Ride
    {
        public DateTime DateRide { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime PlannedStartTime { get; set; }
        public IList<Station> Stations { get; set; }
        public Driver Driver { get; set; }
        public Driver PlannedDriver { get; set; }
        public bool Cancelled { get; set; }
    }

    public class Station
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IList<Passenger> Passengers { get; set; }
        public int Order { get; set; } // int
        public int PlannedOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class Passenger : Person
    {
        public Station DestinationStation { get; set; }
        public bool IsActive { get; set; }
    }

    public class Driver : Person
    {
        public string LicenseNumber { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}