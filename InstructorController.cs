using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    //Api Controller for Instructor
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public InstructorController(WebAPIContext context)
        {
            _context = context;
        }

        //GET: /api/Instructor/{id}/notifications = returning all notfications for a specific Instructor.
        [HttpGet("{id}/notifications")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int id)
        {
            return await _context.Notification
                .Where(n => n.InstructorId == id)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }


        //GET: /api/Instructor/Learner?name=name = returning filtered list of learners querying by name
        [HttpGet("learners")]
        public async Task<ActionResult<IEnumerable<Learner>>> SearchLearner(string? name, string? email)
        {
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
            return await _context.Instructor.ToListAsync();
        }


        //GET: /api/Instructor/id = Finding a specific Instructor
        [HttpGet("{id}")]
        public async Task<ActionResult<Instructor>> GetInstructor(int id)
        {
            var instructor = await _context.Instructor.FindAsync(id);

            if (instructor == null)
                return NotFound();

            return instructor;
        }


        //POST: /api/Instructor = Creating a new Instructor
        [HttpPost]
        public async Task<ActionResult<Instructor>> CreateInstructor (Instructor instructor)
        {
            _context.Instructor.Add(instructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstructor), new { id = instructor.InstructorId }, instructor);
        }


        //POST: /api/Instructor/request = Instructor accepting or declining student requests.
        [HttpPost("request")]
        public async Task<ActionResult<StudentRequest>> CreateStudentRequest(StudentRequest request)
        {
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
            var notif = await _context.Notification.FindAsync(notificationId);
            if (notif == null) 
                return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //PUT: /api/Instructor/id = updating an Instructor
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor(int id, Instructor instructor)
        {
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
            var instructor = await _context.Instructor.FindAsync(id);
            if (instructor == null)
                return NotFound();

            _context.Instructor.Remove(instructor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
