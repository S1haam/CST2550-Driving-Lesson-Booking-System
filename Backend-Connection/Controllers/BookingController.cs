using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Connection.Controllers
{
    [ApiController]
    //Define the API route
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        //method created privately
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context; //saved to be used by other methods
        }

        // GET: api/booking
        [HttpGet]
        public IActionResult GetAllBookings()
        {
            var data = _context.Bookings.ToList();

            //Data is being reutnred in the form of a JSON
            return Ok(data);
        }
    }
}