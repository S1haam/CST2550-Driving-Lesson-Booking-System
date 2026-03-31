using DrivingLessonBookingSystem.DataStructures;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class Learner
    {
        [Key]
        public int LearnerId { get; set; }

        [Required, StringLength(100)]
        public string LearnerName { get; set; }

        [Required]
        public string LearnerLicenceId { get; set; }

        [Required, EmailAddress]
        public string LearnerEmail { get; set; }

        public string LearnerPhone { get; set; }
        public string LearnerPasswordHash { get; set; }
        public string LearnerStatus { get; set; }

        public int PastLessonCount { get; set; }
        public int NextLessonCount { get; set; } 

        // EF CORE SECTION 
        public virtual ICollection<Booking> DbBookings { get; set; }
        public virtual ICollection<StudentRequest> StudentRequests { get; set; }

        // DATA STRUCTURES (Custom Lists) 
        [NotMapped]
        public CustomLessonList LearnerPastLessons { get; set; } = new CustomLessonList();

        [NotMapped]
        public CustomLessonList LearnerUpcomingLessons { get; set; } = new CustomLessonList();
    }
}