namespace AuditLog.Models
{
    public class Passenger : Person
    {
        public Halt DestinationStation { get; set; }
        public bool IsActive { get; set; }
    }
}
