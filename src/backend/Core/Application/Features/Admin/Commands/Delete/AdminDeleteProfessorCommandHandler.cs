using Application.Common.Dispatcher;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminDeleteProfessorCommandHandler : IRequestHandler<AdminDeleteProfessorCommand, bool>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminDeleteProfessorCommandHandler(
        IProfessorRepository professorRepository,
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(AdminDeleteProfessorCommand request, CancellationToken cancellationToken)
    {
        var professor = await _professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken);
        if (professor == null)
            throw new NotFoundException("Docente no encontrado.");

        if (await _enrollmentRepository.HasActiveEnrollmentsByProfessorIdAsync(request.ProfessorId, cancellationToken))
            throw new ConflictException("No es posible desactivar al profesor porque tiene materias con estudiantes inscritos activos.");

        professor.IsDeleted = true;
        professor.IsAvailable = false;
        await _professorRepository.UpdateAsync(professor, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}
