using StudentRegistration.Application.DTOs;

namespace StudentRegistration.Application.Interfaces;

public interface IScheduleService
{
  Task<ScheduleDto> GetCurrentSemesterScheduleAsync(int studentId);
}