using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Catalog;
using Application.Features.Enrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public StudentsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PublicStudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicStudents(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetPublicStudentsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new GetStudentProfileQuery(studentId), ct);
        return Ok(result);
    }

    [HttpGet("me/enrollments")]
    [ProducesResponseType(typeof(StudentEnrollmentSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new GetMyEnrollmentsQuery(studentId), ct);
        return Ok(result);
    }

    public record EnrollRequest(Guid CourseId);

    [HttpPost("me/enrollments")]
    [ProducesResponseType(typeof(StudentEnrollmentSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest body, CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new EnrollCourseCommand(studentId, body.CourseId), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("me/enrollments/{courseId:guid}")]
    [ProducesResponseType(typeof(StudentEnrollmentSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Drop(Guid courseId, CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new DropCourseCommand(studentId, courseId), ct);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(idClaim) || !Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Identificador de usuario ausente en el token.");

        return userId;
    }
}
