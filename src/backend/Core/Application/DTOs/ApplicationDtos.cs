namespace Application.DTOs;

public record UserProfileDto(Guid Id, string FullName, string Email, string Role);

public record AuthResponse(string Token, int ExpiresInSeconds, UserProfileDto User);

public record PublicStudentDto(Guid Id, string FullName);

public record ClassmateDto(Guid Id, string FullName);

public record StudentProfileDto(
    Guid Id,
    string FullName,
    string IdentificationNumber,
    string Email,
    int TotalCredits,
    int EnrolledCoursesCount
);

public record ProfessorDto(Guid Id, string FullName, string Email, bool IsAvailable);

public record CourseDto(Guid Id, string Code, string Name, int Credits, ProfessorDto Professor);

public record EnrolledCourseItemDto(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseCode,
    string CourseName,
    int Credits,
    Guid ProfessorId,
    string ProfessorName,
    DateTimeOffset EnrolledAtUtc
);

public record StudentEnrollmentSummaryDto(
    int TotalCredits,
    int MaxCredits,
    int EnrolledCoursesCount,
    int MaxCourses,
    IReadOnlyList<EnrolledCourseItemDto> Courses
);
