namespace WebAPI.Models
{
    public class Notification
    {
        //Notification specific Identifier
        public int NotificationId { get; set; }

        //Specific Instructor Identifier
        public int InstructorId { get; set; }

        //Message
        public string Message { get; set; }

        //Notification types = Rescheduled, Cancelled, Student Request and Removed student
        public string NotificationType { get; set; }

        //Time message was sent or recieved
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //If message is read
        public bool IsRead { get; set; } = false;

        //If message is pinned
        public bool IsPinned { get; set; } = false;

        //If message is deleted
        public bool IsDeleted { get; set; } = false;
    }
}
