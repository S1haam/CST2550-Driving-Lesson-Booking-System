namespace DrivingLessonApp.Models
{
    // represents a single booking in the system
    public class Booking
    {
        // unique id for each booking
        public int BookingId { get; set; }

        // id of learner making the booking
        public int LearnerId { get; set; }

        // id of instructor assigned to lesson
        public int InstructorId { get; set; }

        // date and time of lesson (used as hash key)
        public DateTime LessonDateTime { get; set; }

        // type of lesson (manual or automatic)
        public string LessonType { get; set; }

        // current status of booking
        public string BookingStatus { get; set; }

        // when booking was created
        public DateTime CreatedAt { get; set; }

        // last update time
        public DateTime UpdatedAt { get; set; }
    }
}