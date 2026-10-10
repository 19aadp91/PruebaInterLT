using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Enrollments.Commands;

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
