using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIImageGeneratorBackend.Models;
using AIImageGeneratorBackend.Services;

namespace AIImageGeneratorBackend.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "All fields are required" });
            }

            var existingUser = await _userService.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = await _userService.RegisterUser(request);
            if (user == null)
            {
                return BadRequest(new { message = "Registration failed" });
            }

            var token = _jwtService.GenerateToken(user.Email);
            
            return Ok(new 
            { 
                message = "User registered successfully",
                userId = user.Id,
                email = user.Email,
                name = user.Name,
                token = token,
                expiresIn = "1 hour"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            var user = await _userService.AuthenticateUser(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user.Email);
            
            return Ok(new { 
                token = token,
                userId = user.Id,
                username = user.Email,
                name = user.Name,
                expiresIn = "1 hour"
            });
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userService.GetUserByEmail(userEmail);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var newToken = _jwtService.GenerateToken(user.Email);
            
            return Ok(new 
            { 
                token = newToken,
                expiresIn = "1 hour"
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // In a real implementation, you might want to:
            // 1. Add the token to a blacklist
            // 2. Remove the token from user's active sessions
            // 3. Log the logout event
            
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
