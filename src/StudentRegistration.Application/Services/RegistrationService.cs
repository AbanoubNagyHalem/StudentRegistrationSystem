using System.Data;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Application.Services;

public class RegistrationService : IRegistrationService
{
  private const int MaxCreditHours = 18;

  private readonly IAppDbContext _db;

  public RegistrationService(IAppDbContext db)
  {
    _db = db;
  }

  // ------------------------------------------------------------------
  // 1. ELIGIBILITY
  // ------------------------------------------------------------------
  public async Task<RegistrationEligibilityDto> CheckEligibilityAsync(int studentId)
  {
    var student = await _db.Students.AsNoTracking()
        .FirstOrDefaultAsync(s => s.Id == studentId);

    if (student is null)
      return NotEligible("STUDENT_NOT_FOUND", "Student not found.");

    if (student.Status != StudentStatus.Active)
      return NotEligible("STUDENT_NOT_ACTIVE",
          $"Your student status is '{student.Status}'. Only active students can register.");

    var semester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent);

    if (semester is null)
      return NotEligible("NO_CURRENT_SEMESTER", "No current semester is set.");

    var period = await _db.RegistrationPeriods.AsNoTracking()
        .FirstOrDefaultAsync(rp => rp.DepartmentId == student.DepartmentId
                                && rp.SemesterId == semester.Id);

    if (period is null)
      return NotEligible("NO_REGISTRATION_PERIOD",
          "No registration period configured for your department.");

    var now = DateTime.UtcNow;
    if (!period.IsOpen)
      return NotEligible("REGISTRATION_CLOSED",
          "Registration is currently closed for your department.");

    if (now < period.StartDate || now > period.EndDate)
      return NotEligible("REGISTRATION_OUT_OF_WINDOW",
          $"Registration window is from {period.StartDate:yyyy-MM-dd} to {period.EndDate:yyyy-MM-dd}.");

    return new RegistrationEligibilityDto { IsEligible = true };
  }

  private static RegistrationEligibilityDto NotEligible(string code, string reason) =>
      new() { IsEligible = false, ErrorCode = code, Reason = reason };

  // ------------------------------------------------------------------
  // 2. AVAILABLE COURSES (filtered by study plan + prerequisites)
  // ------------------------------------------------------------------
  public async Task<List<AvailableCourseDto>> GetAvailableCoursesAsync(int studentId)
  {
    var student = await _db.Students.AsNoTracking()
        .FirstOrDefaultAsync(s => s.Id == studentId)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    if (student.StudyPlanId is null)
      throw new BusinessRuleException("No study plan assigned to this student.", "NO_STUDY_PLAN");

    var semester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent)
        ?? throw new NotFoundException("No current semester found.", "NO_CURRENT_SEMESTER");

    var completedCourseIds = await _db.StudentCompletedCourses.AsNoTracking()
        .Where(scc => scc.StudentId == studentId)
        .Select(scc => scc.CourseId)
        .ToListAsync();

    var planCourseIds = await _db.StudyPlanCourses.AsNoTracking()
        .Where(spc => spc.StudyPlanId == student.StudyPlanId)
        .Select(spc => spc.CourseId)
        .ToListAsync();

    var prerequisites = await _db.CoursePrerequisites.AsNoTracking()
        .Where(cp => planCourseIds.Contains(cp.CourseId))
        .Include(cp => cp.PrerequisiteCourse)
        .ToListAsync();

    var courses = await _db.Courses.AsNoTracking()
        .Where(c => planCourseIds.Contains(c.Id))
        .OrderBy(c => c.Code)
        .ToListAsync();

    var result = new List<AvailableCourseDto>();

    foreach (var course in courses)
    {
      var coursePrereqs = prerequisites.Where(p => p.CourseId == course.Id).ToList();
      var missing = coursePrereqs
          .Where(p => !completedCourseIds.Contains(p.PrerequisiteCourseId))
          .Select(p => $"{p.PrerequisiteCourse.Code} - {p.PrerequisiteCourse.Name}")
          .ToList();

      var sections = await GetSectionsInternalAsync(course.Id, semester.Id);

      result.Add(new AvailableCourseDto
      {
        CourseId = course.Id,
        Code = course.Code,
        Name = course.Name,
        CreditHours = course.CreditHours,
        IsAlreadyCompleted = completedCourseIds.Contains(course.Id),
        IsPrerequisitesMet = missing.Count == 0,
        PrerequisiteMissing = missing.Count == 0 ? null : string.Join(", ", missing),
        Sections = sections
      });
    }

    return result;
  }

  // ------------------------------------------------------------------
  // 3. GET SECTIONS
  // ------------------------------------------------------------------
  public async Task<List<CourseSectionDto>> GetSectionsAsync(int courseId)
  {
    var semester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent)
        ?? throw new NotFoundException("No current semester found.", "NO_CURRENT_SEMESTER");

    return await GetSectionsInternalAsync(courseId, semester.Id);
  }

  private async Task<List<CourseSectionDto>> GetSectionsInternalAsync(int courseId, int semesterId)
  {
    var sections = await _db.CourseSections.AsNoTracking()
        .Where(cs => cs.CourseId == courseId && cs.SemesterId == semesterId)
        .Include(cs => cs.Instructor)
        .Include(cs => cs.TeachingAssistant)
        .Include(cs => cs.Schedules)
        .OrderBy(cs => cs.SectionNumber)
        .ToListAsync();

    return sections.Select(s => new CourseSectionDto
    {
      Id = s.Id,
      SectionNumber = s.SectionNumber,
      Capacity = s.Capacity,
      EnrolledCount = s.EnrolledCount,
      IsFull = s.EnrolledCount >= s.Capacity,
      InstructorName = s.Instructor.Name,
      TeachingAssistantName = s.TeachingAssistant?.Name,
      Schedules = s.Schedules.Select(sch => new SectionScheduleDto
      {
        DayOfWeek = sch.DayOfWeek.ToString(),
        StartTime = sch.StartTime.ToString("HH:mm"),
        EndTime = sch.EndTime.ToString("HH:mm"),
        Room = sch.Room
      }).ToList()
    }).ToList();
  }

  // ------------------------------------------------------------------
  // 4. VALIDATE SELECTION (all business rules, but read-only)
  // ------------------------------------------------------------------
  public async Task<RegistrationValidationResult> ValidateSelectionAsync(int studentId, List<int> sectionIds)
  {
    var result = new RegistrationValidationResult { MaxCreditHours = MaxCreditHours };

    if (sectionIds is null || sectionIds.Count == 0)
    {
      result.Errors.Add("No sections selected.");
      return result;
    }

    if (sectionIds.Distinct().Count() != sectionIds.Count)
    {
      result.Errors.Add("Duplicate section selections are not allowed.");
      return result;
    }

    var eligibility = await CheckEligibilityAsync(studentId);
    if (!eligibility.IsEligible)
    {
      result.Errors.Add(eligibility.Reason ?? "Not eligible for registration.");
      return result;
    }

    var student = await _db.Students.AsNoTracking().FirstAsync(s => s.Id == studentId);
    var semester = await _db.Semesters.AsNoTracking().FirstAsync(s => s.IsCurrent);

    var sections = await _db.CourseSections.AsNoTracking()
        .Where(cs => sectionIds.Contains(cs.Id))
        .Include(cs => cs.Course)
        .Include(cs => cs.Schedules)
        .ToListAsync();

    if (sections.Count != sectionIds.Count)
    {
      result.Errors.Add("One or more selected sections no longer exist.");
      return result;
    }

    if (sections.Any(s => s.SemesterId != semester.Id))
      result.Errors.Add("One or more sections are not offered in the current semester.");

    // Study plan
    var planCourseIds = await _db.StudyPlanCourses.AsNoTracking()
        .Where(spc => spc.StudyPlanId == student.StudyPlanId)
        .Select(spc => spc.CourseId)
        .ToListAsync();

    foreach (var s in sections)
      if (!planCourseIds.Contains(s.CourseId))
        result.Errors.Add($"Course '{s.Course.Code}' is not part of your study plan.");

    // Completed
    var completedCourseIds = await _db.StudentCompletedCourses.AsNoTracking()
        .Where(scc => scc.StudentId == studentId)
        .Select(scc => scc.CourseId)
        .ToListAsync();

    foreach (var s in sections)
      if (completedCourseIds.Contains(s.CourseId))
        result.Errors.Add($"Course '{s.Course.Code}' is already completed.");

    // Prerequisites
    var sectionCourseIds = sections.Select(s => s.CourseId).ToList();
    var prereqs = await _db.CoursePrerequisites.AsNoTracking()
        .Where(cp => sectionCourseIds.Contains(cp.CourseId))
        .Include(cp => cp.PrerequisiteCourse)
        .ToListAsync();

    foreach (var s in sections)
    {
      var coursePrereqs = prereqs.Where(p => p.CourseId == s.CourseId);
      foreach (var p in coursePrereqs)
        if (!completedCourseIds.Contains(p.PrerequisiteCourseId))
          result.Errors.Add(
              $"Course '{s.Course.Code}' requires '{p.PrerequisiteCourse.Code}' as prerequisite.");
    }

    // Capacity
    foreach (var s in sections)
      if (s.EnrolledCount >= s.Capacity)
        result.Errors.Add(
            $"Section {s.SectionNumber} of '{s.Course.Code}' is FULL ({s.EnrolledCount}/{s.Capacity}).");

    // Duplicates already in current registration
    var existingEnrollments = await _db.Enrollments.AsNoTracking()
        .Where(e => e.StudentId == studentId
                 && e.Section.SemesterId == semester.Id
                 && e.Status == EnrollmentStatus.Registered)
        .Select(e => e.SectionId)
        .ToListAsync();

    foreach (var s in sections)
      if (existingEnrollments.Contains(s.Id))
        result.Errors.Add($"You are already registered in section {s.SectionNumber} of '{s.Course.Code}'.");

    // Credit limit
    var newCredits = sections.Sum(s => s.Course.CreditHours);
    result.TotalCreditHours = newCredits;

    if (newCredits > MaxCreditHours)
      result.Errors.Add(
          $"Total credit hours ({newCredits}) exceeds the maximum allowed ({MaxCreditHours}).");

    // Schedule conflicts
    var conflicts = DetectConflicts(sections);
    if (conflicts.Count > 0)
    {
      result.Conflicts = conflicts;
      foreach (var c in conflicts)
        result.Errors.Add(c.Message);
    }

    result.IsValid = result.Errors.Count == 0;
    return result;
  }

  private static List<ConflictDto> DetectConflicts(List<CourseSection> sections)
  {
    var conflicts = new List<ConflictDto>();

    var flattened = sections
        .SelectMany(s => s.Schedules.Select(sch => new { Section = s, Sch = sch }))
        .ToList();

    for (int i = 0; i < flattened.Count; i++)
    {
      for (int j = i + 1; j < flattened.Count; j++)
      {
        var a = flattened[i];
        var b = flattened[j];

        if (a.Section.Id == b.Section.Id) continue;

        // Interval overlap: a.Start < b.End && b.Start < a.End
        if (a.Sch.DayOfWeek == b.Sch.DayOfWeek &&
            a.Sch.StartTime < b.Sch.EndTime &&
            b.Sch.StartTime < a.Sch.EndTime)
        {
          conflicts.Add(new ConflictDto
          {
            Message =
                  $"Conflict on {a.Sch.DayOfWeek}: " +
                  $"'{a.Section.Course.Code}' ({a.Sch.StartTime:HH\\:mm}-{a.Sch.EndTime:HH\\:mm}) " +
                  $"overlaps with '{b.Section.Course.Code}' ({b.Sch.StartTime:HH\\:mm}-{b.Sch.EndTime:HH\\:mm}).",
            SectionIds = new List<int> { a.Section.Id, b.Section.Id }
          });
        }
      }
    }

    return conflicts;
  }

  // ------------------------------------------------------------------
  // 5. CONFIRM REGISTRATION (transaction + atomic capacity check)
  // ------------------------------------------------------------------
  public async Task<RegistrationSummaryDto> ConfirmRegistrationAsync(
      int studentId, ConfirmRegistrationRequest request)
  {
    if (request.SectionIds is null || request.SectionIds.Count == 0)
      throw new BusinessRuleException("No sections selected.", "NO_SECTIONS");

    var validation = await ValidateSelectionAsync(studentId, request.SectionIds);
    if (!validation.IsValid)
      throw new BusinessRuleException(
          string.Join(" | ", validation.Errors),
          "REGISTRATION_VALIDATION_FAILED");

    var sections = await _db.CourseSections
        .Where(cs => request.SectionIds.Contains(cs.Id))
        .Include(cs => cs.Course)
        .Include(cs => cs.Instructor)
        .ToListAsync();

    await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

    try
    {
      var now = DateTime.UtcNow;
      var enrollments = new List<Enrollment>();

      foreach (var section in sections)
      {
        // Atomic capacity check + increment (prevents over-enrollment under concurrency)
        var affected = await _db.Database.ExecuteSqlRawAsync(
            "UPDATE [CourseSections] SET [EnrolledCount] = [EnrolledCount] + 1 " +
            "WHERE [Id] = {0} AND [EnrolledCount] < [Capacity]",
            section.Id);

        if (affected == 0)
          throw new ConflictException(
              $"Section {section.SectionNumber} of '{section.Course.Code}' is now full.",
              "SECTION_FULL");

        enrollments.Add(new Enrollment
        {
          StudentId = studentId,
          SectionId = section.Id,
          Status = EnrollmentStatus.Registered,
          RegisteredAt = now
        });
      }

      _db.Enrollments.AddRange(enrollments);
      await _db.SaveChangesAsync();

      await tx.CommitAsync();

      return new RegistrationSummaryDto
      {
        EnrollmentsCount = enrollments.Count,
        TotalCreditHours = sections.Sum(s => s.Course.CreditHours),
        RegisteredAt = now,
        Courses = enrollments.Select((e, idx) => new EnrolledCourseDto
        {
          EnrollmentId = e.Id,
          CourseCode = sections[idx].Course.Code,
          CourseName = sections[idx].Course.Name,
          CreditHours = sections[idx].Course.CreditHours,
          SectionNumber = sections[idx].SectionNumber,
          InstructorName = sections[idx].Instructor?.Name ?? string.Empty
        }).ToList()
      };
    }
    catch
    {
      await tx.RollbackAsync();
      throw;
    }
  }

  // ------------------------------------------------------------------
  // 6. DELETE CURRENT REGISTRATION
  // ------------------------------------------------------------------
  public async Task DeleteCurrentRegistrationAsync(int studentId)
  {
    var semester = await _db.Semesters.AsNoTracking()
        .FirstOrDefaultAsync(s => s.IsCurrent)
        ?? throw new NotFoundException("No current semester found.", "NO_CURRENT_SEMESTER");

    var enrollments = await _db.Enrollments
        .Where(e => e.StudentId == studentId
                 && e.Section.SemesterId == semester.Id
                 && e.Status == EnrollmentStatus.Registered)
        .ToListAsync();

    if (enrollments.Count == 0)
      throw new NotFoundException("No active registrations to delete.", "NO_REGISTRATIONS");

    await using var tx = await _db.Database.BeginTransactionAsync();

    try
    {
      foreach (var e in enrollments)
      {
        await _db.Database.ExecuteSqlRawAsync(
            "UPDATE [CourseSections] SET [EnrolledCount] = " +
            "CASE WHEN [EnrolledCount] > 0 THEN [EnrolledCount] - 1 ELSE 0 END " +
            "WHERE [Id] = {0}",
            e.SectionId);
      }

      _db.Enrollments.RemoveRange(enrollments);
      await _db.SaveChangesAsync();

      await tx.CommitAsync();
    }
    catch
    {
      await tx.RollbackAsync();
      throw;
    }
  }
}