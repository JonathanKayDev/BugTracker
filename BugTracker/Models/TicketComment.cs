using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketComment
    {
        // Primary Key
        public int Id { get; set; }


        [DisplayName("Member Comment")]
        public string? Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }


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
