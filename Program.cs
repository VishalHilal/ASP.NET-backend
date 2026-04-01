using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AIImageGeneratorBackend.Data;
using AIImageGeneratorBackend.Services;
using AIImageGeneratorBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "AIImageGeneratorBackend";
var audience = builder.Configuration["Jwt:Audience"] ?? "AIImageGeneratorUsers";




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddAuthentication(options =>{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
.AddJwtBearer( options =>{
		options.RequireHttpsMetadata = false;
		options.SaveToken = true;
		options.TokenValidationParameters = new TokenValidationParameters{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,

		ValidIssuer = issuer,
		ValidAudience = audience,

		IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtKey)
				)
		};
		}
		);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapGet("/", async () =>
{
    return Results.Ok(new { message = "Test Backend Running 🚀" });
});


app.MapPost("/register", async (RegisterRequest request, IUserService userService) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || 
        string.IsNullOrWhiteSpace(request.Email) || 
        string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { message = "All fields are required" });
    }

    var existingUser = await userService.GetUserByEmail(request.Email);
    if (existingUser != null)
    {
        return Results.BadRequest(new { message = "Email already exists" });
    }

    var user = await userService.RegisterUser(request);
    if (user == null)
    {
        return Results.BadRequest(new { message = "Registration failed" });
    }

    return Results.Ok(new 
    { 
        message = "User registered successfully",
        userId = user.Id,
        email = user.Email,
        name = user.Name
    });
});


app.MapPost("/login", async (LoginRequest request, IUserService userService, JwtService jwtService) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { message = "Email and password are required" });
    }

    var user = await userService.AuthenticateUser(request.Username, request.Password);
    if (user == null)
    {
        return Results.Unauthorized();
    }

    var token = jwtService.GenerateToken(user.Email);
    
    return Results.Ok(new { 
        token = token,
        userId = user.Id,
        username = user.Email,
        name = user.Name,
        expiresIn = "1 hour"
    });
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
