using Microsoft.Extensions.Configuration;   // Allows access to appsettings.json values
using Microsoft.IdentityModel.Tokens;       // Provides classes for signing and validating JWTs
using System.IdentityModel.Tokens.Jwt;      // Main library for creating JWT tokens
using System.Security.Claims;               // Used to store user identity information inside the token
using System.Text;                          // Needed for encoding the secret key

namespace Backend_Connection.Services
{
    // this service is for generating JWT tokens for authenticated users
    public class JwtService
    {
        private readonly IConfiguration _config;
        // IConfiguration lets us read values from appsettings.json, such as the JWT secret key

        public JwtService(IConfiguration config)
        {
            _config = config;
            // gives us access to configuration settings
        }

        // Generates a JWT token containing the user's ID, email, and role
        public string GenerateToken(int userId, string email, string role)
        {
            
            //    This key is used to sign the token so it cannot be tampered with
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            // creates the signing credentials using the secret key and HMAC SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // defines the claims (information stored inside the token))
            //    These claims will be readable by the backend when validating the token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // User ID
                new Claim(JwtRegisteredClaimNames.Email, email),           // User email
                new Claim(ClaimTypes.Role, role)                           // User role (Learner/Instructor)
            };

            // this creates the actual JWT token object.
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],       // Who issued the token
                audience: _config["Jwt:Audience"],   // Who the token is intended for
                claims: claims,                      // The claims we defined above
                expires: DateTime.UtcNow.AddHours(6),// Token expiry time (6 hours)
                signingCredentials: creds            // The signing credentials created earlier
            );

            // it then  takes token object into a string that can be returned to the client.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}