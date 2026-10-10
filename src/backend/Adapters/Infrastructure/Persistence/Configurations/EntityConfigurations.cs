using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(30).IsRequired();
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.IdentificationNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.IdentificationNumber).IsUnique();

        builder.HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professors");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FullName).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(150).IsRequired();
        builder.HasIndex(p => p.Email).IsUnique();
        builder.Property(p => p.IsAvailable).IsRequired().HasDefaultValue(true);
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(c => c.Code).IsUnique();
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Credits).IsRequired().HasDefaultValue(3);

        builder.HasOne(c => c.Professor)
            .WithMany(p => p.Courses)
            .HasForeignKey(c => c.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.ProfessorId).HasDatabaseName("IX_Courses_ProfessorId");
    }
}

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EnrolledAtUtc).IsRequired();

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.StudentId).HasDatabaseName("IX_Enrollments_StudentId");
        builder.HasIndex(e => e.CourseId).HasDatabaseName("IX_Enrollments_CourseId");
    }
}
