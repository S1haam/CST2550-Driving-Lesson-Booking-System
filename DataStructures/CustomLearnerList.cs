using Backend_admin.Models;

namespace Backend_admin.DataStructures
{
    public class CustomLearnerList
    {
        private LearnerNode head;

        public void AddLearner(Learner learner)
        {
            LearnerNode newNode = new LearnerNode(learner);
            if (head == null)
            {
                head = newNode;
                return;
            }

            LearnerNode current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }

            current.Next = newNode;
        }
    }
}
