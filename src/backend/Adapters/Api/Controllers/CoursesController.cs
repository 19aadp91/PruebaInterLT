using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Catalog.Queries;
using Application.Features.Enrollments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public CoursesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{courseId:guid}/classmates")]
    [ProducesResponseType(typeof(IReadOnlyList<ClassmateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClassmates(Guid courseId, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCourseClassmatesQuery(courseId), ct);
        return Ok(result);
    }
}
