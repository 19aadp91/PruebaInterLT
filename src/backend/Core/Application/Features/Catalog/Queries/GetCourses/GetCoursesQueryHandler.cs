using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Result<IReadOnlyList<CourseDto>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Result<IReadOnlyList<CourseDto>>> HandleAsync(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllWithProfessorAsync(cancellationToken);
        var dtos = (IReadOnlyList<CourseDto>)courses.Select(c => new CourseDto(
            c.Id,
            c.Code,
            c.Name,
            c.Credits,
            new ProfessorDto(
                c.Professor?.Id ?? Guid.Empty,
                c.Professor?.FullName ?? "Docente",
                c.Professor?.Email ?? string.Empty,
                c.Professor?.IsAvailable ?? true
            )
        )).ToList();

        return Result<IReadOnlyList<CourseDto>>.Success(dtos);
    }
}
