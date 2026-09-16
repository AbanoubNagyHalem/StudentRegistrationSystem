using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.Common.Exceptions;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Application.Services;

public class AdminService : IAdminService
{
  private readonly IAppDbContext _db;

  public AdminService(IAppDbContext db) => _db = db;

  // ============================================================
  // STUDENTS
  // ============================================================
  public async Task<List<AdminStudentDto>> GetStudentsAsync()
  {
    return await _db.Students
        .AsNoTracking()
        .Include(s => s.Department).ThenInclude(d => d.College)
        .Include(s => s.StudyPlan)
        .OrderBy(s => s.RegistrationNumber)
        .Select(s => new AdminStudentDto
        {
          Id = s.Id,
          RegistrationNumber = s.RegistrationNumber,
          FullName = s.FullName,
          CollegeName = s.Department.College.Name,
          DepartmentName = s.Department.Name,
          StudyPlanName = s.StudyPlan != null ? s.StudyPlan.Name : null,
          Status = s.Status.ToString(),
          Level = s.Level.ToString(),
          GPA = s.GPA,
          Sponsor = s.Sponsor
        })
        .ToListAsync();
  }

  public async Task<AdminStudentDto> GetStudentByIdAsync(int id)
  {
    var s = await _db.Students.AsNoTracking()
        .Include(x => x.Department).ThenInclude(d => d.College)
        .Include(x => x.StudyPlan)
        .FirstOrDefaultAsync(x => x.Id == id)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    return new AdminStudentDto
    {
      Id = s.Id,
      RegistrationNumber = s.RegistrationNumber,
      FullName = s.FullName,
      CollegeName = s.Department.College.Name,
      DepartmentName = s.Department.Name,
      StudyPlanName = s.StudyPlan?.Name,
      Status = s.Status.ToString(),
      Level = s.Level.ToString(),
      GPA = s.GPA,
      Sponsor = s.Sponsor
    };
  }

  public async Task<AdminStudentDto> CreateStudentAsync(CreateStudentRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.RegistrationNumber) ||
        string.IsNullOrWhiteSpace(request.FullName) ||
        string.IsNullOrWhiteSpace(request.Pin))
      throw new BusinessRuleException("Registration number, full name, and PIN are required.", "INVALID_INPUT");

    if (await _db.Students.AnyAsync(s => s.RegistrationNumber == request.RegistrationNumber))
      throw new ConflictException(
          $"Registration number '{request.RegistrationNumber}' already exists.",
          "DUPLICATE_REGISTRATION_NUMBER");

    if (!await _db.Departments.AnyAsync(d => d.Id == request.DepartmentId))
      throw new NotFoundException("Department not found.", "DEPARTMENT_NOT_FOUND");

    var student = new Student
    {
      RegistrationNumber = request.RegistrationNumber,
      FullName = request.FullName,
      PinHash = BCrypt.Net.BCrypt.HashPassword(request.Pin),
      DepartmentId = request.DepartmentId,
      StudyPlanId = request.StudyPlanId,
      Status = (StudentStatus)request.Status,
      Level = (StudentLevel)request.Level,
      Sponsor = request.Sponsor
    };

    _db.Students.Add(student);
    await _db.SaveChangesAsync();

    return await GetStudentByIdAsync(student.Id);
  }

  public async Task<AdminStudentDto> UpdateStudentAsync(int id, UpdateStudentRequest request)
  {
    var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    student.FullName = request.FullName;
    student.DepartmentId = request.DepartmentId;
    student.StudyPlanId = request.StudyPlanId;
    student.Status = (StudentStatus)request.Status;
    student.Level = (StudentLevel)request.Level;
    student.Sponsor = request.Sponsor;

    await _db.SaveChangesAsync();
    return await GetStudentByIdAsync(student.Id);
  }

  public async Task ResetStudentPinAsync(int id, ResetPinRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.NewPin) || request.NewPin.Length < 4)
      throw new BusinessRuleException("PIN must be at least 4 characters.", "INVALID_PIN");

    var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    student.PinHash = BCrypt.Net.BCrypt.HashPassword(request.NewPin);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteStudentAsync(int id)
  {
    var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Student not found.", "STUDENT_NOT_FOUND");

    _db.Students.Remove(student);
    await _db.SaveChangesAsync();
  }

  // ============================================================
  // REGISTRATION PERIODS
  // ============================================================
  public async Task<List<AdminRegistrationPeriodDto>> GetRegistrationPeriodsAsync()
  {
    var now = DateTime.UtcNow;
    return await _db.RegistrationPeriods
        .AsNoTracking()
        .Include(rp => rp.Department)
        .Include(rp => rp.Semester)
        .OrderByDescending(rp => rp.StartDate)
        .Select(rp => new AdminRegistrationPeriodDto
        {
          Id = rp.Id,
          DepartmentId = rp.DepartmentId,
          DepartmentName = rp.Department.Name,
          SemesterId = rp.SemesterId,
          SemesterName = rp.Semester.Name,
          StartDate = rp.StartDate,
          EndDate = rp.EndDate,
          IsOpen = rp.IsOpen,
          IsCurrentlyActive = rp.IsOpen && now >= rp.StartDate && now <= rp.EndDate
        })
        .ToListAsync();
  }

  public async Task<AdminRegistrationPeriodDto> CreateRegistrationPeriodAsync(
      CreateRegistrationPeriodRequest request)
  {
    if (request.EndDate <= request.StartDate)
      throw new BusinessRuleException("End date must be after start date.", "INVALID_DATES");

    if (await _db.RegistrationPeriods.AnyAsync(rp =>
        rp.DepartmentId == request.DepartmentId && rp.SemesterId == request.SemesterId))
      throw new ConflictException(
          "A registration period already exists for this department and semester.",
          "DUPLICATE_PERIOD");

    var period = new RegistrationPeriod
    {
      DepartmentId = request.DepartmentId,
      SemesterId = request.SemesterId,
      StartDate = request.StartDate,
      EndDate = request.EndDate,
      IsOpen = request.IsOpen
    };

    _db.RegistrationPeriods.Add(period);
    await _db.SaveChangesAsync();

    return await GetPeriodDtoAsync(period.Id);
  }

  public async Task<AdminRegistrationPeriodDto> UpdateRegistrationPeriodAsync(
      int id, UpdateRegistrationPeriodRequest request)
  {
    if (request.EndDate <= request.StartDate)
      throw new BusinessRuleException("End date must be after start date.", "INVALID_DATES");

    var period = await _db.RegistrationPeriods.FirstOrDefaultAsync(rp => rp.Id == id)
        ?? throw new NotFoundException("Registration period not found.", "PERIOD_NOT_FOUND");

    period.StartDate = request.StartDate;
    period.EndDate = request.EndDate;
    await _db.SaveChangesAsync();

    return await GetPeriodDtoAsync(period.Id);
  }

  public async Task<AdminRegistrationPeriodDto> OpenRegistrationPeriodAsync(int id)
  {
    var period = await _db.RegistrationPeriods.FirstOrDefaultAsync(rp => rp.Id == id)
        ?? throw new NotFoundException("Registration period not found.", "PERIOD_NOT_FOUND");

    period.IsOpen = true;
    await _db.SaveChangesAsync();

    return await GetPeriodDtoAsync(period.Id);
  }

  public async Task<AdminRegistrationPeriodDto> CloseRegistrationPeriodAsync(int id)
  {
    var period = await _db.RegistrationPeriods.FirstOrDefaultAsync(rp => rp.Id == id)
        ?? throw new NotFoundException("Registration period not found.", "PERIOD_NOT_FOUND");

    period.IsOpen = false;
    await _db.SaveChangesAsync();

    return await GetPeriodDtoAsync(period.Id);
  }

  public async Task DeleteRegistrationPeriodAsync(int id)
  {
    var period = await _db.RegistrationPeriods.FirstOrDefaultAsync(rp => rp.Id == id)
        ?? throw new NotFoundException("Registration period not found.", "PERIOD_NOT_FOUND");

    _db.RegistrationPeriods.Remove(period);
    await _db.SaveChangesAsync();
  }

  private async Task<AdminRegistrationPeriodDto> GetPeriodDtoAsync(int id)
  {
    var now = DateTime.UtcNow;
    return await _db.RegistrationPeriods
        .AsNoTracking()
        .Include(rp => rp.Department)
        .Include(rp => rp.Semester)
        .Where(rp => rp.Id == id)
        .Select(rp => new AdminRegistrationPeriodDto
        {
          Id = rp.Id,
          DepartmentId = rp.DepartmentId,
          DepartmentName = rp.Department.Name,
          SemesterId = rp.SemesterId,
          SemesterName = rp.Semester.Name,
          StartDate = rp.StartDate,
          EndDate = rp.EndDate,
          IsOpen = rp.IsOpen,
          IsCurrentlyActive = rp.IsOpen && now >= rp.StartDate && now <= rp.EndDate
        })
        .FirstAsync();
  }

  // ============================================================
  // SEMESTERS
  // ============================================================
  public async Task<List<AdminSemesterDto>> GetSemestersAsync()
  {
    return await _db.Semesters
        .AsNoTracking()
        .OrderByDescending(s => s.StartDate)
        .Select(s => new AdminSemesterDto
        {
          Id = s.Id,
          Name = s.Name,
          StartDate = s.StartDate,
          EndDate = s.EndDate,
          IsCurrent = s.IsCurrent
        })
        .ToListAsync();
  }

  public async Task<AdminSemesterDto> CreateSemesterAsync(CreateSemesterRequest request)
  {
    if (request.EndDate <= request.StartDate)
      throw new BusinessRuleException("End date must be after start date.", "INVALID_DATES");

    if (request.IsCurrent)
    {
      var currents = await _db.Semesters.Where(s => s.IsCurrent).ToListAsync();
      foreach (var s in currents) s.IsCurrent = false;
    }

    var semester = new Semester
    {
      Name = request.Name,
      StartDate = request.StartDate,
      EndDate = request.EndDate,
      IsCurrent = request.IsCurrent
    };

    _db.Semesters.Add(semester);
    await _db.SaveChangesAsync();

    return new AdminSemesterDto
    {
      Id = semester.Id,
      Name = semester.Name,
      StartDate = semester.StartDate,
      EndDate = semester.EndDate,
      IsCurrent = semester.IsCurrent
    };
  }

  public async Task<AdminSemesterDto> SetCurrentSemesterAsync(int id)
  {
    var target = await _db.Semesters.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Semester not found.", "SEMESTER_NOT_FOUND");

    var currents = await _db.Semesters.Where(s => s.IsCurrent).ToListAsync();
    foreach (var s in currents) s.IsCurrent = false;

    target.IsCurrent = true;
    await _db.SaveChangesAsync();

    return new AdminSemesterDto
    {
      Id = target.Id,
      Name = target.Name,
      StartDate = target.StartDate,
      EndDate = target.EndDate,
      IsCurrent = true
    };
  }

  // ============================================================
  // COURSES
  // ============================================================
  public async Task<List<AdminCourseDto>> GetCoursesAsync()
  {
    return await _db.Courses
        .AsNoTracking()
        .Include(c => c.Department)
        .OrderBy(c => c.Code)
        .Select(c => new AdminCourseDto
        {
          Id = c.Id,
          Code = c.Code,
          Name = c.Name,
          CreditHours = c.CreditHours,
          DepartmentId = c.DepartmentId,
          DepartmentName = c.Department.Name
        })
        .ToListAsync();
  }

  public async Task<AdminCourseDto> GetCourseByIdAsync(int id)
  {
    var c = await _db.Courses.AsNoTracking()
        .Include(x => x.Department)
        .FirstOrDefaultAsync(x => x.Id == id)
        ?? throw new NotFoundException("Course not found.", "COURSE_NOT_FOUND");

    return new AdminCourseDto
    {
      Id = c.Id,
      Code = c.Code,
      Name = c.Name,
      CreditHours = c.CreditHours,
      DepartmentId = c.DepartmentId,
      DepartmentName = c.Department.Name
    };
  }

  public async Task<AdminCourseDto> CreateCourseAsync(CreateCourseRequest request)
  {
    if (request.CreditHours <= 0)
      throw new BusinessRuleException("Credit hours must be positive.", "INVALID_CREDIT_HOURS");

    if (await _db.Courses.AnyAsync(c => c.Code == request.Code))
      throw new ConflictException($"Course code '{request.Code}' already exists.", "DUPLICATE_CODE");

    var course = new Course
    {
      Code = request.Code,
      Name = request.Name,
      CreditHours = request.CreditHours,
      DepartmentId = request.DepartmentId
    };

    _db.Courses.Add(course);
    await _db.SaveChangesAsync();

    return await GetCourseByIdAsync(course.Id);
  }

  public async Task<AdminCourseDto> UpdateCourseAsync(int id, UpdateCourseRequest request)
  {
    var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new NotFoundException("Course not found.", "COURSE_NOT_FOUND");

    if (request.CreditHours <= 0)
      throw new BusinessRuleException("Credit hours must be positive.", "INVALID_CREDIT_HOURS");

    course.Name = request.Name;
    course.CreditHours = request.CreditHours;
    course.DepartmentId = request.DepartmentId;
    await _db.SaveChangesAsync();

    return await GetCourseByIdAsync(course.Id);
  }

  public async Task DeleteCourseAsync(int id)
  {
    var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new NotFoundException("Course not found.", "COURSE_NOT_FOUND");

    if (await _db.CourseSections.AnyAsync(s => s.CourseId == id))
      throw new ConflictException("Cannot delete course with existing sections.", "COURSE_HAS_SECTIONS");

    _db.Courses.Remove(course);
    await _db.SaveChangesAsync();
  }

  // ============================================================
  // SECTIONS
  // ============================================================
  public async Task<List<AdminSectionDto>> GetSectionsAsync(int? semesterId)
  {
    var query = _db.CourseSections
        .AsNoTracking()
        .Include(s => s.Course)
        .Include(s => s.Semester)
        .Include(s => s.Instructor)
        .Include(s => s.TeachingAssistant)
        .Include(s => s.Schedules)
        .AsQueryable();

    if (semesterId.HasValue)
      query = query.Where(s => s.SemesterId == semesterId.Value);

    return await query
        .OrderBy(s => s.Course.Code)
        .ThenBy(s => s.SectionNumber)
        .Select(s => new AdminSectionDto
        {
          Id = s.Id,
          CourseId = s.CourseId,
          CourseCode = s.Course.Code,
          CourseName = s.Course.Name,
          SemesterId = s.SemesterId,
          SemesterName = s.Semester.Name,
          SectionNumber = s.SectionNumber,
          Capacity = s.Capacity,
          EnrolledCount = s.EnrolledCount,
          InstructorId = s.InstructorId,
          InstructorName = s.Instructor.Name,
          TAId = s.TAId,
          TAName = s.TeachingAssistant != null ? s.TeachingAssistant.Name : null,
          Schedules = s.Schedules.Select(sch => new SectionScheduleDto
          {
            DayOfWeek = sch.DayOfWeek.ToString(),
            StartTime = sch.StartTime.ToString("HH:mm"),
            EndTime = sch.EndTime.ToString("HH:mm"),
            Room = sch.Room
          }).ToList()
        })
        .ToListAsync();
  }

  public async Task<AdminSectionDto> GetSectionByIdAsync(int id)
  {
    var list = await GetSectionsAsync(null);
    return list.FirstOrDefault(s => s.Id == id)
        ?? throw new NotFoundException("Section not found.", "SECTION_NOT_FOUND");
  }

  public async Task<AdminSectionDto> CreateSectionAsync(CreateSectionRequest request)
  {
    if (request.Capacity <= 0)
      throw new BusinessRuleException("Capacity must be positive.", "INVALID_CAPACITY");

    if (await _db.CourseSections.AnyAsync(s =>
        s.CourseId == request.CourseId &&
        s.SemesterId == request.SemesterId &&
        s.SectionNumber == request.SectionNumber))
      throw new ConflictException("Section already exists for this course/semester.",
          "DUPLICATE_SECTION");

    var section = new CourseSection
    {
      CourseId = request.CourseId,
      SemesterId = request.SemesterId,
      SectionNumber = request.SectionNumber,
      Capacity = request.Capacity,
      EnrolledCount = 0,
      InstructorId = request.InstructorId,
      TAId = request.TAId
    };

    foreach (var s in request.Schedules)
    {
      if (!TimeOnly.TryParse(s.StartTime, out var start))
        throw new BusinessRuleException($"Invalid start time '{s.StartTime}'.", "INVALID_TIME");
      if (!TimeOnly.TryParse(s.EndTime, out var end))
        throw new BusinessRuleException($"Invalid end time '{s.EndTime}'.", "INVALID_TIME");
      if (start >= end)
        throw new BusinessRuleException("Start time must be before end time.", "INVALID_TIME_RANGE");

      section.Schedules.Add(new SectionSchedule
      {
        DayOfWeek = (WeekDay)s.DayOfWeek,
        StartTime = start,
        EndTime = end,
        Room = s.Room
      });
    }

    _db.CourseSections.Add(section);
    await _db.SaveChangesAsync();

    return await GetSectionByIdAsync(section.Id);
  }

  public async Task<AdminSectionDto> UpdateSectionAsync(int id, UpdateSectionRequest request)
  {
    var section = await _db.CourseSections.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Section not found.", "SECTION_NOT_FOUND");

    if (request.Capacity < section.EnrolledCount)
      throw new BusinessRuleException(
          $"Cannot reduce capacity below current enrolled count ({section.EnrolledCount}).",
          "INVALID_CAPACITY");

    section.Capacity = request.Capacity;
    section.InstructorId = request.InstructorId;
    section.TAId = request.TAId;
    await _db.SaveChangesAsync();

    return await GetSectionByIdAsync(section.Id);
  }

  public async Task DeleteSectionAsync(int id)
  {
    var section = await _db.CourseSections.FirstOrDefaultAsync(s => s.Id == id)
        ?? throw new NotFoundException("Section not found.", "SECTION_NOT_FOUND");

    if (section.EnrolledCount > 0)
      throw new ConflictException("Cannot delete a section with active enrollments.",
          "SECTION_HAS_ENROLLMENTS");

    _db.CourseSections.Remove(section);
    await _db.SaveChangesAsync();
  }

  // ============================================================
  // LOOKUPS
  // ============================================================
  public async Task<List<AdminDepartmentDto>> GetDepartmentsAsync()
  {
    return await _db.Departments.AsNoTracking()
        .Include(d => d.College)
        .OrderBy(d => d.Code)
        .Select(d => new AdminDepartmentDto
        {
          Id = d.Id,
          Name = d.Name,
          Code = d.Code,
          CollegeId = d.CollegeId,
          CollegeName = d.College.Name
        })
        .ToListAsync();
  }

  public async Task<AdminDepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request)
  {
    if (await _db.Departments.AnyAsync(d => d.Code == request.Code))
      throw new ConflictException($"Department code '{request.Code}' exists.", "DUPLICATE_CODE");

    if (!await _db.Colleges.AnyAsync(c => c.Id == request.CollegeId))
      throw new NotFoundException("College not found.", "COLLEGE_NOT_FOUND");

    var dept = new Department
    {
      Name = request.Name,
      Code = request.Code,
      CollegeId = request.CollegeId
    };
    _db.Departments.Add(dept);
    await _db.SaveChangesAsync();

    var college = await _db.Colleges.FindAsync(dept.CollegeId);
    return new AdminDepartmentDto
    {
      Id = dept.Id,
      Name = dept.Name,
      Code = dept.Code,
      CollegeId = dept.CollegeId,
      CollegeName = college?.Name ?? ""
    };
  }

  public async Task<List<AdminInstructorDto>> GetInstructorsAsync()
  {
    return await _db.Instructors.AsNoTracking()
        .OrderBy(i => i.Name)
        .Select(i => new AdminInstructorDto { Id = i.Id, Name = i.Name, Email = i.Email })
        .ToListAsync();
  }

  public async Task<AdminInstructorDto> CreateInstructorAsync(CreateInstructorRequest request)
  {
    var inst = new Instructor { Name = request.Name, Email = request.Email };
    _db.Instructors.Add(inst);
    await _db.SaveChangesAsync();

    return new AdminInstructorDto { Id = inst.Id, Name = inst.Name, Email = inst.Email };
  }

  public async Task<List<AdminTADto>> GetTeachingAssistantsAsync()
  {
    return await _db.TeachingAssistants.AsNoTracking()
        .OrderBy(t => t.Name)
        .Select(t => new AdminTADto { Id = t.Id, Name = t.Name, Email = t.Email })
        .ToListAsync();
  }

  public async Task<AdminTADto> CreateTeachingAssistantAsync(CreateTARequest request)
  {
    var ta = new TeachingAssistant { Name = request.Name, Email = request.Email };
    _db.TeachingAssistants.Add(ta);
    await _db.SaveChangesAsync();

    return new AdminTADto { Id = ta.Id, Name = ta.Name, Email = ta.Email };
  }
}