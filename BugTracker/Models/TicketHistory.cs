using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        // Primary Key
        public int Id { get; set; }


        [DisplayName("Updated Item")]
        public string? Property { get; set; }

        [DisplayName("Previous")]
        public string? OldValue { get; set; }

        [DisplayName("Current")]
        public string? NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of Change")]
        public string? Description { get; set; }


        // Foreign keys - retrieved from navigation property below
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Navigation properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
