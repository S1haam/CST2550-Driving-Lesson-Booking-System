using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Node structure for storing lesson bookings </summary>
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