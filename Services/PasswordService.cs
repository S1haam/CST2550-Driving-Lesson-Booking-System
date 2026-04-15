using Microsoft.AspNetCore.Identity;

namespace Backend_admin.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();
        private readonly object _dummy = new object();

        public string HashPassword(string plainPassword)
        {
            return _hasher.HashPassword(_dummy, plainPassword);
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var result = _hasher.VerifyHashedPassword(_dummy, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
