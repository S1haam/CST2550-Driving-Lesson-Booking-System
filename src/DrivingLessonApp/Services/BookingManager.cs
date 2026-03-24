using DrivingLessonApp.Models;
using DrivingLessonApp.DataStructures;

namespace DrivingLessonApp.Services
{
    // manages booking operations and connects ui to hash table
    public class BookingManager
    {
        private BookingHashTable table;

        // constructor creates hash table
        public BookingManager()
        {
            table = new BookingHashTable(100);
        }

        // creates a new booking and stores it
        public void CreateBooking(Booking booking)
        {
            booking.CreatedAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;

            table.Add(booking);
        }

        // retrieves bookings based on datetime
        public List<Booking> GetBookings(DateTime key)
        {
            return table.Search(key);
        }
    }
}