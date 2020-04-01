using AuditLog.Enums;
using System;
using System.Collections.Generic;

namespace AuditLog.Models
{
    public class AuditLogEntry
    {
        public DateTime StartDateOfChange { get; set; }
        public DateTime EndDateOfChange { get; set; }
        public string AffectedDays { get; set; } //todo bitmask
        public string TypeChange { get; set; } // todo
        public bool Planned { get; set; } // todo
        public string OriginalValue { get; set; } // todo
        public string NewValue { get; set; } // todo
        public IList<Approval> Approvals { get; set; }
    }
    public class Approval
    {
        public Driver Driver { get; set; }
        public bool Approved { get; set; } // todo
    }
}
