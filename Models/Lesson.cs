using DrivingLessonBookingSystem.DataStructures;

namespace DrivingLessonBookingSystem.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public int LearnerId {  get; set; }
        public int InstructorId { get; set; }
        public DateTime LessonDate { get; set; }
        public string Status { get; set; } // "Available", "Booked", "Completed" 
    }
}
