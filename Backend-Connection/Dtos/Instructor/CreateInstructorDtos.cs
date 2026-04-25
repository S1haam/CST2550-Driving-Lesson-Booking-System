namespace Backend_Connection.Dtos.Instructor
{
    public class CreateInstructorDto
    {
        public string InstructorCode { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string InstructorCarType { get; set; } = string.Empty;
    }
}