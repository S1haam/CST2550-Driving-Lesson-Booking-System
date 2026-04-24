using System.ComponentModel.DataAnnotations;

namespace Backend_Connection.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int InstructorId { get; set; }

        public string Message { get; set; } = string.Empty;

        // examples: CancelledBooking, RescheduledBooking, StudentRequest, RemovedStudent
        public string NotificationType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public bool IsPinned { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public virtual Instructor? Instructor { get; set; }
    }
}