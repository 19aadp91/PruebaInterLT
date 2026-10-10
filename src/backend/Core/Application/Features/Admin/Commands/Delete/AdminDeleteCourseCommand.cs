using Application.Common.Dispatcher;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public record AdminDeleteCourseCommand(Guid CourseId) : IRequest<Result>;
