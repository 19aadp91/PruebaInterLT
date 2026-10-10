using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Features.Enrollments.Commands;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, Result<StudentEnrollmentSummaryDto>>
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

    public async Task<Result<StudentEnrollmentSummaryDto>> HandleAsync(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
            return Result<StudentEnrollmentSummaryDto>.Failure("El estudiante no fue encontrado.", ErrorType.NotFound);

        var courseToEnroll = await _courseRepository.GetByIdWithProfessorAsync(request.CourseId, cancellationToken);
        if (courseToEnroll == null)
            return Result<StudentEnrollmentSummaryDto>.Failure("La materia solicitada no existe o no se encuentra disponible.", ErrorType.NotFound);

        var activeEnrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        var currentCourses = activeEnrollments
            .Where(e => e.Course != null)
            .Select(e => e.Course!)
            .ToList();

        var candidate = new EnrollmentCandidate(currentCourses, courseToEnroll);

        // 1. Evaluar si ya está inscrita
        var notAlreadyEnrolledSpec = new NotAlreadyEnrolledSpecification();
        if (!notAlreadyEnrolledSpec.IsSatisfiedBy(candidate))
            return Result<StudentEnrollmentSummaryDto>.Failure(notAlreadyEnrolledSpec.ErrorMessage, ErrorType.Conflict);

        // 2. Evaluar límite de 3 materias (9 créditos)
        var maxCoursesSpec = new MaxCoursesSpecification();
        if (!maxCoursesSpec.IsSatisfiedBy(candidate))
            return Result<StudentEnrollmentSummaryDto>.Failure(maxCoursesSpec.ErrorMessage, ErrorType.Validation);

        // 3. Evaluar regla innegociable de profesor único
        var uniqueProfessorSpec = new UniqueProfessorSpecification();
        if (!uniqueProfessorSpec.IsSatisfiedBy(candidate))
            return Result<StudentEnrollmentSummaryDto>.Failure(uniqueProfessorSpec.ErrorMessage, ErrorType.Validation);

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

        var updatedEnrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        return Result<StudentEnrollmentSummaryDto>.Success(MapToSummary(updatedEnrollments), "Inscripción realizada exitosamente.");
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
