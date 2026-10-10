using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Catalog;

public record GetCoursesQuery : IRequest<IReadOnlyList<CourseDto>>;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, IReadOnlyList<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IReadOnlyList<CourseDto>> HandleAsync(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllWithProfessorAsync(cancellationToken);
        return courses.Select(c => new CourseDto(
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
    }
}

public record GetPublicStudentsQuery : IRequest<IReadOnlyList<PublicStudentDto>>;

public class GetPublicStudentsQueryHandler : IRequestHandler<GetPublicStudentsQuery, IReadOnlyList<PublicStudentDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetPublicStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IReadOnlyList<PublicStudentDto>> HandleAsync(GetPublicStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllPublicAsync(cancellationToken);
        // PRIVACIDAD: Proyectar únicamente ID y Nombre Completo
        return students.Select(s => new PublicStudentDto(s.Id, s.User?.FullName ?? "Estudiante")).ToList();
    }
}

public record GetStudentProfileQuery(Guid StudentId) : IRequest<StudentProfileDto>;

public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, StudentProfileDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetStudentProfileQueryHandler(IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<StudentProfileDto> HandleAsync(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdWithUserAsync(request.StudentId, cancellationToken);
        if (student == null)
            throw new NotFoundException("Estudiante no encontrado.");

        var enrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        var count = enrollments.Count;

        return new StudentProfileDto(
            student.Id,
            student.User?.FullName ?? string.Empty,
            student.IdentificationNumber,
            student.User?.Email ?? string.Empty,
            count * 3,
            count
        );
    }
}
