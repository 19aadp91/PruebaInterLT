using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public record AdminCreateCourseCommand(string Code, string Name, Guid ProfessorId) : IRequest<Result<CourseDto>>;
