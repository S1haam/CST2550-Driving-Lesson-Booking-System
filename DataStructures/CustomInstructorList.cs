using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Custom list to handle Instructor data operations </summary>
    public class CustomInstructorList
    {
        private InstructorNode head;

        public void AddInstructor(Instructor instructor)
        {
            InstructorNode newNode = new InstructorNode(instructor);
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

        /// <summary> Finds an Instructor based on their unique ID </summary>
        public Instructor FindInstructor(int id)
        {
            InstructorNode current = head;
            while (current != null)
            {
                if (current.Data.InstructorId == id) return current.Data;
                current = current.Next;
            }
            return null;
        }
    }
}