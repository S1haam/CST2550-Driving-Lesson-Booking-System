using Backend_admin.Models;

namespace Backend_admin.DataStructures
{
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
    }
}

