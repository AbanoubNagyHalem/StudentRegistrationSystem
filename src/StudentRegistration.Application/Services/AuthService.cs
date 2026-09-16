using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;
namespace StudentRegistration.Application.Services;

public class AuthService : IAuthService
{
  private readonly IAppDbContext _db;
  private readonly IConfiguration _configuration;

  public AuthService(IAppDbContext db, IConfiguration configuration)
  {
    _db = db;
    _configuration = configuration;
  }

  public async Task<LoginResponse> StudentLoginAsync(LoginRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.RegistrationNumber) ||
        string.IsNullOrWhiteSpace(request.Pin))
    {
      throw new BusinessRuleException(
          "Registration number and PIN are required.", "INVALID_INPUT");
    }

    var student = await _db.Students
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.RegistrationNumber == request.RegistrationNumber);

    if (student is null)
      throw new UnauthorizedException(
          "Invalid registration number or PIN.", "INVALID_CREDENTIALS");

    // Verify BCrypt hash
    if (!BCrypt.Net.BCrypt.Verify(request.Pin, student.PinHash))
      throw new UnauthorizedException(
          "Invalid registration number or PIN.", "INVALID_CREDENTIALS");

    var (token, expiresAt) = GenerateJwt(
        student.Id,
        "Student",
        student.FullName,
        student.RegistrationNumber);

    return new LoginResponse
    {
      Token = token,
      ExpiresAt = expiresAt,
      UserId = student.Id,
      FullName = student.FullName,
      Role = "Student",
      RegistrationNumber = student.RegistrationNumber
    };
  }

  public async Task<LoginResponse> AdminLoginAsync(AdminLoginRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Password))
    {
      throw new BusinessRuleException(
          "Username and password are required.", "INVALID_INPUT");
    }

    var admin = await _db.AdminUsers
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Username == request.Username);

    if (admin is null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
      throw new UnauthorizedException(
          "Invalid username or password.", "INVALID_CREDENTIALS");

    var (token, expiresAt) = GenerateJwt(
        admin.Id,
        admin.Role,
        admin.Username,
        null);

    return new LoginResponse
    {
      Token = token,
      ExpiresAt = expiresAt,
      UserId = admin.Id,
      FullName = admin.Username,
      Role = admin.Role,
      RegistrationNumber = null
    };
  }

  private (string token, DateTime expiresAt) GenerateJwt(
      int userId, string role, string fullName, string? registrationNumber)
  {
    var jwtSection = _configuration.GetSection("Jwt");
    var key = jwtSection["Key"]
        ?? throw new InvalidOperationException("JWT Key is not configured.");
    var issuer = jwtSection["Issuer"];
    var audience = jwtSection["Audience"];
    var expiryHours = int.TryParse(jwtSection["ExpiryHours"], out var h) ? h : 8;

    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Role, role)
        };

    if (!string.IsNullOrEmpty(registrationNumber))
      claims.Add(new Claim("regnum", registrationNumber));

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: expiresAt,
        signingCredentials: creds);

    return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
  }
}