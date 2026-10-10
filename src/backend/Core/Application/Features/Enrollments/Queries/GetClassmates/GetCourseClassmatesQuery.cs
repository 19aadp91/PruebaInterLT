using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Enrollments.Queries;

public record GetCourseClassmatesQuery(Guid CourseId) : IRequest<Result<IReadOnlyList<ClassmateDto>>>;
