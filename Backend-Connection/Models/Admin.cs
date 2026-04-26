using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_Connection.Models
{
    // this represents an admin user account in the system
    public class Admin
    {
        // unique id for each admin
        [Key]
        public int AdminId { get; set; }

        // email used for login
        [Required, EmailAddress]
        public string AdminEmail { get; set; }

        // hashed password stored securely
        [Required]
        public string AdminPasswordHash { get; set; }

        // role used for authorization (always "Admin")
        [Required]
        public string AdminRole { get; set; } = "Admin";

        // display name shown in settings/dashboard
        public string AdminName { get; set; } = "Admin User";

        // optional contact number
        public string? AdminPhone { get; set; }

        // account status (Active / Inactive)
        public string Status { get; set; } = "Active";

        // tracks last password update for security
        public DateTime LastPasswordChange { get; set; } = DateTime.UtcNow;
    }
}