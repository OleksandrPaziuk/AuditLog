namespace AuditLog.Models
{
    public class Passenger : Person
    {
        public Station DestinationStation { get; set; } //todo string
        public bool IsActive { get; set; }
    }
}
