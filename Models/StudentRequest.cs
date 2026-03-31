using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class StudentRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int LearnerId { get; set; }

        [Required]
        public int InstructorId { get; set; } 

        public int AvailabilityId { get; set; }

        public string RequestMessage { get; set; }

        [Required]
        public string RequestStatus { get; set; }

        public DateTime RequestCreatedAt { get; set; } = DateTime.Now;
        public DateTime? RequestUpdatedAt { get; set; } 

        // Instructor Actions
        public DateTime? AcceptedAt { get; set; } 
        public DateTime? RejectedAt { get; set; } 
        public string RejectionReason { get; set; } 
        public string AcceptanceMessage { get; set; } 

        [cite_start]// Learner Removal 
        public DateTime? RemovedAt { get; set; }
        public string RemovalReason { get; set; }

        // Navigation Properties
        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; }

        [ForeignKey("AvailabilityId")]
        public virtual Availability Availability { get; set; }
    }
}