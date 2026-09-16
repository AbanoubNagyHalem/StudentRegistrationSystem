using FluentAssertions;
using Microsoft.Extensions.Configuration;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;
using StudentRegistration.UnitTests.Helpers;
using Xunit;

namespace StudentRegistration.UnitTests.Services;

public class AuthServiceTests
{
  private static IConfiguration BuildConfig()
  {
    var dict = new Dictionary<string, string?>
    {
      ["Jwt:Issuer"] = "TestIssuer",
      ["Jwt:Audience"] = "TestAudience",
      ["Jwt:Key"] = "TEST_KEY_AT_LEAST_32_CHARS_LONG_FOR_HMAC_SHA256_1234567890",
      ["Jwt:ExpiryHours"] = "8"
    };
    return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
  }

  private static Student MakeStudent(string regNum, string pin, StudentStatus status = StudentStatus.Active)
      => new()
      {
        RegistrationNumber = regNum,
        FullName = "Test Student",
        PinHash = BCrypt.Net.BCrypt.HashPassword(pin),
        DepartmentId = 1,
        Status = status
      };

  [Fact]
  public async Task StudentLogin_WithValidCredentials_ReturnsToken()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(StudentLogin_WithValidCredentials_ReturnsToken));
    db.Students.Add(MakeStudent("20240001", "1234"));
    await db.SaveChangesAsync();

    var service = new AuthService(db, BuildConfig());

    var result = await service.StudentLoginAsync(new LoginRequest
    {
      RegistrationNumber = "20240001",
      Pin = "1234"
    });

    result.Token.Should().NotBeNullOrEmpty();
    result.Role.Should().Be("Student");
    result.RegistrationNumber.Should().Be("20240001");
  }

  [Fact]
  public async Task StudentLogin_WithWrongPin_ThrowsUnauthorized()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(StudentLogin_WithWrongPin_ThrowsUnauthorized));
    db.Students.Add(MakeStudent("20240001", "1234"));
    await db.SaveChangesAsync();

    var service = new AuthService(db, BuildConfig());

    var act = async () => await service.StudentLoginAsync(new LoginRequest
    {
      RegistrationNumber = "20240001",
      Pin = "9999"
    });

    await act.Should().ThrowAsync<UnauthorizedException>();
  }

  [Fact]
  public async Task StudentLogin_WithUnknownRegNumber_ThrowsUnauthorized()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(StudentLogin_WithUnknownRegNumber_ThrowsUnauthorized));
    var service = new AuthService(db, BuildConfig());

    var act = async () => await service.StudentLoginAsync(new LoginRequest
    {
      RegistrationNumber = "99999999",
      Pin = "1234"
    });

    await act.Should().ThrowAsync<UnauthorizedException>();
  }

  [Fact]
  public async Task StudentLogin_WithEmptyFields_ThrowsBusinessRule()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(StudentLogin_WithEmptyFields_ThrowsBusinessRule));
    var service = new AuthService(db, BuildConfig());

    var act = async () => await service.StudentLoginAsync(new LoginRequest
    {
      RegistrationNumber = "",
      Pin = ""
    });

    await act.Should().ThrowAsync<BusinessRuleException>();
  }

  [Fact]
  public async Task AdminLogin_WithValidCredentials_ReturnsToken()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(AdminLogin_WithValidCredentials_ReturnsToken));
    db.AdminUsers.Add(new AdminUser
    {
      Username = "admin",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
      Role = "Admin"
    });
    await db.SaveChangesAsync();

    var service = new AuthService(db, BuildConfig());

    var result = await service.AdminLoginAsync(new AdminLoginRequest
    {
      Username = "admin",
      Password = "Admin@123"
    });

    result.Token.Should().NotBeNullOrEmpty();
    result.Role.Should().Be("Admin");
  }

  [Fact]
  public async Task AdminLogin_WithWrongPassword_ThrowsUnauthorized()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(AdminLogin_WithWrongPassword_ThrowsUnauthorized));
    db.AdminUsers.Add(new AdminUser
    {
      Username = "admin",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
      Role = "Admin"
    });
    await db.SaveChangesAsync();

    var service = new AuthService(db, BuildConfig());

    var act = async () => await service.AdminLoginAsync(new AdminLoginRequest
    {
      Username = "admin",
      Password = "wrong"
    });

    await act.Should().ThrowAsync<UnauthorizedException>();
  }
}