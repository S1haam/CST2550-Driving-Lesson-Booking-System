using DrivingLessonBookingSystem.Models;
using System.Reflection.Metadata.Ecma335;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class CustomAdminList
    {
        private AdminNode head;

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

        public Admin FindAdmin(int id)
        {
            AdminNode current = head;
            while (current != null)
            {
                if (current.Data.AdminId == id) return current.Data;
                current = current.Next;
            }
            return null;
        }

        public Admin FindAdminByEmail(string email)
        {
            AdminNode current = head;
            while (current != null)
            {
                if (current.Data.AdminEmail == email) return current.Data;
                current = current.Next;
            }
            return null;
        }

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
                    currentNext = current.Next.Next;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
    }
}
