using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using StudentRegistration.Application.DTOs;
using Xunit;

namespace StudentRegistration.IntegrationTests;

public class ApiSmokeTests : IClassFixture<CustomWebApplicationFactory>
{
  private readonly CustomWebApplicationFactory _factory;

  // Demo credentials from DbSeeder
  private const string StudentRegNo = "20260001";
  private const string StudentPin = "1234";

  public ApiSmokeTests(CustomWebApplicationFactory factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task CurrentSemester_ReturnsSuccess()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/semesters/current");
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task StudentLogin_WithValidCredentials_ReturnsToken()
  {
    var client = _factory.CreateClient();
    var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      RegistrationNumber = StudentRegNo,
      Pin = StudentPin
    });

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
    body!.Token.Should().NotBeNullOrEmpty();
    body.Role.Should().Be("Student");
  }

  [Fact]
  public async Task StudentLogin_WithWrongPin_Returns401()
  {
    var client = _factory.CreateClient();
    var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      RegistrationNumber = StudentRegNo,
      Pin = "wrong"
    });

    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task ProtectedEndpoint_WithoutToken_Returns401()
  {
    var client = _factory.CreateClient();
    var response = await client.GetAsync("/api/students/me");
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task AdminEndpoint_WithStudentToken_Returns403()
  {
    // 1. Login as student using a fresh client
    var loginClient = _factory.CreateClient();
    var loginResponse = await loginClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
    {
      RegistrationNumber = StudentRegNo,
      Pin = StudentPin
    });

    loginResponse.StatusCode.Should().Be(HttpStatusCode.OK,
        "student login must succeed for this test to be meaningful");

    var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
    login.Should().NotBeNull();
    login!.Token.Should().NotBeNullOrEmpty();

    // 2. Use a SEPARATE client with the auth header
    var authedClient = _factory.CreateClient();
    authedClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", login.Token);

    // 3. Call admin endpoint
    var response = await authedClient.GetAsync("/api/admin/students");
    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
  }
}