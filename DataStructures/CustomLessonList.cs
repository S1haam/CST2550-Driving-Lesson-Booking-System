using Backend_admin.Models;

namespace Backend_admin.DataStructures
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
    }
}

