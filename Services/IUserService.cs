using AIImageGeneratorBackend.Models;

namespace AIImageGeneratorBackend.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByEmail(string email);
        Task<List<User>> Search(string name);
        Task<User> CreateUser(User user);
        Task<User?> RegisterUser(RegisterRequest request);
        Task<bool> UpdateUser(int id, User user);
        Task<bool> DeleteUser(int id);
        Task<User?> AuthenticateUser(string email, string password);
    }
}
