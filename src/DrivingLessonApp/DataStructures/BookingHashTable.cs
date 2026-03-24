using DrivingLessonApp.Models;

namespace DrivingLessonApp.DataStructures
{
    // custom hash table used to store bookings efficiently
    public class BookingHashTable
    {
        // array of lists to store bookings (handles collisions)
        private List<Booking>[] table;

        // size of the hash table
        private int size;

        // constructor to initialise the table
        public BookingHashTable(int size)
        {
            this.size = size;
            table = new List<Booking>[size];
        }

        // hash function converts datetime into array index
        private int Hash(DateTime key)
        {
            return Math.Abs(key.GetHashCode()) % size;
        }

        // adds a booking into the hash table
        public void Add(Booking booking)
        {
            int index = Hash(booking.LessonDateTime);

            // create bucket if empty
            if (table[index] == null)
                table[index] = new List<Booking>();

            // add booking to bucket
            table[index].Add(booking);
        }

        // searches for bookings using datetime key
        public List<Booking> Search(DateTime key)
        {
            int index = Hash(key);

            // return empty list if nothing found
            if (table[index] == null)
                return new List<Booking>();

            return table[index];
        }
    }
}