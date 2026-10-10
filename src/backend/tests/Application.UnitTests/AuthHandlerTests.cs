using Application.Features.Auth.Commands;
using Application.Ports;
using Domain.Common;
using Domain.Entities;
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
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("jwt_sample_token");
        result.Data.User.FullName.Should().Be("Carlos Perez");
        result.Data.User.Email.Should().Be("carlos@test.com");
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterStudent_WithExistingEmailOrId_ReturnsConflictResult()
    {
        // Arrange
        var command = new RegisterStudentCommand("Carlos Perez", "1020304050", "carlos@test.com", "Password123!");
        _userRepo.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _registerHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Conflict);
        result.Message.Should().Contain("correo electrónico");
    }

    [Fact]
    public async Task RegisterStudent_WithShortPassword_ReturnsValidationResult()
    {
        // Arrange
        var command = new RegisterStudentCommand("Carlos Perez", "1020304050", "carlos@test.com", "123");

        // Act
        var result = await _registerHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Validation);
        result.Message.Should().Contain("al menos 6 caracteres");
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
        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().Be("token_abc");
        result.Data.User.Email.Should().Be("carlos@test.com");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("inexistente@test.com", "123456");

        // Act
        var result = await _loginHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Unauthorized);
        result.Message.Should().Contain("inválidas");
    }
}
