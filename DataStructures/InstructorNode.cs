using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Node structure for the Instructor linked list </summary>
    public class InstructorNode
    {
        public Instructor Data { get; set; }
        public InstructorNode Next { get; set; }

        public InstructorNode(Instructor data)
        {
            Data = data;
            Next = null;
        }
    }
}