using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;
using Domain.Entities;

namespace Application.Features.Admin.Commands;

public class AdminCreateCourseCommandHandler : IRequestHandler<AdminCreateCourseCommand, Result<CourseDto>>
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

    public async Task<Result<CourseDto>> HandleAsync(AdminCreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<CourseDto>.Failure("El código de la materia es obligatorio.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<CourseDto>.Failure("El nombre de la materia es obligatorio.", ErrorType.Validation);

        var professor = await _professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken);
        if (professor == null)
            return Result<CourseDto>.Failure("El profesor asignado no existe.", ErrorType.NotFound);

        if (await _courseRepository.ExistsByCodeAsync(request.Code.Trim().ToUpperInvariant(), cancellationToken))
            return Result<CourseDto>.Failure("Ya existe una materia con ese código.", ErrorType.Conflict);

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

        var dto = new CourseDto(
            course.Id,
            course.Code,
            course.Name,
            course.Credits,
            new ProfessorDto(professor.Id, professor.FullName, professor.Email, professor.IsAvailable)
        );

        return Result<CourseDto>.Success(dto, "Materia creada exitosamente.");
    }
}
