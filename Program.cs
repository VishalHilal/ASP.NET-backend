var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", ()=>{
		return new{
		message = "test backend"
		};
		});
app.Run();
