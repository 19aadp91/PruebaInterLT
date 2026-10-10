using Domain.Common;

namespace Domain.Entities;

public class Course : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; } = 3;
    public Guid ProfessorId { get; set; }

    public Professor? Professor { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
