using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Admin.Commands;
using Application.Features.Catalog.Queries;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/admin")]
[Authorize]
public class AdminController : BaseApiController
{
    private readonly IDispatcher _dispatcher;

    public AdminController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("professors")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<ProfessorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfessors(CancellationToken ct)
    {
        var coursesResult = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        if (!coursesResult.IsSuccess)
            return ToActionResult(Result<IReadOnlyList<ProfessorDto>>.Failure(coursesResult.Message ?? "Error al obtener materias", coursesResult.ErrorType, coursesResult.Errors));

        var professors = (coursesResult.Data ?? Array.Empty<CourseDto>())
            .Select(c => c.Professor)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();

        return ToActionResult(Result<IReadOnlyList<ProfessorDto>>.Success(professors));
    }

    [HttpPost("professors")]
    [ProducesResponseType(typeof(Result<ProfessorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<ProfessorDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<ProfessorDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProfessor([FromBody] AdminCreateProfessorCommand command, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(command, ct);
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpDelete("professors/{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteProfessor(Guid id, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new AdminDeleteProfessorCommand(id), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    [HttpGet("courses")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    [HttpPost("courses")]
    [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<CourseDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCourse([FromBody] AdminCreateCourseCommand command, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(command, ct);
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpDelete("courses/{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCourse(Guid id, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new AdminDeleteCourseCommand(id), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }
}
