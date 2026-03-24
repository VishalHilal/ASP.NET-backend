var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => {
    return new {
        message = "test backend"
    };
});


app.MapGet("/user/{id}", (int id) => {
    return new {
        userId = id,
        name = "Vishal"
    };
});

app.MapGet("/search", (string name) => {
    return new {
        query = name,
        result = $"Searching for {name}"
    };
});

app.MapPost("/user", (User user) => {
    return new {
        message = "User created successfully",
        data = user
    };
});

app.MapPut("/user/{id}", (int id, User user) => {
    return new {
        message = $"User {id} updated",
        updatedData = user
    };
});

app.MapDelete("/user/{id}", (int id) => {
    return new {
        message = $"User {id} deleted"
    };
});

app.Run();

public record User(int Id, string Name, string Email);
