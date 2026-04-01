using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    /// <summary>
    /// Represents a specific time slot when an instructor is free to teach
    /// </summary>
    public class Availability
    {
        /// <summary> Unique identifier for the availability slot </summary>
        [Key]
        public int AvailabilityId { get; set; }

        /// <summary> The specific date and time the instructor is free </summary>
        [Required]
        public DateTime AvailableDateTime { get; set; }

        /// <summary> Indicates if this slot has already been booked </summary>
        public bool IsTaken { get; set; } = false;

        /// <summary> Foreign key linking to the instructor </summary>
        public int InstructorId { get; set; }

        /// <summary> Navigation property for the associated instructor </summary>
        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; }
    }
}