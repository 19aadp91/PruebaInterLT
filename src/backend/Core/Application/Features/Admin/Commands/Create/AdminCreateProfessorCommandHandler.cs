using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminCreateProfessorCommandHandler : IRequestHandler<AdminCreateProfessorCommand, ProfessorDto>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminCreateProfessorCommandHandler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfessorDto> HandleAsync(AdminCreateProfessorCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new DomainValidationException("El nombre del profesor es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            throw new DomainValidationException("El correo del profesor es inválido.");

        if (await _professorRepository.ExistsByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken))
            throw new ConflictException("Ya existe un docente registrado con ese correo.");

        var professor = new Professor
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            IsAvailable = true
        };

        await _professorRepository.AddAsync(professor, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ProfessorDto(professor.Id, professor.FullName, professor.Email, professor.IsAvailable);
    }
}
