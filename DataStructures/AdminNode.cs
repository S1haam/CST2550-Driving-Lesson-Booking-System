using DrivingLessonBookingSystem.Models;

namespace DrivingLessonBookingSystem.DataStructures
{
    /// <summary> Represents a single node in the Admin linked list </summary>
    public class AdminNode
    {
        /// <summary> The Admin data stored in this node </summary>
        public Admin Data { get; set; }

        /// <summary> Reference to the next AdminNode in the list </summary>
        public AdminNode Next { get; set; }

        public AdminNode(Admin data)
        {
            Data = data;
            Next = null;
        }
    }
}