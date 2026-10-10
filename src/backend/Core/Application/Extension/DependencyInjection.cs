using Application.Common.Dispatcher;
using Application.Common.Validation;
using Application.DTOs;
using Application.Features.Admin.Commands;
using Application.Features.Auth.Commands;
using Application.Features.Catalog.Queries;
using Application.Features.Enrollments.Commands;
using Application.Features.Enrollments.Queries;
using Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extension;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDispatcher(this IServiceCollection services, Assembly? assembly = null)
    {
        services.AddScoped<IDispatcher, NativeDispatcher>();

        // Registro determinista y seguro de Handlers y Validadores
        // Evita ReflectionTypeLoadException en entornos protegidos por directivas de Code Integrity / Smart App Control
        RegisterHandlersAndValidators(services);

        return services;
    }

    private static void RegisterHandlersAndValidators(IServiceCollection services)
    {
        // 1. Handlers
        services.AddScoped<IRequestHandler<RegisterStudentCommand, Result<AuthResponse>>, RegisterStudentCommandHandler>();
        services.AddScoped<IRequestHandler<LoginCommand, Result<AuthResponse>>, LoginCommandHandler>();
        services.AddScoped<IRequestHandler<GetCoursesQuery, Result<IReadOnlyList<CourseDto>>>, GetCoursesQueryHandler>();
        services.AddScoped<IRequestHandler<GetPublicStudentsQuery, Result<IReadOnlyList<PublicStudentDto>>>, GetPublicStudentsQueryHandler>();
        services.AddScoped<IRequestHandler<GetStudentProfileQuery, Result<StudentProfileDto>>, GetStudentProfileQueryHandler>();
        services.AddScoped<IRequestHandler<EnrollCourseCommand, Result<StudentEnrollmentSummaryDto>>, EnrollCourseCommandHandler>();
        services.AddScoped<IRequestHandler<DropCourseCommand, Result<StudentEnrollmentSummaryDto>>, DropCourseCommandHandler>();
        services.AddScoped<IRequestHandler<GetCourseClassmatesQuery, Result<IReadOnlyList<ClassmateDto>>>, GetCourseClassmatesQueryHandler>();
        services.AddScoped<IRequestHandler<GetMyEnrollmentsQuery, Result<StudentEnrollmentSummaryDto>>, GetMyEnrollmentsQueryHandler>();
        services.AddScoped<IRequestHandler<AdminCreateCourseCommand, Result<CourseDto>>, AdminCreateCourseCommandHandler>();
        services.AddScoped<IRequestHandler<AdminDeleteCourseCommand, Result>, AdminDeleteCourseCommandHandler>();
        services.AddScoped<IRequestHandler<AdminCreateProfessorCommand, Result<ProfessorDto>>, AdminCreateProfessorCommandHandler>();
        services.AddScoped<IRequestHandler<AdminDeleteProfessorCommand, Result>, AdminDeleteProfessorCommandHandler>();

        // 2. Validadores
        services.AddScoped<IValidator<RegisterStudentCommand>, RegisterStudentCommandValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
        services.AddScoped<IValidator<GetCoursesQuery>, GetCoursesQueryValidator>();
        services.AddScoped<IValidator<GetPublicStudentsQuery>, GetPublicStudentsQueryValidator>();
        services.AddScoped<IValidator<GetStudentProfileQuery>, GetStudentProfileQueryValidator>();
        services.AddScoped<IValidator<EnrollCourseCommand>, EnrollCourseCommandValidator>();
        services.AddScoped<IValidator<DropCourseCommand>, DropCourseCommandValidator>();
        services.AddScoped<IValidator<GetCourseClassmatesQuery>, GetCourseClassmatesQueryValidator>();
        services.AddScoped<IValidator<GetMyEnrollmentsQuery>, GetMyEnrollmentsQueryValidator>();
        services.AddScoped<IValidator<AdminCreateCourseCommand>, AdminCreateCourseCommandValidator>();
        services.AddScoped<IValidator<AdminDeleteCourseCommand>, AdminDeleteCourseCommandValidator>();
        services.AddScoped<IValidator<AdminCreateProfessorCommand>, AdminCreateProfessorCommandValidator>();
        services.AddScoped<IValidator<AdminDeleteProfessorCommand>, AdminDeleteProfessorCommandValidator>();
    }
}
