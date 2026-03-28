using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();



app.MapGet("/", async () =>
{
    return Results.Ok(new { message = "Test Backend Running 🚀" });
});


app.MapGet("/user/{id}", async (int id, IUserService userService) =>
{
    var user = await userService.GetUserById(id);

    if (user == null)
        return Results.NotFound($"User with ID {id} not found");

    return Results.Ok(user);
});


app.MapGet("/search", async (string name, IUserService userService) =>
{
    var result = await userService.Search(name);
    return Results.Ok(result);
});


app.MapPost("/user", async (User user, IUserService userService) =>
{
    if (string.IsNullOrWhiteSpace(user.Name))
        return Results.BadRequest("Name is required");

    var createdUser = await userService.CreateUser(user);

    return Results.Ok(new
    {
        message = "User created successfully",
        data = createdUser
    });
});


app.MapPut("/user/{id}", async (int id, User user, IUserService userService) =>
{
    var updated = await userService.UpdateUser(id, user);

    if (!updated)
        return Results.NotFound($"User {id} not found");

    return Results.Ok(new
    {
        message = $"User {id} updated",
        updatedData = user
    });
});


app.MapDelete("/user/{id}", async (int id, IUserService userService) =>
{
    var deleted = await userService.DeleteUser(id);

    if (!deleted)
        return Results.NotFound($"User {id} not found");

    return Results.Ok(new
    {
        message = $"User {id} deleted"
    });
});


app.Run();



public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}



public interface IUserService
{
    Task<User?> GetUserById(int id);
    Task<List<User>> Search(string name);
    Task<User> CreateUser(User user);
    Task<bool> UpdateUser(int id, User user);
    Task<bool> DeleteUser(int id);
}



public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public Task<User?> GetUserById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<List<User>> Search(string name)
    {
        var result = _users
            .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(result);
    }

    public Task<User> CreateUser(User user)
    {
        user.Id = _users.Count + 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<bool> UpdateUser(int id, User user)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);

        if (existing == null)
            return Task.FromResult(false);

        existing.Name = user.Name;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return Task.FromResult(false);

        _users.Remove(user);
        return Task.FromResult(true);
    }
}
