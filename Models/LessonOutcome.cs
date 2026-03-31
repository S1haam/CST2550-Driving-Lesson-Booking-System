using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class LessonOutcome
    {
        [Key]
        public int OutcomeId { get; set; }

        public int BookingId { get; set; }

        public string LessonNotes { get; set; }
        public string LessonOutcomeStatus { get; set; }
        public int? LessonRating { get; set; }

        public DateTime NotesCreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}