using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    /// <summary>
    /// Represents an administrator with authority to manage users and view audit logs
    /// </summary>
    public class Admin
    {
        /// <summary> Unique identifier for the admin </summary>
        [Key]
        public int AdminId { get; set; }

        /// <summary> The email address used for admins login </summary>
        [Required, EmailAddress, StringLength(100)]
        public string AdminEmail { get; set; }

        /// <summary> The hashed version of the admins password for security </summary>
        [Required]
        public string AdminPasswordHash { get; set; }

        /// <summary> The specific role assigned to the admin </summary>
        [Required, StringLength(50)]
        public string AdminRole { get; set; }

        /// <summary> Total count of users currently in the system </summary>
        public int SystemUserCount { get; set; }
        /// <summary> Total count of bookings ever made in the system </summary>
        public int SystemBookingCount { get; set; }
        /// <summary> Total count of pending requests in the system </summary>
        public int SystemRequestCount { get; set; }

        // Removal & Audit Variables 
        /// <summary> ID of a user (learner or instructor) who was removed by this admin </summary>
        public int? RemovedUserId { get; set; }
        /// <summary> The role of the user who was removed </summary>
        public string RemovedUserRole { get; set; }
        /// <summary> The ID of the admin who performed a removal action </summary>
        public int? RemovedByAdminId { get; set; }
        /// <summary> Explanation for why a user was removed from the database </summary>
        public string RemovalReason { get; set; }
        /// <summary> The timestamp when the removal occurred </summary>
        public DateTime? RemovedAt { get; set; }
        /// <summary> The status of the removal process </summary>
        public string RemovalStatus { get; set; }
    }
}