using Application.Common.Dispatcher;

namespace Application.Features.Admin.Commands;

public record AdminDeleteCourseCommand(Guid CourseId) : IRequest<bool>;
