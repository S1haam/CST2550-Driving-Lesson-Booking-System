using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend_Connection.Services
{
    // this service is responsible for creating jwt tokens for authenticated users
    public class JwtService
    {
        private readonly IConfiguration _config;

        // this constructor gives access to appsettings.json values
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        // this method generates a jwt token for a user after login
        public string GenerateToken(int userId, string email, string role)
        {

            Console.WriteLine($"JWT KEY = '{_config["Jwt:Key"]}'");
            // this gets the secret key from appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            // this defines how the token will be signed using the secret key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // these are the values stored inside the token (called claims)
            var claims = new[]
            {
                // sub = subject = user id (required by walkthrough)
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),

                // this is an extra standard claim to easily read user id later
                // does not break walkthrough, just improves usability
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),

                // stores the user's email
                new Claim(JwtRegisteredClaimNames.Email, email),

                // stores the user's role (learner instructor admin)
                // this is used by authorize roles
                new Claim(ClaimTypes.Role, role)
            };

            // this creates the actual jwt token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],       // who created the token
                audience: _config["Jwt:Audience"],   // who the token is for
                claims: claims,                      // data stored inside token
                expires: DateTime.UtcNow.AddHours(6),// token expires after 6 hours
                signingCredentials: creds            // signing info
            );

            // this converts the token object into a string to send to frontend
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}