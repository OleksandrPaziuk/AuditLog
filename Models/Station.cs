using System.Collections.Generic;

namespace AuditLog.Models
{
    public class Station : Halt
    {
        public IList<Passenger> Passengers { get; set; }
    }
}
