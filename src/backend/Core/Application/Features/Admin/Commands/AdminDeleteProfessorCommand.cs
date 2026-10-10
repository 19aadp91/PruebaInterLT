using Application.Common.Dispatcher;

namespace Application.Features.Admin.Commands;

public record AdminDeleteProfessorCommand(Guid ProfessorId) : IRequest<bool>;
