using System.Text;
using System.Text.Json.Serialization;
using Business_Logic_Layer.AutoMappers;
using Business_Logic_Layer.Middleware;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Data;
using Data_Access_Layer.Repositories;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Env.Load();

//var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

var _server = Environment.GetEnvironmentVariable("SERVER_LOCAL");
var _port = Environment.GetEnvironmentVariable("PORT_LOCAL");
var _user = Environment.GetEnvironmentVariable("USER_LOCAL");
var _password = Environment.GetEnvironmentVariable("PASSWORD_LOCAL");
var _databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME_LOCAL");
var _sslMode = Environment.GetEnvironmentVariable("SSLMODE");

var connectionString = $"Server={_server};Port={_port};User Id={_user};Password={_password};Database={_databaseName};SslMode={_sslMode};";
//var connectionString = $"Server=localhost;Port=3306;User Id=root;Password=Pass;Database=DB_MILK_TEA;SslMode=Required;";

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("DATABASE_CONNECTION is not set!");
}
Console.WriteLine($"DATABASE_CONNECTION: {connectionString}");

// Cấu hình DbContext với MySQL

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 31)),
        mySqlOptions =>
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null // Set this to null or an empty collection if no specific error numbers are needed.
            )
    );
});

// lấy biến JWT từ môi trường
var _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

// kiểm tra xem, nó có tồn tai hay khoong
//muốn chạy thì comment từ đây lại, + xóa Migration
if (string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_issuer))
{
    throw new InvalidOperationException("JWT environment variables are not set properly.");
}

// đăng kí xác thực
builder
    .Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // mặc định cơ chế xác thực Bearer (JWT)
        // hiểu nôm na đơn giản là khi một người dùng yêu cầu đến API, hệ thống sẽ kiểm tra JWT token trong Authorization owrphaanf header,
        // nếu không có hoặc không hợp lệ sẽ nhận lỗi 401
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "roleName",
            ValidateIssuer = true, // người, chỗ, nơi phát hành, tức là tk cho tạo token
            ValidateAudience = true, // đối tượng sử dụng token
            ValidateLifetime = true, // kiểm tra thời gian hết hạn
            ValidateIssuerSigningKey = true, // kiểm tra khóa primate dùng để sign
            ValidIssuer = _issuer, // giá trị phá hành được lấy từ biến môi trường
            ValidAudience = _audience, // tương tự
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_secretKey)
            ) // phải mã khóa serect_key lại nhé
            ,
        };
    });

// Add services to the container.
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddAutoMapper(
    typeof(AccountMapper),
    typeof(CategoryMapper),
    typeof(IngredientMapper),
    typeof(ImageMapper),
    typeof(IngredientProductMapper),
    typeof(AccountMapper),
    typeof(CategoryMapper)
);
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<Func<ICategoryService>>(provider =>
    () => provider.GetService<ICategoryService>()
);
// comment đến đây
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IIngredientProductService, IngredientProductService>();
builder.Services.AddScoped<IIngredientProductRepository, IngredientProductRepository>();

// Register ImageRepository and ImageService
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();


// config CORS
var MyAllowSpecificOrigins = "_feAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        MyAllowSpecificOrigins,
        policy =>
        {

            policy.WithOrigins("http://localhost:5173", "https://fe-milk-tea-project.vercel.app") // Replace with your frontend URL
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddHttpClient<AuthenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors(MyAllowSpecificOrigins);

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(
        2
    ) // 2 phút là khoảng tg để client - server kết nối
    ,
};
app.UseWebSockets();
app.Run();
