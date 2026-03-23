using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class CustomLessonList
    {
        private LessonNode head;

        public void AddLesson(Lesson lesson)
        {
            LessonNode newNode = new LessonNode(lesson);
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

        public bool RemoveLesson(int lessonId)
        {
            if (head == null) return false;

            if (head.Data.LessonId == lessonId)
            {
                head = head.Next;
                return true;
            }

            LessonNode current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.LessonId = lessonId)
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
