using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class LessonNode
    {
        public Lesson Data { get; set; }
        public LessonNode Next { get; set; }
        public LessonNode(Lesson data)
        {
            Data = data;
            Next = null;
        }
    }
}
