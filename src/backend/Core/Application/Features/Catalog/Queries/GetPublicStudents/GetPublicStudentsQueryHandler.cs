using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetPublicStudentsQueryHandler : IRequestHandler<GetPublicStudentsQuery, Result<IReadOnlyList<PublicStudentDto>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetPublicStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<PublicStudentDto>>> HandleAsync(GetPublicStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepository.GetAllPublicAsync(cancellationToken);
        // PRIVACIDAD: Proyectar únicamente ID y Nombre Completo
        var dtos = (IReadOnlyList<PublicStudentDto>)students
            .Select(s => new PublicStudentDto(s.Id, s.User?.FullName ?? "Estudiante"))
            .ToList();

        return Result<IReadOnlyList<PublicStudentDto>>.Success(dtos);
    }
}
