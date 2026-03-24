using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    public class AdminNode
    {
        public Admin Data { get; set; }
        public AdminNode Next { get; set; }
        
        public AdminNode(Admin data) 
        {
            Data = data;
            Next = null;
        }
    }
}
