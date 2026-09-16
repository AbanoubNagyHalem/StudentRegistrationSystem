using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/registration")]
[Authorize(Roles = "Student")]
public class RegistrationController : ControllerBase
{
  private readonly IRegistrationService _registrationService;
  private readonly ICurrentUserService _currentUser;

  public RegistrationController(
      IRegistrationService registrationService,
      ICurrentUserService currentUser)
  {
    _registrationService = registrationService;
    _currentUser = currentUser;
  }

  [HttpGet("eligibility")]
  [ProducesResponseType(typeof(RegistrationEligibilityDto), StatusCodes.Status200OK)]
  public async Task<IActionResult> CheckEligibility()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _registrationService.CheckEligibilityAsync(studentId));
  }

  [HttpGet("available-courses")]
  [ProducesResponseType(typeof(List<AvailableCourseDto>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAvailableCourses()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _registrationService.GetAvailableCoursesAsync(studentId));
  }

  [HttpGet("sections")]
  [ProducesResponseType(typeof(List<CourseSectionDto>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetSections([FromQuery] int courseId)
  {
    return Ok(await _registrationService.GetSectionsAsync(courseId));
  }

  [HttpPost("validate")]
  [ProducesResponseType(typeof(RegistrationValidationResult), StatusCodes.Status200OK)]
  public async Task<IActionResult> Validate([FromBody] ConfirmRegistrationRequest request)
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _registrationService.ValidateSelectionAsync(studentId, request.SectionIds));
  }

  [HttpPost("confirm")]
  [ProducesResponseType(typeof(RegistrationSummaryDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> Confirm([FromBody] ConfirmRegistrationRequest request)
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    return Ok(await _registrationService.ConfirmRegistrationAsync(studentId, request));
  }

  [HttpDelete("current")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> DeleteCurrent()
  {
    var studentId = _currentUser.UserId
        ?? throw new UnauthorizedException("Not authenticated.", "UNAUTHENTICATED");
    await _registrationService.DeleteCurrentRegistrationAsync(studentId);
    return NoContent();
  }
}