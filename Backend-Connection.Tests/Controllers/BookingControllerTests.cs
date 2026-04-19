using Backend_Connection.Controllers;
using Backend_Connection.Models;
using Backend_Connection.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend_Connection.Tests.Controllers
{
    [TestClass]
    public class BookingControllerTests
    {
        [TestMethod]
        public void GetAllBookings_ReturnsApiResponse()
        {
            // Arrange
            var context = DbContextHelper.CreateDb("BookingTestDb");

            // Add fake booking
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                LearnerId = 1,
                InstructorId = 1,
                LessonDate = new DateTime(2026, 04, 05),
                LessonTime = new TimeSpan(10, 00, 00),
                LessonType = "Beginners",
                BookingStatus = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            context.SaveChanges();

            var controller = new BookingController(context);

            // Act
            var result = controller.GetAllBookings() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);

            var response = result.Value as ApiResponse<List<Booking>>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Bookings retrieved successfully", response.Message);
            Assert.AreEqual(1, response.Data.Count);
        }
    }
}