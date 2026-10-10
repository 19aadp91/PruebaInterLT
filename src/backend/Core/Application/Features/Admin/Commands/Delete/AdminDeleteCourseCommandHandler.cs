using Application.Common.Dispatcher;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminDeleteCourseCommandHandler : IRequestHandler<AdminDeleteCourseCommand, Result>
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

    public async Task<Result> HandleAsync(AdminDeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            return Result.Failure("Materia no encontrada.", ErrorType.NotFound);

        if (await _enrollmentRepository.HasActiveEnrollmentsByCourseIdAsync(request.CourseId, cancellationToken))
            return Result.Failure("No es posible desactivar la materia porque tiene estudiantes inscritos activos.", ErrorType.Conflict);

        course.IsDeleted = true;
        await _courseRepository.UpdateAsync(course, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Materia eliminada exitosamente.");
    }
}
