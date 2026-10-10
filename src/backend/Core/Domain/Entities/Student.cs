using Domain.Common;

namespace Domain.Entities;

public class Student : BaseEntity
{
    public string IdentificationNumber { get; set; } = string.Empty;

    public User? User { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
