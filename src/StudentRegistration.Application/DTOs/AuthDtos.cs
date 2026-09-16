namespace StudentRegistration.Application.DTOs;

public class LoginRequest
{
  public string RegistrationNumber { get; set; } = string.Empty;
  public string Pin { get; set; } = string.Empty;
}

public class AdminLoginRequest
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
  public string Token { get; set; } = string.Empty;
  public DateTime ExpiresAt { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string? RegistrationNumber { get; set; }
  public int UserId { get; set; }
}