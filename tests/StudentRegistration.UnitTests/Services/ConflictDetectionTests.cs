using FluentAssertions;
using StudentRegistration.Domain.Entities;
using StudentRegistration.Domain.Enums;
using Xunit;

namespace StudentRegistration.UnitTests.Services;

public class ConflictDetectionTests
{
  private static bool HasConflict(SectionSchedule a, SectionSchedule b)
      => a.DayOfWeek == b.DayOfWeek
         && a.StartTime < b.EndTime
         && b.StartTime < a.EndTime;

  [Fact]
  public void SameDay_OverlappingTime_ReturnsTrue()
  {
    var a = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) };
    var b = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(13, 0) };

    HasConflict(a, b).Should().BeTrue();
  }

  [Fact]
  public void SameDay_NonOverlapping_ReturnsFalse()
  {
    var a = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(10, 0) };
    var b = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) };

    HasConflict(a, b).Should().BeFalse();
  }

  [Fact]
  public void DifferentDay_SameTime_ReturnsFalse()
  {
    var a = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) };
    var b = new SectionSchedule { DayOfWeek = WeekDay.Monday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) };

    HasConflict(a, b).Should().BeFalse();
  }

  [Fact]
  public void SameDay_OneInsideOther_ReturnsTrue()
  {
    var a = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(14, 0) };
    var b = new SectionSchedule { DayOfWeek = WeekDay.Sunday, StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(12, 0) };

    HasConflict(a, b).Should().BeTrue();
  }
}