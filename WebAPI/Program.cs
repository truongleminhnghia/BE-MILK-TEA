using Data_Access_Layer.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Business_Logic_Layer.AutoMappers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();
// var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

var _server = Environment.GetEnvironmentVariable("SERVER_LOCAL");
var _port = Environment.GetEnvironmentVariable("PORT_LOCAL");
var _user = Environment.GetEnvironmentVariable("USER_LOCAL");
var _password = Environment.GetEnvironmentVariable("PASSWORD_LOCAL");
var _databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME_LOCAL");
var _sslMode = Environment.GetEnvironmentVariable("SSLMODE");

var connectionString = $"Server={_server};Port={_port};User Id={_user};Password={_password};Database={_databaseName};SslMode={_sslMode};";

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


builder.Services.AddAutoMapper(typeof(AccountMapper));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
