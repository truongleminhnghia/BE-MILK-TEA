using System.Text;
using System.Text.Json.Serialization;
using Business_Logic_Layer.AutoMappers;
using Business_Logic_Layer.Configurations;
using Business_Logic_Layer.Middleware;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientReviewService;
using Business_Logic_Layer.Services.IngredientService;
using Business_Logic_Layer.Services.NotificationService;
using Business_Logic_Layer.Services.PaymentService;
using Business_Logic_Layer.Services.VNPayService;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Data;
using Data_Access_Layer.Repositories;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
// using Business_Logic_Layer.Utils;
using Microsoft.OpenApi.Models;
using Business_Logic_Layer.Services.PromotionService;
using Business_Logic_Layer.Services.PromotionDetailService;
using Business_Logic_Layer.Services.DashboardService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.Configure<VNPayConfiguration>(builder.Configuration.GetSection("VNPay"));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Cấu hình Swagger để hỗ trợ Authorization bằng Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào trường bên dưới. Ví dụ: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


Env.Load();

// var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

var _server = Environment.GetEnvironmentVariable("SERVER_LOCAL");
var _port = Environment.GetEnvironmentVariable("PORT_LOCAL");
var _user = Environment.GetEnvironmentVariable("USER_LOCAL");
var _password = Environment.GetEnvironmentVariable("PASSWORD_LOCAL");
var _databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME_LOCAL");
var _sslMode = Environment.GetEnvironmentVariable("SSLMODE");

// var connectionString =
//   $"Server={_server};Port={_port};User Id={_user};Password={_password};Database={_databaseName};SslMode={_sslMode};";

var connectionString = $"Server=yamabiko.proxy.rlwy.net;Port=46054;User Id=root;Password=LGcWZkUqzkkXPqlpOKnxUvykcQcVcIib;Database=DB_MILK_TEA;SslMode=Required;";

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
// if (string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_issuer))
// {
//   throw new InvalidOperationException("JWT environment variables are not set properly.");
// }

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
    typeof(CategoryMapper),
    typeof(PromotionDetailMapper)
);
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<Func<ICategoryService>>(provider =>
    () => provider.GetService<ICategoryService>()
);

// comment đến đây
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IIngredientProductService, IngredientProductService>();
builder.Services.AddScoped<IIngredientProductRepository, IngredientProductRepository>();
builder.Services.AddScoped<IIngredientQuantityService, IngredientQuantityService>();
builder.Services.AddScoped<IIngredientQuantityRepository, IngredientQuantityRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IIngredientReviewRepository, IngredientReviewRepository>();
builder.Services.AddScoped<IIngredientReviewService, IngredientReviewService>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IIngredientRecipeRepository, IngredientRecipeRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();






// Register ImageRepository and ImageService
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<Source>();

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
            policy.WithOrigins("http://localhost:5173", "https://fe-milk-tea-project.vercel.app", "http://127.0.0.1:5500", "http://192.168.0.2:5173") // Replace with your frontend URL
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

//cấu hình tự động bỏ qua xác thực đối với một số endpoint / API cụ thể ngay từ Program.cs nếu lười dùng [AllowAnonymous] cho từng API
app.Use(
    async (context, next) =>
    {
        var path = context.Request.Path.Value.ToLower();

        var publicEndpoints = new[]
        {
            "/api/v1/auths/register",
            "/api/v1/auths/login",
            "/api/v1/auths/forgot-password",
        };

        // Nếu request thuộc API công khai, bỏ qua xác thực
        if (publicEndpoints.Any(endpoint => path.StartsWith(endpoint)))
        {
            await next();
            return;
        }

        await next();
    }
);

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
