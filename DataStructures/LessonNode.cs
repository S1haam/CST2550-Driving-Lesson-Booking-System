using Backend_admin.Models;

namespace Backend_admin.DataStructures
{
    public class LessonNode
    {
        public Booking Data { get; set; }
        public LessonNode Next { get; set; }

        public LessonNode(Booking data)
        {
            Data = data;
            Next = null;
        }
    }
}
