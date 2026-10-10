using Application.Common.Dispatcher;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminDeleteProfessorCommandHandler : IRequestHandler<AdminDeleteProfessorCommand, Result>
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

    public async Task<Result> HandleAsync(AdminDeleteProfessorCommand request, CancellationToken cancellationToken)
    {
        var professor = await _professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken);
        if (professor == null)
            return Result.Failure("Docente no encontrado.", ErrorType.NotFound);

        if (await _enrollmentRepository.HasActiveEnrollmentsByProfessorIdAsync(request.ProfessorId, cancellationToken))
            return Result.Failure("No es posible desactivar al profesor porque tiene materias con estudiantes inscritos activos.", ErrorType.Conflict);

        professor.IsDeleted = true;
        professor.IsAvailable = false;
        await _professorRepository.UpdateAsync(professor, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Docente desactivado exitosamente.");
    }
}
