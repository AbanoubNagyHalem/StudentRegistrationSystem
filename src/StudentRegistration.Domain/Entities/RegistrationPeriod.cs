using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class RegistrationPeriod : BaseEntity
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsOpen { get; set; }

  // FKs
  public int DepartmentId { get; set; }
  public int SemesterId { get; set; }

  // Navigation properties
  public Department Department { get; set; } = null!;
  public Semester Semester { get; set; } = null!;

  public bool IsActive(DateTime now) =>
      IsOpen && now >= StartDate && now <= EndDate;
}