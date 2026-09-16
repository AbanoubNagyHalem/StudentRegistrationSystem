using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Application.Interfaces;

public interface IStudentService
{
  Task<StudentProfileDto> GetProfileAsync(int studentId);
  Task<StudentDashboardDto> GetDashboardAsync(int studentId);
}