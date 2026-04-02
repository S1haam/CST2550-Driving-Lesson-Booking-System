//controller features being used
using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Connection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearnerController : ControllerBase
    {

        //allows communication to the SQL server
        private readonly ApplicationDbContext _context;

        public LearnerController(ApplicationDbContext context)
        {
            _context = context; //saved to be used by other methods later on
        }

        // GET: api/learner
        [HttpGet]
        public IActionResult GetAllLearners()
        {
            //fetches all data from the database in the SQL server
            var data = _context.Learners.ToList();
            return Ok(data); //saved in JSON format
        }
    }
}