using DrivingLessonBookingSystem.DataStructures;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingLessonBookingSystem.Models
{
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; } 

        [Required]
        public string InstructorCode { get; set; }  

        [Required, StringLength(100)]
        public string InstructorName { get; set; }  

        [Required, EmailAddress]
        public string InstructorEmail { get; set; }  

        public string InstructorPhone { get; set; }  
        public string InstructorPasswordHash { get; set; } 
        public string InstructorCarType { get; set; } 
        public string InstructorStatus { get; set; } 

        // EF CORE SECTION 
        public virtual ICollection<Availability> DbAvailabilities { get; set; }
        public virtual ICollection<Booking> DbBookings { get; set; }
        public virtual ICollection<StudentRequest> IncomingRequests { get; set; }
        public virtual ICollection<LessonOutcome> InstructorOutcomes { get; set; }

        // DATA STRUCTURES (Custom Lists) 
        [NotMapped]
        public CustomLessonList AvailableSlots { get; set; } = new CustomLessonList();

        [NotMapped]
        public CustomLessonList InstructorUpcomingLessons { get; set; } = new CustomLessonList();

        [NotMapped]
        public CustomLessonList InstructorPastLessons { get; set; } = new CustomLessonList();
    }
}