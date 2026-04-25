namespace Backend_Connection.Dtos.Learner
{
    public class UpdateLearnerDto
    {
        public string LearnerName { get; set; } = string.Empty;
        public string LearnerLicenceId { get; set; } = string.Empty;
        public string LearnerEmail { get; set; } = string.Empty;
        public string LearnerPhone { get; set; } = string.Empty;
        public string LearnerStatus { get; set; } = string.Empty;
        public string LearnerLessonType { get; set; } = string.Empty;
        public int InstructorId { get; set; }
    }
}