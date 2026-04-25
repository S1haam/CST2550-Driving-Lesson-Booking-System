namespace Backend_Connection.Dtos.Learner
{
    public class CreateLearnerDto
    {
        public string LearnerName { get; set; } = string.Empty;
        public string LearnerLicenceId { get; set; } = string.Empty;
        public string LearnerEmail { get; set; } = string.Empty;
        public string LearnerPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LearnerLessonType { get; set; } = string.Empty;
        public int InstructorId { get; set; }
    }
}