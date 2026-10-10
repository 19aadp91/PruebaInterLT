using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, Result<StudentProfileDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetStudentProfileQueryHandler(IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Result<StudentProfileDto>> HandleAsync(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdWithUserAsync(request.StudentId, cancellationToken);
        if (student == null)
            return Result<StudentProfileDto>.Failure("Estudiante no encontrado.", ErrorType.NotFound);

        var enrollments = await _enrollmentRepository.GetActiveByStudentIdAsync(request.StudentId, cancellationToken);
        var count = enrollments.Count;

        var dto = new StudentProfileDto(
            student.Id,
            student.User?.FullName ?? string.Empty,
            student.IdentificationNumber,
            student.User?.Email ?? string.Empty,
            count * 3,
            count
        );

        return Result<StudentProfileDto>.Success(dto);
    }
}
