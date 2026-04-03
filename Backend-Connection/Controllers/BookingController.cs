using Backend_Connection.Data;      // Gives access to ApplicationDbContext (your database)
using Backend_Connection.Models;    // Gives access to Booking model + ApiResponse<T>
using Microsoft.AspNetCore.Mvc;     // Provides controller + HTTP response functionality

namespace Backend_Connection.Controllers
{

    // enables automatic model validation and consistent API behaviour
    [ApiController]

    // sets the base route for this controller
    // URL for all endpoints inside this controller
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // The database context is injected so the controller can access the database

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
            //gives us the database connection automatically
        }

        // Handles GET requests to: GET api/booking
        [HttpGet]
        public IActionResult GetAllBookings()
        {
            // Retrieves all booking records from the database.
            var data = _context.Bookings.ToList();

            // Wraps the result in your custom ApiResponse<T> format
            return Ok(new ApiResponse<List<Booking>>
            {
                Success = true,                           //indicates the request succeeded
                Message = "Bookings retrieved successfully", //output
                Data = data                               // actual list of bookings
            });
        }
    }
}