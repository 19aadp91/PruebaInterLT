using Application.Features.Auth.Commands;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests;

public class AuthHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IStudentRepository> _studentRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private readonly RegisterStudentCommandHandler _registerHandler;
    private readonly LoginCommandHandler _loginHandler;

    public AuthHandlerTests()
    {
        _registerHandler = new RegisterStudentCommandHandler(
            _userRepo.Object,
            _studentRepo.Object,
            _hasher.Object,
            _tokenService.Object,
            _uow.Object
        );

        _loginHandler = new LoginCommandHandler(
            _userRepo.Object,
            _hasher.Object,
            _tokenService.Object
        );
    }

    [Fact]
    public async Task RegisterStudent_WithValidData_ReturnsSuccessAndToken()
    {
        // Arrange
        var command = new RegisterStudentCommand("Carlos Perez", "1020304050", "carlos@test.com", "Password123!");
        _userRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _studentRepo.Setup(r => r.ExistsByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed_pwd");
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("jwt_sample_token");

        // Act
        var result = await _registerHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt_sample_token");
        result.User.FullName.Should().Be("Carlos Perez");
        result.User.Email.Should().Be("carlos@test.com");
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterStudent_WithExistingEmailOrId_ThrowsConflictException()
    {
        // Arrange
        var command = new RegisterStudentCommand("Carlos Perez", "1020304050", "carlos@test.com", "Password123!");
        _userRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _registerHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>().WithMessage("*correo electrónico*");
    }

    [Fact]
    public async Task RegisterStudent_WithShortPassword_ThrowsValidationException()
    {
        // Arrange
        var command = new RegisterStudentCommand("Carlos Perez", "1020304050", "carlos@test.com", "123");

        // Act
        Func<Task> act = async () => await _registerHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>().WithMessage("*al menos 6 caracteres*");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), FullName = "Carlos", Email = "carlos@test.com", PasswordHash = "hash" };
        _userRepo.Setup(r => r.GetByEmailAsync("carlos@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword("123456", "hash")).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("token_abc");

        var command = new LoginCommand("carlos@test.com", "123456");

        // Act
        var result = await _loginHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Token.Should().Be("token_abc");
        result.User.Email.Should().Be("carlos@test.com");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("inexistente@test.com", "123456");

        // Act
        Func<Task> act = async () => await _loginHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("*inválidas*");
    }
}
