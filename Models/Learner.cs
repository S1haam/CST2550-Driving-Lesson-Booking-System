namespace DrivingLessonBookingSystem.Models
{
    public class Learner
    {
        public int LearnerID { get; set; }
        public string LearnerName { get; set; }
        public string LearnerLicenceID { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerPhone { get; set; }

        public string LearnerPasswordHash { get; set; }
        public string LearnerStatus { get; set; }

        //Custom Lists
        public CustomLessonList LearnerPastLessons { get; set; } = new CustomLessonList();
        public CustomLessonList LearningUpcomingLessons { get; set; } = new CustomLessonList();

        public int PastLessonCount { get; set; }
        public int NextLessonCount { get; set; }

    }
}
