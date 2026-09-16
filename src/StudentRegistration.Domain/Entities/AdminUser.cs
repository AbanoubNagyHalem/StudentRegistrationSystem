using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class AdminUser : BaseEntity
{
  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public string Role { get; set; } = "Admin";
}