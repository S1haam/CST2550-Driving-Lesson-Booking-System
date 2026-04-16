using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Node representing a Learner in the custom data structure </summary>
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