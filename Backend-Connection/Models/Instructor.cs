using Backend_Connection.DataStructures;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_Connection.Models
{
    /// <summary>
    /// Represents a driving instructor who teaches lessons and manages their availability.
    /// </summary>
    public class Instructor
    {
        /// <summary> Unique identifier for the instructor. </summary>
        [Key]
        public int InstructorId { get; set; }

        /// <summary> A unique display code for the instructor. </summary>
        [Required]
        public string InstructorCode { get; set; } = string.Empty;

        /// <summary> Full name of the driving instructor. </summary>
        [Required]
        [StringLength(100)]
        public string InstructorName { get; set; } = string.Empty;

        /// <summary> Contact email for the instructor. </summary>
        [Required]
        [EmailAddress]
        public string InstructorEmail { get; set; } = string.Empty;

        /// <summary> Contact phone number for the instructor. </summary>
        public string InstructorPhone { get; set; } = string.Empty;

        /// <summary> Hashed password for instructor access. </summary>
        public string? InstructorPasswordHash { get; set; }

        /// <summary> Type of car used for lessons (Manual or Automatic). </summary>
        [Required]
        public string InstructorCarType { get; set; } = string.Empty;

        /// <summary> Current employment status (Active or Inactive). </summary>
        [Required]
        public string InstructorStatus { get; set; } = "Active";

        // EF CORE NAVIGATION PROPERTIES

        /// <summary> Collection of availability slots linked to this instructor. </summary>
        public virtual ICollection<Availability> DbAvailabilities { get; set; } = new List<Availability>();

        /// <summary> Collection of lesson bookings linked to this instructor. </summary>
        public virtual ICollection<Booking> DbBookings { get; set; } = new List<Booking>();

        /// <summary> Collection of learners assigned to this instructor. </summary>
        public virtual ICollection<Learner> DbLearners { get; set; } = new List<Learner>();

        /// <summary> Collection of requests from students for this instructor. </summary>
        public virtual ICollection<StudentRequest> IncomingRequests { get; set; } = new List<StudentRequest>();

        /// <summary> Collection of recorded outcomes for lessons. </summary>
        public virtual ICollection<LessonOutcome> InstructorOutcomes { get; set; } = new List<LessonOutcome>();

        // CUSTOM DATA STRUCTURES (NOT MAPPED TO DATABASE)

        [NotMapped]
        public CustomLessonList AvailableSlots { get; set; } = new CustomLessonList();

        [NotMapped]
        public CustomLessonList InstructorUpcomingLessons { get; set; } = new CustomLessonList();
    }
}