using FluentAssertions;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;
using StudentRegistration.UnitTests.Helpers;
using Xunit;

namespace StudentRegistration.UnitTests.Services;

public class RegistrationServiceTests
{
  private static async Task<(int studentId, int semesterId, int deptId)> SeedBasics(
      Infrastructure.Data.AppDbContext db)
  {
    var dept = new Department { Name = "CS", Code = "CS" };
    db.Departments.Add(dept);
    await db.SaveChangesAsync();

    var sem = new Semester
    {
      Name = "Test Semester",
      StartDate = DateTime.UtcNow.AddDays(-1),
      EndDate = DateTime.UtcNow.AddDays(30),
      IsCurrent = true
    };
    db.Semesters.Add(sem);
    await db.SaveChangesAsync();

    var period = new RegistrationPeriod
    {
      DepartmentId = dept.Id,
      SemesterId = sem.Id,
      StartDate = DateTime.UtcNow.AddDays(-1),
      EndDate = DateTime.UtcNow.AddDays(30),
      IsOpen = true
    };
    db.RegistrationPeriods.Add(period);
    await db.SaveChangesAsync();

    var plan = new StudyPlan { Name = "Test Plan", DepartmentId = dept.Id, Year = 2024 };
    db.StudyPlans.Add(plan);
    await db.SaveChangesAsync();

    var student = new Student
    {
      RegistrationNumber = "20240001",
      FullName = "Test Student",
      PinHash = "hash",
      DepartmentId = dept.Id,
      StudyPlanId = plan.Id,
      Status = StudentStatus.Active
    };
    db.Students.Add(student);
    await db.SaveChangesAsync();

    return (student.Id, sem.Id, dept.Id);
  }

  // ----------------------------------------------------------------
  // Eligibility
  // ----------------------------------------------------------------
  [Fact]
  public async Task Eligibility_ActiveStudent_OpenPeriod_ReturnsEligible()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Eligibility_ActiveStudent_OpenPeriod_ReturnsEligible));
    var (sid, _, _) = await SeedBasics(db);

    var service = new RegistrationService(db);
    var result = await service.CheckEligibilityAsync(sid);

    result.IsEligible.Should().BeTrue();
  }

  [Fact]
  public async Task Eligibility_GraduatedStudent_ReturnsNotEligible()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Eligibility_GraduatedStudent_ReturnsNotEligible));
    var (sid, _, _) = await SeedBasics(db);

    var s = await db.Students.FindAsync(sid);
    s!.Status = StudentStatus.Graduated;
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.CheckEligibilityAsync(sid);

    result.IsEligible.Should().BeFalse();
    result.ErrorCode.Should().Be("STUDENT_NOT_ACTIVE");
  }

  [Fact]
  public async Task Eligibility_ClosedPeriod_ReturnsNotEligible()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Eligibility_ClosedPeriod_ReturnsNotEligible));
    var (sid, _, _) = await SeedBasics(db);

    var p = db.RegistrationPeriods.First();
    p.IsOpen = false;
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.CheckEligibilityAsync(sid);

    result.IsEligible.Should().BeFalse();
    result.ErrorCode.Should().Be("REGISTRATION_CLOSED");
  }

  // ----------------------------------------------------------------
  // Schedule Conflict
  // ----------------------------------------------------------------
  [Fact]
  public async Task Validate_ConflictingSections_ReturnsConflictError()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Validate_ConflictingSections_ReturnsConflictError));
    var (sid, semId, deptId) = await SeedBasics(db);

    var c1 = new Course { Code = "C1", Name = "Course 1", CreditHours = 3, DepartmentId = deptId };
    var c2 = new Course { Code = "C2", Name = "Course 2", CreditHours = 3, DepartmentId = deptId };
    db.Courses.AddRange(c1, c2);
    await db.SaveChangesAsync();

    // Both courses are in the plan
    var planId = (await db.Students.FindAsync(sid))!.StudyPlanId!.Value;
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c1.Id, SuggestedTerm = 1 });
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c2.Id, SuggestedTerm = 1 });
    await db.SaveChangesAsync();

    var inst = new Instructor { Name = "Inst" };
    db.Instructors.Add(inst);
    await db.SaveChangesAsync();

    // Both sections at SAME TIME (Sunday 10-12) -> conflict
    var sec1 = new CourseSection
    {
      CourseId = c1.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 0,
      InstructorId = inst.Id,
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) } }
    };
    var sec2 = new CourseSection
    {
      CourseId = c2.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 0,
      InstructorId = inst.Id,
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(13, 0) } }
    };
    db.CourseSections.AddRange(sec1, sec2);
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.ValidateSelectionAsync(sid, new() { sec1.Id, sec2.Id });

    result.IsValid.Should().BeFalse();
    result.Conflicts.Should().NotBeEmpty();
  }

  [Fact]
  public async Task Validate_NonConflictingSections_ReturnsValid()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Validate_NonConflictingSections_ReturnsValid));
    var (sid, semId, deptId) = await SeedBasics(db);

    var c1 = new Course { Code = "C1", Name = "Course 1", CreditHours = 3, DepartmentId = deptId };
    var c2 = new Course { Code = "C2", Name = "Course 2", CreditHours = 3, DepartmentId = deptId };
    db.Courses.AddRange(c1, c2);
    await db.SaveChangesAsync();

    var planId = (await db.Students.FindAsync(sid))!.StudyPlanId!.Value;
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c1.Id, SuggestedTerm = 1 });
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c2.Id, SuggestedTerm = 1 });
    await db.SaveChangesAsync();

    var inst = new Instructor { Name = "Inst" };
    db.Instructors.Add(inst);
    await db.SaveChangesAsync();

    var sec1 = new CourseSection
    {
      CourseId = c1.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 0,
      InstructorId = inst.Id,
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) } }
    };
    var sec2 = new CourseSection
    {
      CourseId = c2.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 0,
      InstructorId = inst.Id,
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) } }
    };
    db.CourseSections.AddRange(sec1, sec2);
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.ValidateSelectionAsync(sid, new() { sec1.Id, sec2.Id });

    result.IsValid.Should().BeTrue();
  }

  // ----------------------------------------------------------------
  // Credit Hours
  // ----------------------------------------------------------------
  [Fact]
  public async Task Validate_ExceedingCreditLimit_ReturnsError()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Validate_ExceedingCreditLimit_ReturnsError));
    var (sid, semId, deptId) = await SeedBasics(db);

    var planId = (await db.Students.FindAsync(sid))!.StudyPlanId!.Value;
    var inst = new Instructor { Name = "Inst" };
    db.Instructors.Add(inst);
    await db.SaveChangesAsync();

    var sectionIds = new List<int>();
    for (int i = 0; i < 7; i++)
    {
      var c = new Course { Code = $"C{i}", Name = $"Course {i}", CreditHours = 3, DepartmentId = deptId };
      db.Courses.Add(c);
      await db.SaveChangesAsync();
      db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c.Id, SuggestedTerm = 1 });

      var sec = new CourseSection
      {
        CourseId = c.Id,
        SemesterId = semId,
        SectionNumber = "1",
        Capacity = 30,
        EnrolledCount = 0,
        InstructorId = inst.Id,
        Schedules = { new SectionSchedule { DayOfWeek = (WeekDay)i, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) } }
      };
      db.CourseSections.Add(sec);
      await db.SaveChangesAsync();
      sectionIds.Add(sec.Id);
    }

    // 7 courses * 3 hours = 21 hours > 18 -> should fail
    var service = new RegistrationService(db);
    var result = await service.ValidateSelectionAsync(sid, sectionIds);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.Contains("18") || e.Contains("credit hours", StringComparison.OrdinalIgnoreCase));
  }

  // ----------------------------------------------------------------
  // Capacity
  // ----------------------------------------------------------------
  [Fact]
  public async Task Validate_FullSection_ReturnsError()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Validate_FullSection_ReturnsError));
    var (sid, semId, deptId) = await SeedBasics(db);

    var c = new Course { Code = "C1", Name = "Course 1", CreditHours = 3, DepartmentId = deptId };
    db.Courses.Add(c);
    await db.SaveChangesAsync();

    var planId = (await db.Students.FindAsync(sid))!.StudyPlanId!.Value;
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = c.Id, SuggestedTerm = 1 });

    var inst = new Instructor { Name = "Inst" };
    db.Instructors.Add(inst);
    await db.SaveChangesAsync();

    var sec = new CourseSection
    {
      CourseId = c.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 30,
      InstructorId = inst.Id,  // FULL
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) } }
    };
    db.CourseSections.Add(sec);
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.ValidateSelectionAsync(sid, new() { sec.Id });

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.Contains("FULL", StringComparison.OrdinalIgnoreCase));
  }

  // ----------------------------------------------------------------
  // Prerequisites
  // ----------------------------------------------------------------
  [Fact]
  public async Task Validate_MissingPrerequisite_ReturnsError()
  {
    using var db = TestDbContextFactory.CreateInMemory(nameof(Validate_MissingPrerequisite_ReturnsError));
    var (sid, semId, deptId) = await SeedBasics(db);

    var prereq = new Course { Code = "PRE", Name = "Prereq", CreditHours = 3, DepartmentId = deptId };
    var target = new Course { Code = "TGT", Name = "Target", CreditHours = 3, DepartmentId = deptId };
    db.Courses.AddRange(prereq, target);
    await db.SaveChangesAsync();

    db.CoursePrerequisites.Add(new CoursePrerequisite
    {
      CourseId = target.Id,
      PrerequisiteCourseId = prereq.Id
    });

    var planId = (await db.Students.FindAsync(sid))!.StudyPlanId!.Value;
    db.StudyPlanCourses.Add(new StudyPlanCourse { StudyPlanId = planId, CourseId = target.Id, SuggestedTerm = 1 });

    var inst = new Instructor { Name = "Inst" };
    db.Instructors.Add(inst);
    await db.SaveChangesAsync();

    var sec = new CourseSection
    {
      CourseId = target.Id,
      SemesterId = semId,
      SectionNumber = "1",
      Capacity = 30,
      EnrolledCount = 0,
      InstructorId = inst.Id,
      Schedules = { new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) } }
    };
    db.CourseSections.Add(sec);
    await db.SaveChangesAsync();

    var service = new RegistrationService(db);
    var result = await service.ValidateSelectionAsync(sid, new() { sec.Id });

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.Contains("prerequisite", StringComparison.OrdinalIgnoreCase));
  }


}