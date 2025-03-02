using Moives.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddDatabase(builder.Configuration["DB:ConnectionString"]);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<ValidationMappingMiddleware>();
app.UseAuthorization();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DBInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
