using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    /// <summary>
    /// Represents the result and feedback of a completed driving lesson
    /// </summary>
    public class LessonOutcome
    {
        /// <summary> Unique identifier for the outcome record </summary>
        [Key]
        public int OutcomeId { get; set; }

        /// <summary> Foreign key for the related booking </summary>
        public int BookingId { get; set; }

        /// <summary> Detailed notes provided by the instructor </summary>
        public string LessonNotes { get; set; }
        /// <summary> Final status (Outstanding, Good, Fair, Needs improvement, Unsatisfactory) </summary>
        public string LessonOutcomeStatus { get; set; }
        /// <summary> Optional numerical rating of the students performance </summary>
        public int? LessonRating { get; set; }

        /// <summary> The timestamp when the notes were created </summary>
        public DateTime NotesCreatedAt { get; set; } = DateTime.Now;

        /// <summary> Navigation property for the Booking </summary>
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}