using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class CustomInstructorList
    {
        private IntructorNode head;

        public void AddInstructor(Instructor instructor)
        {
            IntructorNode newNode = new InstructorNode(instructor);
            if (head == null)
            {
                head = newNode;
                return;
            }
            InstructorNode current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }

        public Instructor FindInstructor(int id)
        {
            InstructorNode current = head;
            while (current != null)
            {
                if (current.Data.InstructorID == id) return current.Data;
                current = current.Next;
            }
            return null;
        }
    }
}
