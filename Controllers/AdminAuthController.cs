using Backend_admin.Data;
using Backend_admin.Models;
using Backend_admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_admin.Controllers
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;

        public AdminAuthController(ApplicationDbContext context, PasswordService passwordService, JwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        // POST: api/admin/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminRegisterRequest request)
        {
            if (_context.Admins.Any(a => a.AdminEmail == request.Email))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Email already exists",
                    Data = null
                });
            }

            var admin = new Admin
            {
                AdminName = request.Name,
                AdminEmail = request.Email,
                AdminPasswordHash = _passwordService.HashPassword(request.Password),
                Status = "Active",
                LastPasswordChange = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Admin registered successfully",
                Data = null
            });
        }

        // POST: api/admin/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminEmail == request.Email);

            if (admin == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Admin not found",
                    Data = null
                });
            }

            if (!_passwordService.VerifyPassword(admin.AdminPasswordHash, request.Password))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid password",
                    Data = null
                });
            }

            var token = _jwtService.GenerateToken(admin.AdminId, admin.AdminEmail, "Admin");

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful",
                Data = new { token }
            });
        }
    }

    public class AdminRegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
