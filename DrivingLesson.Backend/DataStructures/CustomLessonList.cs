using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class CustomLessonList
    {
        private LessonNode head;

        public void AddLesson(Booking booking)
        {
            LessonNode newNode = new LessonNode(booking);
            if (head == null)
            {
                head = newNode;
                return;
            }
            LessonNode current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }

        public bool RemoveLesson(int bookingId)
        {
            if (head == null) return false;

            if (head.Data.BookingId == bookingId)
            {
                head = head.Next;
                return true;
            }

            LessonNode current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.BookingId == bookingId)
                {
                    current.Next = current.Next.Next;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
    }
}