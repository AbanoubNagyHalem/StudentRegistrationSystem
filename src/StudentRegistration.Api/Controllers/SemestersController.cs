using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.Application.DTOs;
using StudentRegistration.Infrastructure.Data;

namespace StudentRegistration.Api.Controllers;

[ApiController]
[Route("api/semesters")]
[AllowAnonymous]
public class SemestersController : ControllerBase
{
  private readonly AppDbContext _db;

  public SemestersController(AppDbContext db)
  {
    _db = db;
  }

  [HttpGet("current")]
  [ProducesResponseType(typeof(CurrentSemesterDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetCurrent()
  {
    var s = await _db.Semesters.AsNoTracking()
        .Where(x => x.IsCurrent)
        .Select(x => new CurrentSemesterDto
        {
          Id = x.Id,
          Name = x.Name,
          StartDate = x.StartDate,
          EndDate = x.EndDate
        })
        .FirstOrDefaultAsync();

    if (s is null) return NotFound(new { message = "No current semester." });
    return Ok(s);
  }
}