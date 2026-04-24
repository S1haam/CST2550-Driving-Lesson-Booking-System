using Backend_admin.Models;

namespace Backend_admin.DataStructures
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
    }
}
