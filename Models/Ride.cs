using System;
using System.Collections.Generic;

namespace AuditLog.Models
{
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
}
