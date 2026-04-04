using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Custom linked list for managing Admin users </summary>
    public class CustomAdminList
    {
        private AdminNode head;

        /// <summary> Adds a new Admin to the list </summary>
        public void AddAdmin(Admin admin)
        {
            AdminNode newNode = new AdminNode(admin);
            if (head == null)
            {
                head = newNode;
                return;
            }
            AdminNode current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }

        /// <summary> Removes an Admin by their ID </summary>
        public bool RemoveAdmin(int id)
        {
            if (head == null) return false;

            if (head.Data.AdminId == id)
            {
                head = head.Next;
                return true;
            }

            AdminNode current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.AdminId == id)
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