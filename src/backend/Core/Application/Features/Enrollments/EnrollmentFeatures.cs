using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specifications;

namespace Application.Features.Enrollments;

public record EnrollCourseCommand(Guid StudentId, Guid CourseId) : IRequest<StudentEnrollmentSummaryDto>;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, StudentEnrollmentSummaryDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollCourseCommandHandler(
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentEnrollmentSummaryDto> HandleAsync(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
            throw new NotFoundException("El estudiante no fue encontrado.");

        var courseToEnroll = await _courseRepository.GetByIdWithProfessorAsync(request.CourseId, cancellationToken);
        if (courseToEnroll == null)
            throw new NotFoundException("La materia solicitada no existe o no se encuentra disponible.");

        var activeEnrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        var currentCourses = activeEnrollments
            .Where(e => e.Course != null)
            .Select(e => e.Course!)
            .ToList();

        var candidate = new EnrollmentCandidate(currentCourses, courseToEnroll);

        // 1. Evaluar si ya está inscrita
        var notAlreadyEnrolledSpec = new NotAlreadyEnrolledSpecification();
        if (!notAlreadyEnrolledSpec.IsSatisfiedBy(candidate))
            throw new ConflictException(notAlreadyEnrolledSpec.ErrorMessage);

        // 2. Evaluar límite de 3 materias (9 créditos)
        var maxCoursesSpec = new MaxCoursesSpecification();
        if (!maxCoursesSpec.IsSatisfiedBy(candidate))
            throw new DomainValidationException(maxCoursesSpec.ErrorMessage);

        // 3. Evaluar regla innegociable de profesor único
        var uniqueProfessorSpec = new UniqueProfessorSpecification();
        if (!uniqueProfessorSpec.IsSatisfiedBy(candidate))
            throw new DomainValidationException(uniqueProfessorSpec.ErrorMessage);

        // Crear inscripción
        var newEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAtUtc = DateTimeOffset.UtcNow
        };

        await _enrollmentRepository.AddAsync(newEnrollment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Obtener lista actualizada
        var updatedEnrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        return MapToSummary(updatedEnrollments);
    }

    private static StudentEnrollmentSummaryDto MapToSummary(IReadOnlyList<Enrollment> enrollments)
    {
        var items = enrollments.Select(e => new EnrolledCourseItemDto(
            e.Id,
            e.CourseId,
            e.Course?.Code ?? string.Empty,
            e.Course?.Name ?? string.Empty,
            e.Course?.Credits ?? 3,
            e.Course?.ProfessorId ?? Guid.Empty,
            e.Course?.Professor?.FullName ?? "Docente Asignado",
            e.EnrolledAtUtc
        )).ToList();

        var count = items.Count;
        var totalCredits = count * 3;

        return new StudentEnrollmentSummaryDto(totalCredits, 9, count, 3, items);
    }
}

public record DropCourseCommand(Guid StudentId, Guid CourseId) : IRequest<StudentEnrollmentSummaryDto>;

public class DropCourseCommandHandler : IRequestHandler<DropCourseCommand, StudentEnrollmentSummaryDto>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DropCourseCommandHandler(IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentEnrollmentSummaryDto> HandleAsync(DropCourseCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetActiveByStudentAndCourseAsync(request.StudentId, request.CourseId, cancellationToken);
        if (enrollment == null)
            throw new NotFoundException("La materia no se encuentra registrada en la carga académica actual.");

        await _enrollmentRepository.RemoveAsync(enrollment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var updatedEnrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        return MapToSummary(updatedEnrollments);
    }

    private static StudentEnrollmentSummaryDto MapToSummary(IReadOnlyList<Enrollment> enrollments)
    {
        var items = enrollments.Select(e => new EnrolledCourseItemDto(
            e.Id,
            e.CourseId,
            e.Course?.Code ?? string.Empty,
            e.Course?.Name ?? string.Empty,
            e.Course?.Credits ?? 3,
            e.Course?.ProfessorId ?? Guid.Empty,
            e.Course?.Professor?.FullName ?? "Docente Asignado",
            e.EnrolledAtUtc
        )).ToList();

        var count = items.Count;
        var totalCredits = count * 3;

        return new StudentEnrollmentSummaryDto(totalCredits, 9, count, 3, items);
    }
}

public record GetMyEnrollmentsQuery(Guid StudentId) : IRequest<StudentEnrollmentSummaryDto>;

public class GetMyEnrollmentsQueryHandler : IRequestHandler<GetMyEnrollmentsQuery, StudentEnrollmentSummaryDto>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetMyEnrollmentsQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<StudentEnrollmentSummaryDto> HandleAsync(GetMyEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        var items = enrollments.Select(e => new EnrolledCourseItemDto(
            e.Id,
            e.CourseId,
            e.Course?.Code ?? string.Empty,
            e.Course?.Name ?? string.Empty,
            e.Course?.Credits ?? 3,
            e.Course?.ProfessorId ?? Guid.Empty,
            e.Course?.Professor?.FullName ?? "Docente Asignado",
            e.EnrolledAtUtc
        )).ToList();

        return new StudentEnrollmentSummaryDto(items.Count * 3, 9, items.Count, 3, items);
    }
}

public record GetCourseClassmatesQuery(Guid CourseId) : IRequest<IReadOnlyList<ClassmateDto>>;

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
