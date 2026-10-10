using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Auth;

public record RegisterStudentCommand(
    string FullName,
    string IdentificationNumber,
    string Email,
    string Password
) : IRequest<AuthResponse>;

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterStudentCommandHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> HandleAsync(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new DomainValidationException("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
            throw new DomainValidationException("El documento de identificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            throw new DomainValidationException("El correo electrónico es inválido.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new DomainValidationException("La contraseña debe tener al menos 6 caracteres.");

        if (await _userRepository.ExistsByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken))
            throw new ConflictException("Ya existe un usuario registrado con el correo electrónico proporcionado.");

        if (await _studentRepository.ExistsByIdentificationNumberAsync(request.IdentificationNumber.Trim(), cancellationToken))
            throw new ConflictException("Ya existe un estudiante registrado con el documento de identificación proporcionado.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = "Estudiante"
        };

        var student = new Student
        {
            Id = user.Id,
            IdentificationNumber = request.IdentificationNumber.Trim()
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _studentRepository.AddAsync(student, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserProfileDto(user.Id, user.FullName, user.Email, user.Role);

        return new AuthResponse(token, 86400, userDto);
    }
}

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new UnauthorizedException("Credenciales de acceso inválidas.");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user == null)
            throw new UnauthorizedException("Credenciales de acceso inválidas.");

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new UnauthorizedException("Credenciales de acceso inválidas.");

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserProfileDto(user.Id, user.FullName, user.Email, user.Role);

        return new AuthResponse(token, 86400, userDto);
    }
}
