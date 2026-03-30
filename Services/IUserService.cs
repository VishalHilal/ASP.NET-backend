using AIImageGeneratorBackend.Models;

namespace AIImageGeneratorBackend.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
        Task<List<User>> Search(string name);
        Task<User> CreateUser(User user);
        Task<bool> UpdateUser(int id, User user);
        Task<bool> DeleteUser(int id);
    }
}
