using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> StudentLogin([FromBody] LoginRequest request)
  {
    var result = await _authService.StudentLoginAsync(request);
    return Ok(result);
  }

  [HttpPost("admin-login")]
  [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> AdminLogin([FromBody] AdminLoginRequest request)
  {
    var result = await _authService.AdminLoginAsync(request);
    return Ok(result);
  }
}