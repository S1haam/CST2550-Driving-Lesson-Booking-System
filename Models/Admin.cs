namespace DrivingLessonBookingSystem.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPasswordHash { get; set; }
        public string AdminRole { get; set; }
        
        public int SystemUserCount { get; set; }
        public int SystemBookingCount { get; set; }
        public int SystemRequestCount { get; set; }
    }
}
