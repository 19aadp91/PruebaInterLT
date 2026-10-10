using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;
using Domain.Entities;

namespace Application.Features.Admin.Commands;

public class AdminCreateProfessorCommandHandler : IRequestHandler<AdminCreateProfessorCommand, Result<ProfessorDto>>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminCreateProfessorCommandHandler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProfessorDto>> HandleAsync(AdminCreateProfessorCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return Result<ProfessorDto>.Failure("El nombre del profesor es obligatorio.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            return Result<ProfessorDto>.Failure("El correo del profesor es inválido.", ErrorType.Validation);

        if (await _professorRepository.ExistsByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken))
            return Result<ProfessorDto>.Failure("Ya existe un docente registrado con ese correo.", ErrorType.Conflict);

        var professor = new Professor
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            IsAvailable = true
        };

        await _professorRepository.AddAsync(professor, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var dto = new ProfessorDto(professor.Id, professor.FullName, professor.Email, professor.IsAvailable);
        return Result<ProfessorDto>.Success(dto, "Docente creado exitosamente.");
    }
}
