using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Ports;
using Domain.Exceptions;

namespace Application.Features.Auth.Commands;

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
