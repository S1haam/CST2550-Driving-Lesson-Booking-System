using DrivingLessonBookingSystem.DataStructures;

namespace DrivingLessonBookingSystem.Models
{
    public class Instructor
    {
        public int InstructorID { get; set; }
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorPasswordHash { get; set; }
        public string InstructorCarType { get; set; }
        public string InstructorStatus { get; set; }

        public CustomLessonList AvailableSlots { get; set; } = new CustomLessonList();
        public CustomLessonList InstructorUpcomingLessons { get; set; } = new CustomLessonList();
        public CustomLessonList InstructorPastLessons { get; set; } = new CustomLessonList();
    }
}
