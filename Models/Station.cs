using System.Collections.Generic;

namespace AuditLog.Models
{
    public class Station
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IList<Passenger> Passengers { get; set; }
        public int Order { get; set; }
        public int PlannedOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
