using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Catalog.Queries;
using Application.Features.Enrollments.Commands;
using Application.Features.Enrollments.Queries;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/v1/students")]
[Authorize]
public class StudentsController : BaseApiController
{
    private readonly IDispatcher _dispatcher;

    public StudentsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyList<PublicStudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicStudents(CancellationToken ct)
    {
        var result = await _dispatcher.SendAsync(new GetPublicStudentsQuery(), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(Result<StudentProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<StudentProfileDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new GetStudentProfileQuery(studentId), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    [HttpGet("me/enrollments")]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new GetMyEnrollmentsQuery(studentId), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
    }

    public record EnrollRequest(Guid CourseId);

    [HttpPost("me/enrollments")]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest body, CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new EnrollCourseCommand(studentId, body.CourseId), ct);
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpDelete("me/enrollments/{courseId:guid}")]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<StudentEnrollmentSummaryDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Drop(Guid courseId, CancellationToken ct)
    {
        var studentId = GetCurrentUserId();
        var result = await _dispatcher.SendAsync(new DropCourseCommand(studentId, courseId), ct);
        return ToActionResult(result, StatusCodes.Status200OK);
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
