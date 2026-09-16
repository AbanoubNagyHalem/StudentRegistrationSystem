using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/schedule")]
[Authorize(Roles = "Student")]
public class ScheduleController : ControllerBase
{
  private readonly IScheduleService _scheduleService;
  private readonly ICurrentUserService _currentUser;

  public ScheduleController(IScheduleService scheduleService, ICurrentUserService currentUser)
  {
    _scheduleService = scheduleService;
    _currentUser = currentUser;
  }

  [HttpGet("current")]
  [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetCurrentSchedule()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _scheduleService.GetCurrentSemesterScheduleAsync(studentId));
  }
}