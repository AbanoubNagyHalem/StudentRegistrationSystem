using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Application.Services;

public class ScheduleService : IScheduleService
{
  private readonly IAppDbContext _db;

  public ScheduleService(IAppDbContext db)
  {
    _db = db;
  }

  public async Task<ScheduleDto> GetCurrentSemesterScheduleAsync(int studentId)
  {
    var student = await _db.Students.AsNoTracking()
        .Include(s => s.Department)
        .FirstOrDefaultAsync(s => s.Id == studentId)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    var currentSemester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent)
        ?? throw new NotFoundException("No current semester found.", "NO_CURRENT_SEMESTER");

    var enrollments = await _db.Enrollments
        .AsNoTracking()
        .Include(e => e.Section).ThenInclude(s => s.Course)
        .Include(e => e.Section).ThenInclude(s => s.Instructor)
        .Include(e => e.Section).ThenInclude(s => s.TeachingAssistant)
        .Include(e => e.Section).ThenInclude(s => s.Schedules)
        .Where(e => e.StudentId == studentId
                 && e.Section.SemesterId == currentSemester.Id
                 && e.Status == EnrollmentStatus.Registered)
        .ToListAsync();

    var items = new List<ScheduleItemDto>();
    var totalCredits = 0;

    foreach (var e in enrollments)
    {
      foreach (var sch in e.Section.Schedules)
      {
        items.Add(new ScheduleItemDto
        {
          CourseCode = e.Section.Course.Code,
          CourseName = e.Section.Course.Name,
          SectionNumber = e.Section.SectionNumber,
          InstructorName = e.Section.Instructor.Name,
          TeachingAssistantName = e.Section.TeachingAssistant?.Name,
          DayOfWeek = sch.DayOfWeek.ToString(),
          StartTime = sch.StartTime.ToString("HH:mm"),
          EndTime = sch.EndTime.ToString("HH:mm"),
          Room = sch.Room
        });
      }

      totalCredits += e.Section.Course.CreditHours;
    }

    return new ScheduleDto
    {
      StudentId = student.Id,
      StudentName = student.FullName,
      RegistrationNumber = student.RegistrationNumber,
      DepartmentName = student.Department.Name,
      SemesterName = currentSemester.Name,
      TotalCreditHours = totalCredits,
      Items = items
    };
  }
}