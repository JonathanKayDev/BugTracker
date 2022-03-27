using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketPriority
    {
        // Primary Key
        public int Id { get; set; }

        [DisplayName("Priority Name")]
        public string? Name { get; set; }
    }
}
