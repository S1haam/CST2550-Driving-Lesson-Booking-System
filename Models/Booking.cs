using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public int LearnerId { get; set; }
        public int InstructorId { get; set; }

        [Required]
        public DateTime LessonDate { get; set; }

        [Required]
        public TimeSpan LessonTime { get; set; }

        public string LessonType { get; set; }
        public string BookingStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; }
    }
}