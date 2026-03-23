using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
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
