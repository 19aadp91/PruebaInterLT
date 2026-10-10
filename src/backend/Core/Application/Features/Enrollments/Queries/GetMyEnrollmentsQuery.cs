using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Enrollments.Queries;

public record GetMyEnrollmentsQuery(Guid StudentId) : IRequest<StudentEnrollmentSummaryDto>;
