namespace DrivingLessonApp.Services
{
    public class LearnerSignupState
    {
        public string LicenceId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Phone { get; set; } = "";
        public string LessonType { get; set; } = "";
        public int SelectedInstructorId { get; set; }

        public void Clear()
        {
            LicenceId = "";
            Name = "";
            Email = "";
            Password = "";
            Phone = "";
            LessonType = "";
            SelectedInstructorId = 0;
        }
    }
}