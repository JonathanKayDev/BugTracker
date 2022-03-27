using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketType
    {
        // Primary Key
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string? Name { get; set; }

    }
}
