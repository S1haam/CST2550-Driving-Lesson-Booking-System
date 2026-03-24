using DrivingLessonBookingSystem.DataStructures;

namespace DrivingLessonBookingSystem.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public int LearnerId {  get; set; }
        public int InstructorId { get; set; }

        public DateTime LessonDate { get; set; }
        public TimeSpan LessonTime { get; set; }

        public string BookingStatus { get; set; } // "Available", "Booked", "Completed" 

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
