using Application.Ports;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DbInitializer(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        if (await _context.Professors.AnyAsync())
            return; // Ya fue inicializado

        // 1. Crear Profesores
        var prof1 = new Professor { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), FullName = "Prof. Andrés Gómez", Email = "andres.gomez@universidad.edu.co" };
        var prof2 = new Professor { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), FullName = "Prof. Beatriz Morales", Email = "beatriz.morales@universidad.edu.co" };
        var prof3 = new Professor { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), FullName = "Prof. Carlos Herrera", Email = "carlos.herrera@universidad.edu.co" };
        var prof4 = new Professor { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), FullName = "Prof. Diana Ramírez", Email = "diana.ramirez@universidad.edu.co" };
        var prof5 = new Professor { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), FullName = "Prof. Eduardo Torres", Email = "eduardo.torres@universidad.edu.co" };

        await _context.Professors.AddRangeAsync(prof1, prof2, prof3, prof4, prof5);

        // 2. Crear las 10 Materias (2 por cada profesor, 3 créditos c/u)
        var courses = new List<Course>
        {
            new() { Id = Guid.Parse("A1111111-0000-0000-0000-000000000001"), Code = "MAT-101", Name = "Cálculo Diferencial", Credits = 3, ProfessorId = prof1.Id },
            new() { Id = Guid.Parse("A1111111-0000-0000-0000-000000000002"), Code = "MAT-102", Name = "Álgebra Lineal", Credits = 3, ProfessorId = prof1.Id },
            new() { Id = Guid.Parse("A2222222-0000-0000-0000-000000000001"), Code = "FIS-201", Name = "Física Mecánica", Credits = 3, ProfessorId = prof2.Id },
            new() { Id = Guid.Parse("A2222222-0000-0000-0000-000000000002"), Code = "FIS-202", Name = "Electromagnetismo", Credits = 3, ProfessorId = prof2.Id },
            new() { Id = Guid.Parse("A3333333-0000-0000-0000-000000000001"), Code = "PRG-301", Name = "Fundamentos de Programación", Credits = 3, ProfessorId = prof3.Id },
            new() { Id = Guid.Parse("A3333333-0000-0000-0000-000000000002"), Code = "PRG-302", Name = "Estructuras de Datos", Credits = 3, ProfessorId = prof3.Id },
            new() { Id = Guid.Parse("A4444444-0000-0000-0000-000000000001"), Code = "BD-401", Name = "Bases de Datos Relacionales", Credits = 3, ProfessorId = prof4.Id },
            new() { Id = Guid.Parse("A4444444-0000-0000-0000-000000000002"), Code = "BD-402", Name = "Modelado y Gestión de Datos", Credits = 3, ProfessorId = prof4.Id },
            new() { Id = Guid.Parse("A5555555-0000-0000-0000-000000000001"), Code = "SO-501", Name = "Sistemas Operativos", Credits = 3, ProfessorId = prof5.Id },
            new() { Id = Guid.Parse("A5555555-0000-0000-0000-000000000002"), Code = "RED-502", Name = "Redes de Computadores", Credits = 3, ProfessorId = prof5.Id }
        };

        await _context.Courses.AddRangeAsync(courses);

        // 3. Crear Usuario Administrador
        var adminUser = new User
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            FullName = "Administrador del Sistema",
            Email = "admin@universidad.edu.co",
            PasswordHash = _passwordHasher.HashPassword("Admin123!"),
            Role = "Administrador"
        };

        await _context.Users.AddAsync(adminUser);
        await _context.SaveChangesAsync();
    }
}
