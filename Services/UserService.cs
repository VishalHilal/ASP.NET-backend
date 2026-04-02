using AIImageGeneratorBackend.Data;
using AIImageGeneratorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AIImageGeneratorBackend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public UserService(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> Search(string name)
        {
            return await _context.Users
                .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<User> CreateUser(User user)
        {
            user.Password = _passwordService.HashPassword(user.Password);
            user.Id = 0; // Let database generate the ID
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> RegisterUser(RegisterRequest request)
        {
            // Check if email already exists
            var existingUser = await GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                return null; // Email already exists
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                CreatedAt = DateTime.UtcNow
            };

            return await CreateUser(user);
        }

        public async Task<bool> UpdateUser(int id, User user)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                return false;

            existing.Name = user.Name;
            existing.Email = user.Email;
            
            if (!string.IsNullOrEmpty(user.Password))
            {
                existing.Password = _passwordService.HashPassword(user.Password);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await GetUserByEmail(email);
            if (user == null)
                return null;

            var isPasswordValid = _passwordService.VerifyPassword(password, user.Password);
            return isPasswordValid ? user : null;
        }
    }
}
