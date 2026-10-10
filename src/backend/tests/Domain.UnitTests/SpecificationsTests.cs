using Domain.Entities;
using Domain.Specifications;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests;

public class SpecificationsTests
{
    private readonly Professor _profGomez = new() { Id = Guid.NewGuid(), FullName = "Prof. Andrés Gómez" };
    private readonly Professor _profMorales = new() { Id = Guid.NewGuid(), FullName = "Prof. Beatriz Morales" };
    private readonly Professor _profHerrera = new() { Id = Guid.NewGuid(), FullName = "Prof. Carlos Herrera" };

    [Fact]
    public void MaxCoursesSpecification_WhenUnderThreeCourses_IsSatisfied()
    {
        // Arrange
        var spec = new MaxCoursesSpecification();
        var currentCourses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Name = "Cálculo", ProfessorId = _profGomez.Id, Professor = _profGomez },
            new() { Id = Guid.NewGuid(), Name = "Física", ProfessorId = _profMorales.Id, Professor = _profMorales }
        };
        var newCourse = new Course { Id = Guid.NewGuid(), Name = "Programación", ProfessorId = _profHerrera.Id, Professor = _profHerrera };
        var candidate = new EnrollmentCandidate(currentCourses, newCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void MaxCoursesSpecification_WhenThreeCoursesAlreadyEnrolled_Fails()
    {
        // Arrange
        var spec = new MaxCoursesSpecification();
        var currentCourses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Name = "Materia 1" },
            new() { Id = Guid.NewGuid(), Name = "Materia 2" },
            new() { Id = Guid.NewGuid(), Name = "Materia 3" }
        };
        var newCourse = new Course { Id = Guid.NewGuid(), Name = "Materia 4" };
        var candidate = new EnrollmentCandidate(currentCourses, newCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Contain("más de 3 materias");
    }

    [Fact]
    public void UniqueProfessorSpecification_WhenProfessorNotEnrolled_IsSatisfied()
    {
        // Arrange
        var spec = new UniqueProfessorSpecification();
        var currentCourses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Name = "Cálculo", ProfessorId = _profGomez.Id, Professor = _profGomez }
        };
        var newCourse = new Course { Id = Guid.NewGuid(), Name = "Física", ProfessorId = _profMorales.Id, Professor = _profMorales };
        var candidate = new EnrollmentCandidate(currentCourses, newCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void UniqueProfessorSpecification_WhenProfessorAlreadyEnrolled_Fails()
    {
        // Arrange
        var spec = new UniqueProfessorSpecification();
        var currentCourses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Name = "Cálculo Diferencial", ProfessorId = _profGomez.Id, Professor = _profGomez }
        };
        // Intenta inscribir Álgebra Lineal que también la dicta el Prof. Gómez
        var newCourse = new Course { Id = Guid.NewGuid(), Name = "Álgebra Lineal", ProfessorId = _profGomez.Id, Professor = _profGomez };
        var candidate = new EnrollmentCandidate(currentCourses, newCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Contain("Incompatibilidad académica");
        spec.ErrorMessage.Should().Contain("Prof. Andrés Gómez");
    }

    [Fact]
    public void NotAlreadyEnrolledSpecification_WhenCourseAlreadyEnrolled_Fails()
    {
        // Arrange
        var spec = new NotAlreadyEnrolledSpecification();
        var courseId = Guid.NewGuid();
        var currentCourses = new List<Course>
        {
            new() { Id = courseId, Name = "Cálculo Diferencial" }
        };
        var duplicateCourse = new Course { Id = courseId, Name = "Cálculo Diferencial" };
        var candidate = new EnrollmentCandidate(currentCourses, duplicateCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeFalse();
        spec.ErrorMessage.Should().Contain("ya se encuentra inscrita");
    }

    [Fact]
    public void NotAlreadyEnrolledSpecification_WhenCourseNotEnrolled_IsSatisfied()
    {
        // Arrange
        var spec = new NotAlreadyEnrolledSpecification();
        var currentCourses = new List<Course>
        {
            new() { Id = Guid.NewGuid(), Name = "Cálculo Diferencial" }
        };
        var newCourse = new Course { Id = Guid.NewGuid(), Name = "Física Mecánica" };
        var candidate = new EnrollmentCandidate(currentCourses, newCourse);

        // Act
        var result = spec.IsSatisfiedBy(candidate);

        // Assert
        result.Should().BeTrue();
    }
}
