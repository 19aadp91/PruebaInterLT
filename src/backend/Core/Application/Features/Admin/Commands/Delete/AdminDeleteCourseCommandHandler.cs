using Application.Common.Dispatcher;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

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
