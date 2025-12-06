using FarmManager.Application.Common.Interfaces;
using FarmManager.Application.Common.Models.Auth;
using FarmManager.Application.Common.Repositories;
using FarmManager.Application.Common.Services;
using FarmManager.Application.Services;




// --- ВОТ ОНИ ---
using FarmManager.Infrastructure.Persistence;
using FarmManager.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
// ---
using Microsoft.Extensions.FileProviders; // <-- ДЛЯ ФАЙЛОВ
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Получаем строку подключения ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// --- 2. Регистрация DbContext (из Infrastructure) ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 3. РЕГИСТРАЦИЯ РЕПОЗИТОРИЕВ И UNIT OF WORK ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICowRepository, CowRepository>();
builder.Services.AddScoped<IWeightRecordRepository, WeightRecordRepository>();
builder.Services.AddScoped<IMilkYieldRepository, MilkYieldRepository>();
builder.Services.AddScoped<IFoodConsumptionRepository, FoodConsumptionRepository>();


// --- 4. РЕГИСТРАЦИЯ НАШИх СЕРВИСОВ ---
// Auth
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Cows
builder.Services.AddScoped<ICowService, CowService>();
// Production
builder.Services.AddScoped<IProductionService, ProductionService>();

// --- 4.1. РЕГИСТРАЦИЯ СЕРВИСОВ ИНФРАСТРУКТУРЫ И КЛИЕНТОВ ---
// Регистрируем типизированный HttpClient, как ты и хотел (без фабрики)
builder.Services.AddHttpClient<IFlaskMlClient, FlaskMlClient>();
builder.Services.AddScoped<IFileStorage, CloudinaryService>(); // <-- ДОБАВЛЯЕМ НОВЫЙ


// --- 5. Стандартные сервисы API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- 5.1 Настройка SWAGGER для JWT ---
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "FarmManager API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// --- 6. CORS (Обязательно!) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Адрес твоего React-приложения
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- 7. Http-клиент (для Flask) ---
// (Нам больше не нужен `AddHttpClient("FlaskClient")`, так как мы используем `AddHttpClient<T>`)

// --- 8. Настройка АУТЕНТИФИКАЦИИ (JWT) ---
var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Включаем CORS
app.UseCors("AllowReactApp");

// Включаем аутентиКАцию и авториЗАцию
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();