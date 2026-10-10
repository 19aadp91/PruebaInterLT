using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Catalog.Queries;

public record GetPublicStudentsQuery : IRequest<IReadOnlyList<PublicStudentDto>>;
