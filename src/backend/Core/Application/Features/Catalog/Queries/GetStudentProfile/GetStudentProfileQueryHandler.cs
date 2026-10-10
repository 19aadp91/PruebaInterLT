using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Catalog.Queries;

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
