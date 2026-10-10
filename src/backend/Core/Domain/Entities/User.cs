using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Estudiante"; // "Estudiante", "Administrador"

    public Student? Student { get; set; }
}
