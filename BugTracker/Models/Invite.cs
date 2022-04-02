﻿using System.ComponentModel;

namespace BugTracker.Models
{
    public class Invite
    {
        // Primary Key
        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Invitee Email")]
        public string? InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string? InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string? InviteeLastName { get; set; }

        public bool IsValid { get; set; }


        // Foreign Keys
        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string? InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string? InviteeId { get; set; }


        // Navigation properties
        public virtual Company? Company { get; set; }
        public virtual BTUser? Invitor { get; set; }
        public virtual BTUser? Invitee { get; set; }
        public virtual Project? Project { get; set; }
    }
}