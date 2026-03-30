using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AIImageGeneratorBackend.Data;
using AIImageGeneratorBackend.Services;
using AIImageGeneratorBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


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
