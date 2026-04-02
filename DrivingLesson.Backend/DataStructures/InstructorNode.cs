using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class InstructorNode
    {
        public Instructor Data { get; set; }
        public InstructorNode Next { get; set; }

        public InstructorNode(Instructor data) {
            Data = data;
            Next = null;
        }
    }
}
