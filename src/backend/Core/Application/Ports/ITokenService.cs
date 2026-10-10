using Domain.Entities;

namespace Application.Ports;

public interface ITokenService
{
    string GenerateToken(User user);
}
