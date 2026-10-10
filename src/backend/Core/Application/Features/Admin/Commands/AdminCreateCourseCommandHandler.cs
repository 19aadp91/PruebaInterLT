using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

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
