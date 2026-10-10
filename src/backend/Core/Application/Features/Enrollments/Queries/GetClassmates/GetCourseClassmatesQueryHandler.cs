using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Enrollments.Queries;

public class GetCourseClassmatesQueryHandler : IRequestHandler<GetCourseClassmatesQuery, Result<IReadOnlyList<ClassmateDto>>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseClassmatesQueryHandler(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Result<IReadOnlyList<ClassmateDto>>> HandleAsync(GetCourseClassmatesQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            return Result<IReadOnlyList<ClassmateDto>>.Failure("La materia especificada no existe.", ErrorType.NotFound);

        var classmates = await _enrollmentRepository.GetClassmatesByCourseIdAsync(request.CourseId, cancellationToken);

        // PRIVACIDAD POR DISEÑO: Solo proyectar FullName
        var dtos = (IReadOnlyList<ClassmateDto>)classmates
            .Select(s => new ClassmateDto(s.Id, s.User?.FullName ?? "Estudiante"))
            .ToList();

        return Result<IReadOnlyList<ClassmateDto>>.Success(dtos);
    }
}
