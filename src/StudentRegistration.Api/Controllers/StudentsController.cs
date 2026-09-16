using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/students")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
  private readonly IStudentService _studentService;
  private readonly ICurrentUserService _currentUser;

  public StudentsController(IStudentService studentService, ICurrentUserService currentUser)
  {
    _studentService = studentService;
    _currentUser = currentUser;
  }

  [HttpGet("me")]
  [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetMyProfile()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _studentService.GetProfileAsync(studentId));
  }

  [HttpGet("me/dashboard")]
  [ProducesResponseType(typeof(StudentDashboardDto), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetMyDashboard()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _studentService.GetDashboardAsync(studentId));
  }
}