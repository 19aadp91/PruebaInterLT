using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public record AdminCreateProfessorCommand(string FullName, string Email) : IRequest<Result<ProfessorDto>>;
