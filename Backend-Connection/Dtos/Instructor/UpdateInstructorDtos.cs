namespace Backend_Connection.Dtos.Instructor
{
    public class UpdateInstructorDto
    {
        public string InstructorCode { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;
        public string InstructorCarType { get; set; } = string.Empty;
        public string InstructorStatus { get; set; } = string.Empty;
    }
}