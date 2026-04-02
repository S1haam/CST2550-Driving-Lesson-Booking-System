using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    /// <summary>
    /// Represents an initial request from a student to book a specific instructor or slot
    /// </summary>
    public class StudentRequest
    {
        /// <summary> Unique identifier for the request </summary>
        [Key]
        public int RequestId { get; set; }

        /// <summary> ID of the learner making the request </summary>
        [Required]
        public int LearnerId { get; set; }

        /// <summary> ID of the instructor being requested </summary>
        [Required]
        public int InstructorId { get; set; }

        /// <summary> ID of the specific availability slot being requested </summary>
        public int AvailabilityId { get; set; }

        /// <summary> A message sent from the learner to the instructor </summary>
        public string RequestMessage { get; set; }

        /// <summary> The current state of the request (Pending or Accepted or Rejected) </summary>
        [Required]
        public string RequestStatus { get; set; }

        /// <summary> When the request was first submitted </summary>
        public DateTime RequestCreatedAt { get; set; } = DateTime.Now;
        /// <summary> When the request was last modified </summary>
        public DateTime? RequestUpdatedAt { get; set; }

        // Instructor Actions
        /// <summary> Timestamp for when an instructor accepted the request </summary>
        public DateTime? AcceptedAt { get; set; }
        /// <summary> Timestamp for when an instructor rejected the request </summary>
        public DateTime? RejectedAt { get; set; }
        /// <summary> Reason provided if the request was denied </summary>
        public string RejectionReason { get; set; }
        /// <summary> Message sent by the instructor upon acceptance </summary>
        public string AcceptanceMessage { get; set; }

        // Learner Removal (Audit)
        /// <summary> Timestamp if the request was archived or removed </summary>
        public DateTime? RemovedAt { get; set; }
        /// <summary> Reason for removing the request </summary>
        public string RemovalReason { get; set; }

        // Navigation Properties
        /// <summary> Navigation property for the learner </summary>
        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        /// <summary> Navigation property for the instructor </summary>
        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; }

        /// <summary> Navigation property for the availability slot </summary>
        [ForeignKey("AvailabilityId")]
        public virtual Availability Availability { get; set; }
    }
}