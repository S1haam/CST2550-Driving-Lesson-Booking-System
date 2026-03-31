using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string AdminEmail { get; set; } 

        [Required]
        public string AdminPasswordHash { get; set; }

        [Required, StringLength(50)]
        public string AdminRole { get; set; }

        [cite_start]
        public int SystemUserCount { get; set; }
        public int SystemBookingCount { get; set; }
        public int SystemRequestCount { get; set; }

        [cite_start]// Removal & Audit Variables 
        public int? RemovedUserId { get; set; }
        public string RemovedUserRole { get; set; }
        public int? RemovedByAdminId { get; set; }
        public string RemovalReason { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string RemovalStatus { get; set; }
    }
}