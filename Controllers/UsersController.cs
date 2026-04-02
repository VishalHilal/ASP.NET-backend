using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIImageGeneratorBackend.Models;
using AIImageGeneratorBackend.Services;

namespace AIImageGeneratorBackend.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.Search(""); // Get all users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _userService.Search("").CountAsync();
            
            return Ok(new 
            { 
                users = users,
                pagination = new 
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var users = await _userService.Search(name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = (await _userService.Search(name)).Count;
            
            return Ok(new 
            { 
                users = users,
                searchTerm = name,
                pagination = new 
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
                return BadRequest(new { message = "Name is required" });

            var createdUser = await _userService.CreateUser(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.Id },
                new 
                { 
                    message = "User created successfully",
                    data = createdUser
                });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            var updated = await _userService.UpdateUser(id, user);

            if (!updated)
                return NotFound(new { message = $"User {id} not found" });

            return Ok(new
            {
                message = $"User {id} updated",
                updatedData = user
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUser(id);

            if (!deleted)
                return NotFound(new { message = $"User {id} not found" });

            return Ok(new
            {
                message = $"User {id} deleted"
            });
        }

        [HttpGet("{id:int}/profile")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            // Return user profile without sensitive data
            return Ok(new 
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                createdAt = user.CreatedAt
            });
        }
    }
}
