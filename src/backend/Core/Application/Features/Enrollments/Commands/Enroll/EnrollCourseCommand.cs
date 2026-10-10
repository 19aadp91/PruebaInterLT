using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Enrollments.Commands;

public record EnrollCourseCommand(Guid StudentId, Guid CourseId) : IRequest<Result<StudentEnrollmentSummaryDto>>;
