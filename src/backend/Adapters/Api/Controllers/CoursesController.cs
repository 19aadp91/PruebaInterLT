using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Catalog.Queries;
using Application.Features.Enrollments.Queries;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/courses")]
[Authorize]
public class CoursesController : BaseApiController
{
    private readonly IDispatcher _dispatcher;

    public CoursesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyList<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCoursesQuery(), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    [HttpGet("{courseId:guid}/classmates")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<ClassmateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<IReadOnlyList<ClassmateDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClassmates(Guid courseId, CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetCourseClassmatesQuery(courseId), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }
}
