using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class CustomLearnerList
    {
        private LearnerNode head;

        public void AddLearner(Learner learner)
        {
            LearnerNode newNode = new LearnerNode(learner);

        }
    }
}
