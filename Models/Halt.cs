namespace AuditLog.Models
{
    public class Halt
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Order { get; set; }
        public int PlannedOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
