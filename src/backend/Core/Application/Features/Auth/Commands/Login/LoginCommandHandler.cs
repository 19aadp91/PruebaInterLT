using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Common;

namespace Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
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

    public async Task<Result<AuthResponse>> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("Credenciales de acceso inválidas.", ErrorType.Unauthorized);

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user == null)
            return Result<AuthResponse>.Failure("Credenciales de acceso inválidas.", ErrorType.Unauthorized);

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return Result<AuthResponse>.Failure("Credenciales de acceso inválidas.", ErrorType.Unauthorized);

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserProfileDto(user.Id, user.FullName, user.Email, user.Role);

        return Result<AuthResponse>.Success(new AuthResponse(token, 86400, userDto), "Inicio de sesión exitoso.");
    }
}
