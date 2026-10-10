using Application.Common.Dispatcher;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public record AdminDeleteProfessorCommand(Guid ProfessorId) : IRequest<Result>;
