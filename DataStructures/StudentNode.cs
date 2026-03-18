using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class StudentNode
    {
        public Student Data { get; set; }
        public StudentNode Next { get; set; } // Points to the next StudentNode

        public StudentNode(Student data)
        {
            Data = data;
            Next = null;
        }
    }
}
