using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Admin.Commands;
using Application.Features.Catalog.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AdminController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("professors")]
    [ProducesResponseType(typeof(IReadOnlyList<ProfessorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfessors(CancellationToken ct)
    {
        var courses = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        var professors = courses
            .Select(c => c.Professor)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();

        return Ok(professors);
    }

    [HttpPost("professors")]
    [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProfessor([FromBody] AdminCreateProfessorCommand command, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("professors/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProfessor(Guid id, CancellationToken ct)
    {
        await _dispatcher.SendAsync(new AdminDeleteProfessorCommand(id), ct);
        return NoContent();
    }

    [HttpGet("courses")]
    [ProducesResponseType(typeof(IReadOnlyList<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        return Ok(result);
    }

    [HttpPost("courses")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourse([FromBody] AdminCreateCourseCommand command, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("courses/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken ct)
    {
        await _dispatcher.SendAsync(new AdminDeleteCourseCommand(id), ct);
        return NoContent();
    }
}
