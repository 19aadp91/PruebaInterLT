using Domain.Common;

namespace Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTimeOffset EnrolledAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public Student? Student { get; set; }
    public Course? Course { get; set; }
}
