using Backend_Connection.Models;

namespace Backend_Connection.DataStructures
{
    public class LearnerNode
    {
        public Learner Data { get; set; }
        public LearnerNode Next { get; set; }
        public LearnerNode(Learner data) 
        { 
            Data = data;
            Next = null;
        }
    }
}
