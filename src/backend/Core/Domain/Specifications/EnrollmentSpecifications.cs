using Domain.Common;
using Domain.Entities;

namespace Domain.Specifications;

public class EnrollmentCandidate
{
    public IReadOnlyList<Course> CurrentlyEnrolledCourses { get; }
    public Course CourseToEnroll { get; }

    public EnrollmentCandidate(IReadOnlyList<Course> currentlyEnrolledCourses, Course courseToEnroll)
    {
        CurrentlyEnrolledCourses = currentlyEnrolledCourses ?? Array.Empty<Course>();
        CourseToEnroll = courseToEnroll ?? throw new ArgumentNullException(nameof(courseToEnroll));
    }
}

public class MaxCoursesSpecification : ISpecification<EnrollmentCandidate>
{
    private const int MaxCoursesAllowed = 3;

    public string ErrorMessage => "No es posible inscribir más de 3 materias (límite máximo de 9 créditos alcanzado).";

    public bool IsSatisfiedBy(EnrollmentCandidate candidate)
    {
        return candidate.CurrentlyEnrolledCourses.Count < MaxCoursesAllowed;
    }
}

public class UniqueProfessorSpecification : ISpecification<EnrollmentCandidate>
{
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool IsSatisfiedBy(EnrollmentCandidate candidate)
    {
        var existingSameProfessor = candidate.CurrentlyEnrolledCourses
            .FirstOrDefault(c => c.ProfessorId == candidate.CourseToEnroll.ProfessorId);

        if (existingSameProfessor != null)
        {
            var profName = existingSameProfessor.Professor?.FullName ?? "el mismo profesor";
            ErrorMessage = $"Incompatibilidad académica: no es posible inscribir '{candidate.CourseToEnroll.Name}' porque ya tiene inscrita la materia '{existingSameProfessor.Name}' dictada por {profName}.";
            return false;
        }

        return true;
    }
}

public class NotAlreadyEnrolledSpecification : ISpecification<EnrollmentCandidate>
{
    public string ErrorMessage { get; private set; } = string.Empty;

    public bool IsSatisfiedBy(EnrollmentCandidate candidate)
    {
        var alreadyEnrolled = candidate.CurrentlyEnrolledCourses.Any(c => c.Id == candidate.CourseToEnroll.Id);
        if (alreadyEnrolled)
        {
            ErrorMessage = $"La materia '{candidate.CourseToEnroll.Name}' ya se encuentra inscrita en su carga académica.";
            return false;
        }

        return true;
    }
}
