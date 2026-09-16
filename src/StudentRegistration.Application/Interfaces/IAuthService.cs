using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Application.Interfaces;

public interface IAuthService
{
  Task<LoginResponse> StudentLoginAsync(LoginRequest request);
  Task<LoginResponse> AdminLoginAsync(AdminLoginRequest request);
}