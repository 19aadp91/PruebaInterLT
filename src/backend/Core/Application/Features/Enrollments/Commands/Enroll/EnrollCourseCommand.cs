using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Enrollments.Commands;

public record EnrollCourseCommand(Guid StudentId, Guid CourseId) : IRequest<StudentEnrollmentSummaryDto>;
