using Backend_admin.Models;

namespace Backend_admin.DataStructures
{
    public class InstructorNode
    {
        public Instructor Data { get; set; }
        public InstructorNode Next { get; set; }

        public InstructorNode(Instructor data)
        {
            Data = data;
            Next = null;
        }
    }
}
