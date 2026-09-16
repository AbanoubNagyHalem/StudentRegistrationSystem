using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Application.Interfaces;

public interface IAppDbContext
{
  DbSet<College> Colleges { get; }
  DbSet<Department> Departments { get; }
  DbSet<Student> Students { get; }
  DbSet<StudyPlan> StudyPlans { get; }
  DbSet<StudyPlanCourse> StudyPlanCourses { get; }
  DbSet<Course> Courses { get; }
  DbSet<CoursePrerequisite> CoursePrerequisites { get; }
  DbSet<Semester> Semesters { get; }
  DbSet<RegistrationPeriod> RegistrationPeriods { get; }
  DbSet<Instructor> Instructors { get; }
  DbSet<TeachingAssistant> TeachingAssistants { get; }
  DbSet<CourseSection> CourseSections { get; }
  DbSet<SectionSchedule> SectionSchedules { get; }
  DbSet<Enrollment> Enrollments { get; }
  DbSet<StudentCompletedCourse> StudentCompletedCourses { get; }
  DbSet<AdminUser> AdminUsers { get; }

  DatabaseFacade Database { get; }
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}