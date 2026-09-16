using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentRegistration.Api.Middleware;
using StudentRegistration.Api.Services;
using StudentRegistration.Api.Settings;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Application.Services;
using StudentRegistration.Infrastructure.Data;
using StudentRegistration.Infrastructure.Data.Seed;


var builder = WebApplication.CreateBuilder(args);

// 1. Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// 2. EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("StudentRegistration.Infrastructure")));

// 2b. Expose AppDbContext through interface for Application layer
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());


// 3. JWT Settings binding
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

// 4. JWT Authentication
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
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// 5. HttpContextAccessor (for ICurrentUserService)
builder.Services.AddHttpContextAccessor();

// 6. Application Services (DI)
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// 7. CORS for Blazor WASM (dev + docker)
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5174",   // ← Docker client
                "http://localhost:8080",
                "http://localhost:80",
                "http://client",           // ← Docker network service name
                "http://localhost:5000",
                "http://localhost:5001",
                "http://localhost:5200",
                "http://localhost:5201",
                "https://localhost:7000",
                "https://localhost:7001",
                "https://localhost:7200",
                "https://localhost:7201")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// 8. Apply migrations + seed (dev only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// 9. HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 10. Global exception handling (first!)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("BlazorClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Needed for Integration Tests
public partial class Program { }