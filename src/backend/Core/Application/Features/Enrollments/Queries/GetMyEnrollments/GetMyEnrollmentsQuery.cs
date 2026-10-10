using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Enrollments.Queries;

public record GetMyEnrollmentsQuery(Guid StudentId) : IRequest<Result<StudentEnrollmentSummaryDto>>;
