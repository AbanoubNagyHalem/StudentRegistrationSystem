using System.Security.Claims;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Services;

public class CurrentUserService : ICurrentUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUserService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public int? UserId
  {
    get
    {
      var value = _httpContextAccessor.HttpContext?.User
          .FindFirstValue(ClaimTypes.NameIdentifier);
      return int.TryParse(value, out var id) ? id : null;
    }
  }

  public string? RegistrationNumber =>
      _httpContextAccessor.HttpContext?.User
          .FindFirstValue("regnum");

  public string? Role =>
      _httpContextAccessor.HttpContext?.User
          .FindFirstValue(ClaimTypes.Role);

  public bool IsAuthenticated =>
      _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}