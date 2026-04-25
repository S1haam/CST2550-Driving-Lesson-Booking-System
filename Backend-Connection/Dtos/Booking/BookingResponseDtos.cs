namespace Backend_Connection.Dtos.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = string.Empty;
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public DateTime LessonDate { get; set; }
        public TimeSpan LessonTime { get; set; }
        public string LessonType { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string? InstructorNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}