using Backend_Connection.Data;         
using Microsoft.EntityFrameworkCore;        

namespace Backend_Connection.Tests
{
    // A helper class used only while  testing
    // creates an in memory version
    // so no need to test in SQL server and can run without it
    public static class DbContextHelper
    {
    
        // a new in memory is created in ApplicationDbContext
        // 'name' allows each test to have its own isolated database
        public static ApplicationDbContext CreateDb(string name)
        {
            // build database options stating to EF Core that it should use an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)   // creates a fake database stored in memory
                .Options;

            // Return a new ApplicationDbContext using these options
            return new ApplicationDbContext(options);
        }
    }
}