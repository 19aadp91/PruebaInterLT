using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Admin.Commands;

public record AdminCreateProfessorCommand(string FullName, string Email) : IRequest<ProfessorDto>;
