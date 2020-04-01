using System;
using System.Collections;
using System.Collections.Generic;

namespace AuditLog.Models
{
    public class Route
    {
        public string Name { get; set; }
        public BitArray ActiveDays { get; set; }
        public IList<Ride> Rides { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}