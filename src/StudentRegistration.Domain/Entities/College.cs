using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class College : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;

  // Navigation
  public ICollection<Department> Departments { get; set; } = new List<Department>();
}