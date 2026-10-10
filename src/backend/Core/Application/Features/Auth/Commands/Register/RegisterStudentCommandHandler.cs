using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;
using Domain.Entities;

namespace Application.Features.Auth.Commands;

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result<AuthResponse>>
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

    public async Task<Result<AuthResponse>> HandleAsync(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return Result<AuthResponse>.Failure("El nombre completo es obligatorio.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
            return Result<AuthResponse>.Failure("El documento de identificación es obligatorio.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            return Result<AuthResponse>.Failure("El correo electrónico es inválido.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return Result<AuthResponse>.Failure("La contraseña debe tener al menos 6 caracteres.", ErrorType.Validation);

        if (await _userRepository.ExistsByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken))
            return Result<AuthResponse>.Failure("Ya existe un usuario registrado con el correo electrónico proporcionado.", ErrorType.Conflict);

        if (await _studentRepository.ExistsByIdentificationNumberAsync(request.IdentificationNumber.Trim(), cancellationToken))
            return Result<AuthResponse>.Failure("Ya existe un estudiante registrado con el documento de identificación proporcionado.", ErrorType.Conflict);

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

        return Result<AuthResponse>.Success(new AuthResponse(token, 86400, userDto), "Estudiante registrado exitosamente.");
    }
}
