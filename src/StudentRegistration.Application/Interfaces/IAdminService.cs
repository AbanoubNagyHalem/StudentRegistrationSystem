using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Application.Interfaces;

public interface IAdminService
{
  // Students
  Task<List<AdminStudentDto>> GetStudentsAsync();
  Task<AdminStudentDto> GetStudentByIdAsync(int id);
  Task<AdminStudentDto> CreateStudentAsync(CreateStudentRequest request);
  Task<AdminStudentDto> UpdateStudentAsync(int id, UpdateStudentRequest request);
  Task ResetStudentPinAsync(int id, ResetPinRequest request);
  Task DeleteStudentAsync(int id);

  // Registration Periods
  Task<List<AdminRegistrationPeriodDto>> GetRegistrationPeriodsAsync();
  Task<AdminRegistrationPeriodDto> CreateRegistrationPeriodAsync(CreateRegistrationPeriodRequest request);
  Task<AdminRegistrationPeriodDto> UpdateRegistrationPeriodAsync(int id, UpdateRegistrationPeriodRequest request);
  Task<AdminRegistrationPeriodDto> OpenRegistrationPeriodAsync(int id);
  Task<AdminRegistrationPeriodDto> CloseRegistrationPeriodAsync(int id);
  Task DeleteRegistrationPeriodAsync(int id);

  // Semesters
  Task<List<AdminSemesterDto>> GetSemestersAsync();
  Task<AdminSemesterDto> CreateSemesterAsync(CreateSemesterRequest request);
  Task<AdminSemesterDto> SetCurrentSemesterAsync(int id);

  // Courses
  Task<List<AdminCourseDto>> GetCoursesAsync();
  Task<AdminCourseDto> GetCourseByIdAsync(int id);
  Task<AdminCourseDto> CreateCourseAsync(CreateCourseRequest request);
  Task<AdminCourseDto> UpdateCourseAsync(int id, UpdateCourseRequest request);
  Task DeleteCourseAsync(int id);

  // Sections
  Task<List<AdminSectionDto>> GetSectionsAsync(int? semesterId);
  Task<AdminSectionDto> GetSectionByIdAsync(int id);
  Task<AdminSectionDto> CreateSectionAsync(CreateSectionRequest request);
  Task<AdminSectionDto> UpdateSectionAsync(int id, UpdateSectionRequest request);
  Task DeleteSectionAsync(int id);

  // Lookups
  Task<List<AdminDepartmentDto>> GetDepartmentsAsync();
  Task<AdminDepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request);

  Task<List<AdminInstructorDto>> GetInstructorsAsync();
  Task<AdminInstructorDto> CreateInstructorAsync(CreateInstructorRequest request);

  Task<List<AdminTADto>> GetTeachingAssistantsAsync();
  Task<AdminTADto> CreateTeachingAssistantAsync(CreateTARequest request);
}