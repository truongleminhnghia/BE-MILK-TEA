using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Implements;
using Repositories.Interfaces;
using Services.Implements;
using Services.Interfaces;
using dotenv.net;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DotEnv.Load();
DotNetEnv.Env.Load();
// var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("⚠️ DATABASE_CONNECTION không được nạp từ .env!");
}
else
{
    Console.WriteLine($"✅ Đã nạp DATABASE_CONNECTION: {connectionString}");
}

// Cấu hình DbContext với MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Using connection string: {connectionString}");

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


// builder.Services.AddDbContext<ApplicationDbContext>(options =>
// {
//     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//     Console.WriteLine($"Using connection string: {connectionString}");

//     options.UseMySql(
//         connectionString,
//         new MySqlServerVersion(new Version(8, 0, 31)),
//         mySqlOptions => mySqlOptions.EnableRetryOnFailure(
//             maxRetryCount: 5,
//             maxRetryDelay: TimeSpan.FromSeconds(10),
//             errorNumbersToAdd: null // Set this to null or an empty collection if no specific error numbers are needed.
//         )
//     );
// });

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
