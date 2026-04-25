using Backend_Connection.DataStructures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_Connection.Models
{
    /// <summary>
    /// Represents a learner driver booking lessons through the system
    /// </summary>
    public class Learner
    {
        /// <summary> Unique identifier for the learner </summary>
        [Key]
        public int LearnerId { get; set; }

        /// <summary> Full name of the learner </summary>
        [Required]
        [StringLength(100)]
        public string LearnerName { get; set; } = string.Empty;

        /// <summary> The learner's driving licence identification number </summary>
        [Required]
        public string LearnerLicenceId { get; set; } = string.Empty;

        /// <summary> Contact email for the learner </summary>
        [Required]
        [EmailAddress]
        public string LearnerEmail { get; set; } = string.Empty;

        /// <summary> Contact phone number for the learner </summary>
        public string LearnerPhone { get; set; } = string.Empty;

        /// <summary> Hashed password for learner portal access </summary>
        [Required]
        public string LearnerPasswordHash { get; set; } = string.Empty;

        /// <summary> Current account status (Active or Inactive) </summary>
        [Required]
        public string LearnerStatus { get; set; } = "Active";

        /// <summary> Amount of lessons this learner has completed in the past </summary>
        public int PastLessonCount { get; set; } = 0;

        /// <summary> Amount of upcoming lessons booked by this learner </summary>
        public int NextLessonCount { get; set; } = 0;

        /// <summary> Preferred lesson type chosen during signup (Manual or Automatic) </summary>
        [Required]
        public string LearnerLessonType { get; set; } = string.Empty;

        /// <summary> Selected instructor id chosen during signup </summary>
        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        /// <summary> Date and time when the learner account was created </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary> Date and time when the learner last logged in </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary> Navigation property for the selected instructor </summary>
        public virtual Instructor? Instructor { get; set; }

        // EF CORE SECTION

        /// <summary> Collection of bookings associated with this learner </summary>
        public virtual ICollection<Booking> DbBookings { get; set; } = new List<Booking>();

        /// <summary> Collection of lesson requests submitted by this learner </summary>
        public virtual ICollection<StudentRequest> StudentRequests { get; set; } = new List<StudentRequest>();

        // DATA STRUCTURES (Custom Lists)

        [NotMapped]
        public CustomLessonList LearnerPastLessons { get; set; } = new CustomLessonList();

        [NotMapped]
        public CustomLessonList LearnerUpcomingLessons { get; set; } = new CustomLessonList();
    }
}