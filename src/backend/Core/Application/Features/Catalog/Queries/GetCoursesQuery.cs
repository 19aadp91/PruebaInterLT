using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Catalog.Queries;

public record GetCoursesQuery : IRequest<IReadOnlyList<CourseDto>>;
