using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        // Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        // Attachment
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }

        [DisplayName("File Name")]
        public string? ImageFileName { get; set; }

        public byte[]? ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string? ImageFileType { get; set; }

        // Foreign Keys
        [DisplayName("Company")]
        public int? CompanyId { get; set; }

        [DisplayName("Project Priority")]
        public int? ProjectPriorityId { get; set; }


        // Navigation properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>(); // many to many
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>(); // one to many
    }
}
