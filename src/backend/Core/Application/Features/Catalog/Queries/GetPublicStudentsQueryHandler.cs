using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;

namespace Application.Features.Catalog.Queries;

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
