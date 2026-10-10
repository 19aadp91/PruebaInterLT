using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public record GetStudentProfileQuery(Guid StudentId) : IRequest<Result<StudentProfileDto>>;
