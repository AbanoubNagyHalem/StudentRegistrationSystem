using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Application.Interfaces;

public interface IRegistrationService
{
  Task<RegistrationEligibilityDto> CheckEligibilityAsync(int studentId);
  Task<List<AvailableCourseDto>> GetAvailableCoursesAsync(int studentId);
  Task<List<CourseSectionDto>> GetSectionsAsync(int courseId);
  Task<RegistrationValidationResult> ValidateSelectionAsync(int studentId, List<int> sectionIds);
  Task<RegistrationSummaryDto> ConfirmRegistrationAsync(int studentId, ConfirmRegistrationRequest request);
  Task DeleteCurrentRegistrationAsync(int studentId);
}