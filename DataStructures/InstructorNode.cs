using Backend_Connection.Models;

namespace Backend_Connection.DataStructures
{
    public class InstructorNode
    {
        public Instructor Data { get; set; }
        public InstructorNode Next { get; set; }

        public InstructorNode(Instructor data) {
            Data = data;
            Next = null;
        }
    }
}
