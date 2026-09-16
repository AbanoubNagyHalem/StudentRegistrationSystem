using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;

public class StudentService
{
  private readonly ApiClient _api;

  public StudentService(ApiClient api) => _api = api;

  public Task<StudentProfileDto?> GetMyProfileAsync()
      => _api.GetAsync<StudentProfileDto>("/api/students/me");

  public Task<StudentDashboardDto?> GetMyDashboardAsync()
      => _api.GetAsync<StudentDashboardDto>("/api/students/me/dashboard");
}