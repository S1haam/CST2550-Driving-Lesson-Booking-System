using Backend_Connection.Data;      // Gives access to the ApplicationDbContext (your database)
using Backend_Connection.Models;    // Gives access to your ApiResponse and Availability model
using Microsoft.AspNetCore.Mvc;     // Provides controller and HTTP response functionality

namespace Backend_Connection.Controllers
{
    
    // enables automatic model validation and consistent API behaviour
    [ApiController]

    // sets the base route for this controller
    // URL for all endpoints inside this controller
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // The database context is injected so the controller can access the database

        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
            // this gives us the database connection automatically
        }

        // Handles GET requests to: GET api/availability
        [HttpGet]
        public IActionResult GetAllAvailability()
        {
            // Retrieves all availability records from the database.
            var data = _context.Availabilities.ToList();

            // Wraps the result in a consistent API response format.
            return Ok(new ApiResponse<List<Availability>>
            {
                Success = true,                           // Indicates the request succeeded
                Message = "Availabilities retrieved successfully", // output message
                Data = data                               // The actual list of availability records
            });
        }
    }
}