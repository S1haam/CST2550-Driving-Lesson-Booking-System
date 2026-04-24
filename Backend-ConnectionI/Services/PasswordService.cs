using Microsoft.AspNetCore.Identity;

namespace Backend_Connection.Services
{
    // this is a simple service for hashing and verifying passwords
    public class PasswordService
    {
        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();
        private readonly object _dummy = new object(); //non-null reference

        // hashes a plain password
        public string HashPassword(string plainPassword)
        {
            return _hasher.HashPassword(_dummy, plainPassword);
        }

        // verifies a password against a stored hash
        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var result = _hasher.VerifyHashedPassword(_dummy, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}