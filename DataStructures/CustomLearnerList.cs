using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Custom list for managing Learner records </summary>
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

        public Learner FindLearner(int id)
        {
            LearnerNode current = head;
            while (current != null)
            {
                if (current.Data.LearnerId == id) return current.Data;
                current = current.Next;
            }
            return null;
        }
    }
}