using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class InstructorNode
    {
        public Instructor Data { get; set; }
        public InstructorNode Next { get; set; }

        public InstructorNode(Instructor Data) {
            Data = Data;
            Next = null;
        }
    }
}
