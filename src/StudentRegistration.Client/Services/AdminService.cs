using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;

public class AdminService
{
  private readonly ApiClient _api;

  public AdminService(ApiClient api) => _api = api;

  // Students
  public Task<List<AdminStudentDto>?> GetStudentsAsync()
      => _api.GetAsync<List<AdminStudentDto>>("/api/admin/students");

  // Registration Periods
  public Task<List<AdminRegistrationPeriodDto>?> GetPeriodsAsync()
      => _api.GetAsync<List<AdminRegistrationPeriodDto>>("/api/admin/registration-periods");

  public Task<AdminRegistrationPeriodDto?> OpenPeriodAsync(int id)
      => _api.PutAsync<AdminRegistrationPeriodDto>($"/api/admin/registration-periods/{id}/open");

  public Task<AdminRegistrationPeriodDto?> ClosePeriodAsync(int id)
      => _api.PutAsync<AdminRegistrationPeriodDto>($"/api/admin/registration-periods/{id}/close");

  // Semesters
  public Task<List<AdminSemesterDto>?> GetSemestersAsync()
      => _api.GetAsync<List<AdminSemesterDto>>("/api/admin/semesters");

  // Courses
  public Task<List<AdminCourseDto>?> GetCoursesAsync()
      => _api.GetAsync<List<AdminCourseDto>>("/api/admin/courses");

  // Sections
  public Task<List<AdminSectionDto>?> GetSectionsAsync(int? semesterId = null)
  {
    var path = semesterId.HasValue
        ? $"/api/admin/sections?semesterId={semesterId}"
        : "/api/admin/sections";
    return _api.GetAsync<List<AdminSectionDto>>(path);
  }

  // Lookups
  public Task<List<AdminDepartmentDto>?> GetDepartmentsAsync()
      => _api.GetAsync<List<AdminDepartmentDto>>("/api/admin/departments");

  public Task<List<AdminInstructorDto>?> GetInstructorsAsync()
      => _api.GetAsync<List<AdminInstructorDto>>("/api/admin/instructors");

  public Task<List<AdminTADto>?> GetTeachingAssistantsAsync()
      => _api.GetAsync<List<AdminTADto>>("/api/admin/teaching-assistants");
}