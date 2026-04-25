using Backend_Connection.Models;

namespace Backend_Connection.DataStructures
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
