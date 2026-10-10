using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Enrollments.Queries;

public record GetCourseClassmatesQuery(Guid CourseId) : IRequest<IReadOnlyList<ClassmateDto>>;
