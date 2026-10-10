using Domain.Common;

namespace Domain.Entities;

public class Professor : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
