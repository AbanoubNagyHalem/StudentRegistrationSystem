namespace StudentRegistration.Application.Interfaces;


public interface ICurrentUserService
{
  int? UserId { get; }
  string? RegistrationNumber { get; }
  string? Role { get; }
  bool IsAuthenticated { get; }
}