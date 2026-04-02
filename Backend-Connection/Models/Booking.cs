using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_Connection.Models
{
    /// <summary>
    /// Represents a confirmed driving lesson booking between a learner and an instructor
    /// </summary>
    public class Booking
    {
        /// <summary> Unique identifier for the booking </summary>
        [Key]
        public int BookingId { get; set; }

        /// <summary> Foreign key for the associated learner </summary>
        public int LearnerId { get; set; }
        /// <summary> Foreign key for the associated instructor </summary>
        public int InstructorId { get; set; }

        /// <summary> The scheduled date for the lesson </summary>
        [Required]
        public DateTime LessonDate { get; set; }

        /// <summary> The scheduled start time for the lesson </summary>
        [Required]
        public TimeSpan LessonTime { get; set; }

        /// <summary> The type of lesson (Beginners or Mock Test) </summary>
        public string LessonType { get; set; }
        /// <summary> Current status of the booking (Pending or Confirmed or Cancelled) </summary>
        public string BookingStatus { get; set; }

        /// <summary> Timestamp of when the booking was created </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary> Timestamp of the last update to this booking </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary> Navigation property for the Learner </summary>
        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        /// <summary> Navigation property for the Instructor </summary>
        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; }
    }
}