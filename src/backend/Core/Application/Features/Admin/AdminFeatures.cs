using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Admin;

public record AdminCreateProfessorCommand(string FullName, string Email) : IRequest<ProfessorDto>;

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

public record AdminDeleteProfessorCommand(Guid ProfessorId) : IRequest<bool>;

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

public record AdminCreateCourseCommand(string Code, string Name, Guid ProfessorId) : IRequest<CourseDto>;

public class AdminCreateCourseCommandHandler : IRequestHandler<AdminCreateCourseCommand, CourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminCreateCourseCommandHandler(
        ICourseRepository courseRepository,
        IProfessorRepository professorRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _professorRepository = professorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseDto> HandleAsync(AdminCreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new DomainValidationException("El código de la materia es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainValidationException("El nombre de la materia es obligatorio.");

        var professor = await _professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken);
        if (professor == null)
            throw new NotFoundException("El profesor asignado no existe.");

        if (await _courseRepository.ExistsByCodeAsync(request.Code.Trim().ToUpperInvariant(), cancellationToken))
            throw new ConflictException("Ya existe una materia con ese código.");

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Credits = 3,
            ProfessorId = request.ProfessorId,
            Professor = professor
        };

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CourseDto(
            course.Id,
            course.Code,
            course.Name,
            course.Credits,
            new ProfessorDto(professor.Id, professor.FullName, professor.Email, professor.IsAvailable)
        );
    }
}

public record AdminDeleteCourseCommand(Guid CourseId) : IRequest<bool>;

public class AdminDeleteCourseCommandHandler : IRequestHandler<AdminDeleteCourseCommand, bool>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminDeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(AdminDeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new NotFoundException("Materia no encontrada.");

        if (await _enrollmentRepository.HasActiveEnrollmentsByCourseIdAsync(request.CourseId, cancellationToken))
            throw new ConflictException("No es posible desactivar la materia porque tiene estudiantes inscritos activos.");

        course.IsDeleted = true;
        await _courseRepository.UpdateAsync(course, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}
