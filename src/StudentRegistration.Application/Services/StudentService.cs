using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Application.Services;

public class StudentService : IStudentService
{
  private readonly IAppDbContext _db;

  public StudentService(IAppDbContext db) => _db = db;

  public async Task<StudentProfileDto> GetProfileAsync(int studentId)
  {
    var student = await _db.Students.AsNoTracking()
        .Include(s => s.Department).ThenInclude(d => d.College)
        .Include(s => s.StudyPlan)
        .FirstOrDefaultAsync(s => s.Id == studentId)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    return new StudentProfileDto
    {
      Id = student.Id,
      RegistrationNumber = student.RegistrationNumber,
      FullName = student.FullName,
      CollegeName = student.Department.College.Name,
      DepartmentName = student.Department.Name,
      StudyPlanName = student.StudyPlan?.Name,
      Status = student.Status.ToString(),
      Level = student.Level.ToString(),
      GPA = student.GPA,
      TotalAchievement = student.TotalAchievement,
      Sponsor = student.Sponsor
    };
  }

  public async Task<StudentDashboardDto> GetDashboardAsync(int studentId)
  {
    var profile = await GetProfileAsync(studentId);

    var student = await _db.Students.AsNoTracking()
        .FirstAsync(s => s.Id == studentId);

    var currentSemester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent);

    var dashboard = new StudentDashboardDto
    {
      Profile = profile,
      CurrentSemester = currentSemester is null ? null : new CurrentSemesterDto
      {
        Id = currentSemester.Id,
        Name = currentSemester.Name,
        StartDate = currentSemester.StartDate,
        EndDate = currentSemester.EndDate
      }
    };

    if (currentSemester is null)
    {
      dashboard.CanRegister = false;
      dashboard.RegistrationBlockReason = "No current semester is set.";
      return dashboard;
    }

    if (student.Status != StudentStatus.Active)
    {
      dashboard.CanRegister = false;
      dashboard.RegistrationBlockReason =
          $"Your student status is '{student.Status}'. Only active students can register.";
      return dashboard;
    }

    var period = await _db.RegistrationPeriods.AsNoTracking()
        .FirstOrDefaultAsync(rp => rp.DepartmentId == student.DepartmentId
                                && rp.SemesterId == currentSemester.Id);

    if (period is null)
    {
      dashboard.CanRegister = false;
      dashboard.RegistrationBlockReason = "No registration period configured for your department.";
      return dashboard;
    }

    var now = DateTime.UtcNow;
    if (!period.IsOpen || now < period.StartDate || now > period.EndDate)
    {
      dashboard.CanRegister = false;
      dashboard.RegistrationBlockReason = "Registration is currently closed for your department.";
      return dashboard;
    }

    dashboard.CanRegister = true;
    return dashboard;
  }
}