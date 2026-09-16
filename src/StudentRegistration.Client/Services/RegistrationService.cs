using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;

public class RegistrationService
{
  private readonly ApiClient _api;

  public RegistrationService(ApiClient api) => _api = api;

  public Task<RegistrationEligibilityDto?> CheckEligibilityAsync()
      => _api.GetAsync<RegistrationEligibilityDto>("/api/registration/eligibility");

  public Task<List<AvailableCourseDto>?> GetAvailableCoursesAsync()
      => _api.GetAsync<List<AvailableCourseDto>>("/api/registration/available-courses");

  public Task<List<CourseSectionDto>?> GetSectionsAsync(int courseId)
      => _api.GetAsync<List<CourseSectionDto>>($"/api/registration/sections?courseId={courseId}");

  public Task<RegistrationValidationResult?> ValidateAsync(List<int> sectionIds)
      => _api.PostAsync<RegistrationValidationResult>("/api/registration/validate",
          new { sectionIds });

  public Task<RegistrationSummaryDto?> ConfirmAsync(List<int> sectionIds)
      => _api.PostAsync<RegistrationSummaryDto>("/api/registration/confirm",
          new { sectionIds });

  public Task DeleteCurrentAsync()
      => _api.DeleteAsync("/api/registration/current");
}