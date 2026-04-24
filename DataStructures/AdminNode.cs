using Backend_admin.Models;

namespace Backend_admin.DataStructures
{
    public class AdminNode
    {
        public Admin Data { get; set; }
        public AdminNode Next { get; set; }

        public AdminNode(Admin data)
        {
            Data = data;
            Next = null;
        }
    }
}





