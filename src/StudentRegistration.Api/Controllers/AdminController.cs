using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
  private readonly IAdminService _admin;

  public AdminController(IAdminService admin) => _admin = admin;

  // ============================================================
  // STUDENTS
  // ============================================================
  [HttpGet("students")]
  public async Task<IActionResult> GetStudents() => Ok(await _admin.GetStudentsAsync());

  [HttpGet("students/{id:int}")]
  public async Task<IActionResult> GetStudent(int id) => Ok(await _admin.GetStudentByIdAsync(id));

  [HttpPost("students")]
  public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
      => Ok(await _admin.CreateStudentAsync(request));

  [HttpPut("students/{id:int}")]
  public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentRequest request)
      => Ok(await _admin.UpdateStudentAsync(id, request));

  [HttpPut("students/{id:int}/reset-pin")]
  public async Task<IActionResult> ResetPin(int id, [FromBody] ResetPinRequest request)
  {
    await _admin.ResetStudentPinAsync(id, request);
    return NoContent();
  }

  [HttpDelete("students/{id:int}")]
  public async Task<IActionResult> DeleteStudent(int id)
  {
    await _admin.DeleteStudentAsync(id);
    return NoContent();
  }

  // ============================================================
  // REGISTRATION PERIODS
  // ============================================================
  [HttpGet("registration-periods")]
  public async Task<IActionResult> GetPeriods() => Ok(await _admin.GetRegistrationPeriodsAsync());

  [HttpPost("registration-periods")]
  public async Task<IActionResult> CreatePeriod([FromBody] CreateRegistrationPeriodRequest request)
      => Ok(await _admin.CreateRegistrationPeriodAsync(request));

  [HttpPut("registration-periods/{id:int}")]
  public async Task<IActionResult> UpdatePeriod(int id, [FromBody] UpdateRegistrationPeriodRequest request)
      => Ok(await _admin.UpdateRegistrationPeriodAsync(id, request));

  [HttpPut("registration-periods/{id:int}/open")]
  public async Task<IActionResult> OpenPeriod(int id)
      => Ok(await _admin.OpenRegistrationPeriodAsync(id));

  [HttpPut("registration-periods/{id:int}/close")]
  public async Task<IActionResult> ClosePeriod(int id)
      => Ok(await _admin.CloseRegistrationPeriodAsync(id));

  [HttpDelete("registration-periods/{id:int}")]
  public async Task<IActionResult> DeletePeriod(int id)
  {
    await _admin.DeleteRegistrationPeriodAsync(id);
    return NoContent();
  }

  // ============================================================
  // SEMESTERS
  // ============================================================
  [HttpGet("semesters")]
  public async Task<IActionResult> GetSemesters() => Ok(await _admin.GetSemestersAsync());

  [HttpPost("semesters")]
  public async Task<IActionResult> CreateSemester([FromBody] CreateSemesterRequest request)
      => Ok(await _admin.CreateSemesterAsync(request));

  [HttpPut("semesters/{id:int}/set-current")]
  public async Task<IActionResult> SetCurrentSemester(int id)
      => Ok(await _admin.SetCurrentSemesterAsync(id));

  // ============================================================
  // COURSES
  // ============================================================
  [HttpGet("courses")]
  public async Task<IActionResult> GetCourses() => Ok(await _admin.GetCoursesAsync());

  [HttpGet("courses/{id:int}")]
  public async Task<IActionResult> GetCourse(int id) => Ok(await _admin.GetCourseByIdAsync(id));

  [HttpPost("courses")]
  public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
      => Ok(await _admin.CreateCourseAsync(request));

  [HttpPut("courses/{id:int}")]
  public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
      => Ok(await _admin.UpdateCourseAsync(id, request));

  [HttpDelete("courses/{id:int}")]
  public async Task<IActionResult> DeleteCourse(int id)
  {
    await _admin.DeleteCourseAsync(id);
    return NoContent();
  }

  // ============================================================
  // SECTIONS
  // ============================================================
  [HttpGet("sections")]
  public async Task<IActionResult> GetSections([FromQuery] int? semesterId)
      => Ok(await _admin.GetSectionsAsync(semesterId));

  [HttpGet("sections/{id:int}")]
  public async Task<IActionResult> GetSection(int id)
      => Ok(await _admin.GetSectionByIdAsync(id));

  [HttpPost("sections")]
  public async Task<IActionResult> CreateSection([FromBody] CreateSectionRequest request)
      => Ok(await _admin.CreateSectionAsync(request));

  [HttpPut("sections/{id:int}")]
  public async Task<IActionResult> UpdateSection(int id, [FromBody] UpdateSectionRequest request)
      => Ok(await _admin.UpdateSectionAsync(id, request));

  [HttpDelete("sections/{id:int}")]
  public async Task<IActionResult> DeleteSection(int id)
  {
    await _admin.DeleteSectionAsync(id);
    return NoContent();
  }

  // ============================================================
  // LOOKUPS
  // ============================================================
  [HttpGet("departments")]
  public async Task<IActionResult> GetDepartments() => Ok(await _admin.GetDepartmentsAsync());

  [HttpPost("departments")]
  public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
      => Ok(await _admin.CreateDepartmentAsync(request));

  [HttpGet("instructors")]
  public async Task<IActionResult> GetInstructors() => Ok(await _admin.GetInstructorsAsync());

  [HttpPost("instructors")]
  public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
      => Ok(await _admin.CreateInstructorAsync(request));

  [HttpGet("teaching-assistants")]
  public async Task<IActionResult> GetTAs() => Ok(await _admin.GetTeachingAssistantsAsync());

  [HttpPost("teaching-assistants")]
  public async Task<IActionResult> CreateTA([FromBody] CreateTARequest request)
      => Ok(await _admin.CreateTeachingAssistantAsync(request));
}