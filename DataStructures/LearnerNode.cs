using Backend_admin.Models;

namespace Backend_admin.DataStructures
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
