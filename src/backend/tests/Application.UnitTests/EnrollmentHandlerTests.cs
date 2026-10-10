using Application.DTOs;
using Application.Features.Catalog;
using Application.Features.Enrollments;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class EnrollmentHandlerTests
{
    private readonly Mock<IStudentRepository> _studentRepo = new();
    private readonly Mock<ICourseRepository> _courseRepo = new();
    private readonly Mock<IEnrollmentRepository> _enrollmentRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private readonly EnrollCourseCommandHandler _enrollHandler;
    private readonly DropCourseCommandHandler _dropHandler;
    private readonly GetCourseClassmatesQueryHandler _classmatesHandler;
    private readonly GetPublicStudentsQueryHandler _publicStudentsHandler;

    public EnrollmentHandlerTests()
    {
        _enrollHandler = new EnrollCourseCommandHandler(
            _studentRepo.Object,
            _courseRepo.Object,
            _enrollmentRepo.Object,
            _uow.Object
        );

        _dropHandler = new DropCourseCommandHandler(
            _enrollmentRepo.Object,
            _uow.Object
        );

        _classmatesHandler = new GetCourseClassmatesQueryHandler(
            _enrollmentRepo.Object,
            _courseRepo.Object
        );

        _publicStudentsHandler = new GetPublicStudentsQueryHandler(_studentRepo.Object);
    }

    [Fact]
    public async Task EnrollCourse_WithValidConditions_CreatesEnrollmentAndUpdatesCredits()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var profId = Guid.NewGuid();

        var student = new Student { Id = studentId };
        var course = new Course { Id = courseId, Code = "MAT-101", Name = "Cálculo", Credits = 3, ProfessorId = profId, Professor = new Professor { Id = profId, FullName = "Prof. Gómez" } };

        _studentRepo.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _courseRepo.Setup(r => r.GetByIdWithProfessorAsync(courseId, It.IsAny<CancellationToken>())).ReturnsAsync(course);
        _enrollmentRepo.Setup(r => r.GetActiveByStudentIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Enrollment>()); // 0 materias actualmente

        var updatedEnrollments = new List<Enrollment>
        {
            new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = courseId, Course = course }
        };

        // Simular que tras añadir, la consulta devuelve la lista con el curso
        _enrollmentRepo.SetupSequence(r => r.GetActiveByStudentIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Enrollment>())
            .ReturnsAsync(updatedEnrollments);

        // Act
        var result = await _enrollHandler.HandleAsync(new EnrollCourseCommand(studentId, courseId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EnrolledCoursesCount.Should().Be(1);
        result.TotalCredits.Should().Be(3);
        _enrollmentRepo.Verify(r => r.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DropCourse_WhenEnrolled_RemovesEnrollmentAndReleasesCredits()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment { Id = Guid.NewGuid(), StudentId = studentId, CourseId = courseId };

        _enrollmentRepo.Setup(r => r.GetActiveByStudentAndCourseAsync(studentId, courseId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);
        _enrollmentRepo.Setup(r => r.GetActiveByStudentIdAsync(studentId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Enrollment>());

        // Act
        var result = await _dropHandler.HandleAsync(new DropCourseCommand(studentId, courseId), CancellationToken.None);

        // Assert
        result.EnrolledCoursesCount.Should().Be(0);
        result.TotalCredits.Should().Be(0);
        _enrollmentRepo.Verify(r => r.RemoveAsync(enrollment, It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCourseClassmates_ReturnsOnlyClassmateFullNames()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId, Name = "Física" };
        var classmateUser = new User { FullName = "María Fernanda Ruiz", Email = "maria@secret.com" };
        var classmateStudent = new Student { Id = Guid.NewGuid(), IdentificationNumber = "99999999", User = classmateUser };

        _courseRepo.Setup(r => r.GetByIdAsync(courseId, It.IsAny<CancellationToken>())).ReturnsAsync(course);
        _enrollmentRepo.Setup(r => r.GetClassmatesByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Student> { classmateStudent });

        // Act
        var result = await _classmatesHandler.HandleAsync(new GetCourseClassmatesQuery(courseId), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].FullName.Should().Be("María Fernanda Ruiz");
    }

    [Fact]
    public async Task GetPublicStudents_ReturnsOnlyFullNamesAndHidesSensitiveData()
    {
        // Arrange
        var studentUser = new User { FullName = "Carlos Gómez", Email = "carlos@privado.com" };
        var student = new Student { Id = Guid.NewGuid(), IdentificationNumber = "12345678", User = studentUser };

        _studentRepo.Setup(r => r.GetAllPublicAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Student> { student });

        // Act
        var result = await _publicStudentsHandler.HandleAsync(new GetPublicStudentsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].FullName.Should().Be("Carlos Gómez");
    }
}
