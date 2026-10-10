using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Enrollments.Queries;

public class GetCourseClassmatesQueryHandler : IRequestHandler<GetCourseClassmatesQuery, IReadOnlyList<ClassmateDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseClassmatesQueryHandler(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IReadOnlyList<ClassmateDto>> HandleAsync(GetCourseClassmatesQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new NotFoundException("La materia especificada no existe.");

        var classmates = await _enrollmentRepository.GetClassmatesByCourseIdAsync(request.CourseId, cancellationToken);

        // PRIVACIDAD POR DISEÑO: Solo proyectar FullName
        return classmates.Select(s => new ClassmateDto(s.Id, s.User?.FullName ?? "Estudiante")).ToList();
    }
}
