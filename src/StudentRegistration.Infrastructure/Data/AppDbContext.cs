using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.Interfaces;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<College> Colleges => Set<College>();
  public DbSet<Department> Departments => Set<Department>();
  public DbSet<Student> Students => Set<Student>();
  public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
  public DbSet<StudyPlanCourse> StudyPlanCourses => Set<StudyPlanCourse>();
  public DbSet<Course> Courses => Set<Course>();
  public DbSet<CoursePrerequisite> CoursePrerequisites => Set<CoursePrerequisite>();
  public DbSet<Semester> Semesters => Set<Semester>();
  public DbSet<RegistrationPeriod> RegistrationPeriods => Set<RegistrationPeriod>();
  public DbSet<Instructor> Instructors => Set<Instructor>();
  public DbSet<TeachingAssistant> TeachingAssistants => Set<TeachingAssistant>();
  public DbSet<CourseSection> CourseSections => Set<CourseSection>();
  public DbSet<SectionSchedule> SectionSchedules => Set<SectionSchedule>();
  public DbSet<Enrollment> Enrollments => Set<Enrollment>();
  public DbSet<StudentCompletedCourse> StudentCompletedCourses => Set<StudentCompletedCourse>();
  public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}