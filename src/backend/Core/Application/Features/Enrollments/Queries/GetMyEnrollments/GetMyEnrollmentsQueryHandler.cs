using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;

namespace Application.Features.Enrollments.Queries;

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
