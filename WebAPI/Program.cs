using Data_Access_Layer.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("DATABASE_CONNECTION is not set!");
}
Console.WriteLine($"DATABASE_CONNECTION: {Environment.GetEnvironmentVariable("DATABASE_CONNECTION")}");


// Cấu hình DbContext với MySQL


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 31)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null // Set this to null or an empty collection if no specific error numbers are needed.
        )
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
