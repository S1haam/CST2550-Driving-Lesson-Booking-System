using Backend_admin.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_admin.Models
{
    /// <summary>
    /// Represents a learner driver booking lessons through the system
    /// </summary>
    public class Learner
    {
       

        /// <summary> Unique identifier for the learner </summary>
        [Key]
        public int LearnerId { get; set; }

        /// <summary> Full name of the learner </summary>
        [Required, StringLength(100)]
        public string LearnerName { get; set; }

        /// <summary> The learners driving licence identification number </summary>
        [Required]
        public string LearnerLicenceId { get; set; }

        /// <summary> Contact email for the learner </summary>
        [Required, EmailAddress]
        public string LearnerEmail { get; set; }

        /// <summary> Contact phone number for the learner </summary>
        public string LearnerPhone { get; set; }
        /// <summary> Hashed password for learner portal access </summary>
        public string? LearnerPasswordHash { get; set; }
        /// <summary> Current account status (Active or inactive) </summary>
        public string LearnerStatus { get; set; }

        /// <summary> Amount of lessons this learner has completed in the past </summary>
        public int PastLessonCount { get; set; }
        /// <summary> Amount of upcoming lessons booked by this learner </summary>
        public int NextLessonCount { get; set; }

        // EF CORE SECTION 
        /// <summary> Collection of bookings associated with this learner </summary>
        public virtual ICollection<Booking> DbBookings { get; set; }
        /// <summary> Collection of lesson requests submitted by this learner </summary>
        public virtual ICollection<StudentRequest> StudentRequests { get; set; }

        public Learner()
        {
            DbBookings = new List<Booking>();
            StudentRequests = new List<StudentRequest>();
        }


    }
}