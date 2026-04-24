using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_admin.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required, EmailAddress]
        public string AdminEmail { get; set; }

        [Required]
        public string AdminPasswordHash { get; set; }

        public string AdminName { get; set; }
        public string? AdminPhone { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime LastPasswordChange { get; set; } = DateTime.UtcNow;
    }

}
