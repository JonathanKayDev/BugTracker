﻿using BugTracker.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        // Primary Key
        public int Id { get; set; }


        [DisplayName("File Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("File Description")]
        public string? Description { get; set; }

        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf", ".txt" })]
        public IFormFile? FormFile { get; set; }

        [DisplayName("File Name")]
        public string? FileName { get; set; }

        public byte[]? FileData { get; set; }

        [DisplayName("File Extension")]
        public string? FileType { get; set; }


        // Foreign keys - retrieved from navigation property below
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Navigation properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; } // the 'virtual' keyword means the property won't be created, since they are navigation properties
    }
}
