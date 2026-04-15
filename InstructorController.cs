using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Connection.Data;
using Backend_Connection.Models;
using System.Threading.Tasks;

namespace Backend_Connection.Controllers
{
    //Api Controller for Instructor - Makes all routes start with /api/Instructor
    //Handles all CRUD operations for Instructor.
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        //Access to SQL through EF Core
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: /api/Instructor/{id}/notifications = returning all notfications for a specific Instructor.
        [HttpGet("{id}/notifications")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int id)
        {
            //Queries the notifications table for notificaitions related to specific instructor
            return await _context.Notification
                .Where(n => n.InstructorId == id && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }


        //GET: /api/Instructor/Learner?name=name = returning filtered list of learners querying by name
        [HttpGet("learners")]
        public async Task<ActionResult<IEnumerable<Learner>>> SearchLearner(string? name, string? email)

        {
            //Queries the Learner Table by matching name and email parameters.
            var query = _context.Learner.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(l => l.LearnerName.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(l => l.LearnerEmail.Contains(email));

            return Ok(await query.ToListAsync());
        }

        //GET: /api/Instructor = Finding all Instructors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instructor>>> GetInstructors()
        {
            //reads entire Instructors Table and returns as a list.
            return await _context.Instructor.ToListAsync();
        }


        //GET: /api/Instructor/id = Finding a specific Instructor
        [HttpGet("{id}")]
        public async Task<ActionResult<Instructor>> GetInstructor(int id)
        {
            //Reads the instructor table and matches the id parameter.
            //Fetches primary key
            var instructor = await _context.Instructor.FindAsync(id);

            if (instructor == null)
                return NotFound();

            return instructor;
        }


        //POST: /api/Instructor = Creating a new Instructor
        [HttpPost]
        public async Task<ActionResult<Instructor>> CreateInstructor (Instructor instructor)
        {
            //Adds new row to the instructors table.
            _context.Instructor.Add(instructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.InstructorId }, instructor);
        }


        //POST: /api/Instructor/request = Instructor accepting or declining student requests.
        [HttpPost("request")]
        public async Task<ActionResult<StudentRequest>> CreateStudentRequest(StudentRequest request)
        {
            //Writes to StudentRequest Table and Notification Table when a student requests to be taught.
            _context.StudentRequest.Add(request);

            _context.Notification.Add(new Notification
            { 
                InstructorId = request.InstructorId,
                NotificationType = "StudentRequest",
                Message = $"Learner {request.LearnerId} has requested to be taught by you."
            });

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        //POST: /api/Instructor/{InstructorId}/removed-student/{learnerId} = removing student from students list
        [HttpPost("{instructorId}/removed-student/{learnerId}")]
        public async Task<IActionResult> NotifyRemovedStudent(int instructorId, int learnerId)
        {
            //Writes to notification table when instructor removed them from their list of students.
            _context.Notification.Add(new Notification
            {
                InstructorId = instructorId,
                NotificationType = "RemovedStudent",
                Message = $"Learner {learnerId} has been removed from your list."
            });

            await _context.SaveChangesAsync();
            return Ok();
        }


        //POST: /api/Instructor/{InstructorId}/booking/{bookingId}/rescheduled = notifies if booking is recheduled.
        [HttpPost("{instructorId}/booking/{bookingId}/rescheduled")]
        public async Task<IActionResult> NotifyRescheduledBooking(int instructorId, int bookingId)
        {
            //Adds row to notification table when booking is rescheduled
            _context.Notification.Add(new Notification
            {
                InstructorId = instructorId,
                NotificationType = "RescheduledBooking",
                Message = $"Booking {bookingId} has been rescheduled by the learner."
            });

            await _context.SaveChangesAsync();
            return Ok();
        }


        //POST: /api/Instructor/{InstructorId}/booking/{bookingId}/cancelled = notifies if booking is cancelled.
        [HttpPost("{instructorId}/booking/{bookingId}/cancelled")]
        public async Task<IActionResult> NotifyCancelledBooking(int instructorId, int bookingId)
        {
            //Adds tow to notification table when booking is cancelled by the learner
            _context.Notification.Add(new Notification
            {
                InstructorId = instructorId,
                NotificationType = "CancelledBooking",
                Message = $"Booking {bookingId} has been cancelled by the learner."
            });

            await _context.SaveChangesAsync();
            return Ok();
        }


        //PUT: /api/Instructor/request/{requestId}/accept = accepting request
        [HttpPut("request/{requestId}/accept")]
        public async Task<IActionResult> AcceptRequest(int requestId, [FromBody] string acceptanceMessage)
        {
            //Updates the StudentRequest Table when accepting a request
            //Sets timestamps for request acceptance and updates
            //adds acceptance message
            var request = await _context.StudentRequest.FindAsync(requestId);
            if (request == null)
                return NotFound("Request not found.");

            request.RequestStatus = "Accepted";
            request.AcceptedAt = DateTime.Now;
            request.RequestUpdatedAt = DateTime.Now;
            request.AcceptanceMessage = acceptanceMessage;

            await _context.SaveChangesAsync();

            return Ok(new 
            {
                message = "Request accepted successfully.",
                request
            });
        }


        //PUT: /api/Instructor/request/{requestId}/decline = declining request.
        [HttpPut("request/{requestId}/decline")]
        public async Task<IActionResult> DeclineRequest(int requestId, [FromBody] string rejectionReason)
        {
            //Updates the StudentRequest Table when declining a request
            //Sets time stamps for request rejection and updates
            //adds rejecttion message/reason
            var request = await _context.StudentRequest.FindAsync(requestId);
            if (request == null)
                return NotFound("Request not found.");

            request.RequestStatus = "Rejected";
            request.RejectedAt = DateTime.Now;
            request.RequestUpdatedAt = DateTime.Now;
            request.AcceptanceMessage = rejectionReason;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Request declined successfully.",
                request
            });
        }


        //PUT: /api/Instructor/notifications/{notificationId}/read
        [HttpPut("notifications/{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationRead(int notificationId)
        {
            //Updates notification table when the message is read by the instructor. 
            //Sets IsRead to true.
            var notif = await _context.Notification.FindAsync(notificationId);
            if (notif == null) 
                return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //PUT: /api/Instructor/notifications/{notificationId}/pin = pinning notifcations that are pinned by instructor.
        [HttpPut("notifications/{notificationId}/pin")]
        public async Task<IActionResult> PinNotification(int notificationId)
        {
            //Updates notification table when the message is pinned by instructor
            //Sets IsPinned to true.
            //Returns message confirming pin
            var notif = await _context.Notifications.FindAsync(notificationId);
            if (notif == null)
                return NotFound();

            notif.IsPinned = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification is pinned." });
        }


        //PUT: /api/Instructor/Notifications/{notificationId}/unpin = unpinning notifications that rae unpinned by instructor.
        [HttpPut("notifications/{notificationId}/unpin")]
        public async Task<IActionResult> UnpinNotification(int notificationId)
        {
            //Updates notifcation table when message is unpinned by instuctor
            //Sets IsPinned to false.
            //Returns confirmation message.
            var notif = await _context.Notifications.FindAsync(notificationId);
            if (notif == null)
                return NotFound();

            notif.IsPinned = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification is Unpinned." });
        }


        //PUT: /api/Instructor/notifications/{notificationId}/delete = deleting notifications that are deleted by instructor.
        [HttpPut("notifications/{notificationId}/delete")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            //Updates notifcation table when message is deleted by instuctor
            //Sets IsDeleted to true.
            //Returns confirmation message.
            var notif = await _context.Notifications.FindAsync(notificationId);
            if (notif == null)
                return NotFound();

            notif.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification is Deleted." });
        }


        //PUT: /api/Instructor/notifications/{notificationId}/restore = restoring deleted notifications
        [HttpPut("notifications/{notificationId}/restore")]
        public async Task<IActionResult> RestoreNotification(int notificationId)
        {
            //Updates notifcation table when message is restored by instuctor
            //Sets IsDeleted to false.
            //Returns confirmation message.
            var notif = await _context.Notifications.FindAsync(notificationId);
            if (notif == null)
                return NotFound();

            notif.IsDeleted = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification is restored." });
        }


        //PUT: /api/Instructor/id = updating an Instructor
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor(int id, Instructor instructor)
        {
            //Updates intructor table when their details are changed.
            //Makes sure that the ID is matched to the primary key.
            if (id != instructor.InstructorId)
                return BadRequest("ID mismatch");

            _context.Entry(instructor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Instructor.Any(e => e.InstructorId == id))
                    return NotFound();

                throw;
            }
            return NoContent();
        }


        //DELETE: /api/Instructor/id = deleting an Instructor
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            //Updates instructor table when an instructor is deleted.
            var instructor = await _context.Instructor.FindAsync(id);
            if (instructor == null)
                return NotFound();

            _context.Instructor.Remove(instructor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
