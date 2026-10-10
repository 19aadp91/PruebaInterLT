using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Admin.Commands;

public record AdminCreateCourseCommand(string Code, string Name, Guid ProfessorId) : IRequest<CourseDto>;
