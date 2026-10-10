-- ==========================================================================================
-- SCRIPT DE CREACIÓN DE BASE DE DATOS Y DATOS SEMILLA
-- Motor: Microsoft SQL Server 2022+ / Azure SQL Database
-- Cumplimiento: Auditoría canónica, integridad referencial e índices optimizados
-- ==========================================================================================

SET NOCOUNT ON;
GO

-- 1. TABLA: Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [FullName]         NVARCHAR(150)    NOT NULL,
        [Email]            NVARCHAR(150)    NOT NULL,
        [PasswordHash]     NVARCHAR(256)    NOT NULL,
        [Role]             NVARCHAR(30)     NOT NULL,
        [CreatedAtUtc]     DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc]     DATETIMEOFFSET   NULL,
        [IsDeleted]        BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
END
GO

-- 2. TABLA: Students
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Students] (
        [Id]                   UNIQUEIDENTIFIER NOT NULL,
        [IdentificationNumber] NVARCHAR(50)     NOT NULL,
        [CreatedAtUtc]         DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc]         DATETIMEOFFSET   NULL,
        [IsDeleted]            BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Students_Users] FOREIGN KEY ([Id]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Students_IdentificationNumber] UNIQUE NONCLUSTERED ([IdentificationNumber] ASC)
    );
END
GO

-- 3. TABLA: Professors
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Professors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Professors] (
        [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [FullName]     NVARCHAR(150)    NOT NULL,
        [Email]        NVARCHAR(150)    NOT NULL,
        [IsAvailable]  BIT              NOT NULL DEFAULT 1,
        [CreatedAtUtc] DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc] DATETIMEOFFSET   NULL,
        [IsDeleted]    BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Professors] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Professors_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );
END
GO

-- 4. TABLA: Courses
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Courses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Courses] (
        [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Code]         NVARCHAR(20)     NOT NULL,
        [Name]         NVARCHAR(150)    NOT NULL,
        [Credits]      INT              NOT NULL DEFAULT 3,
        [ProfessorId]  UNIQUEIDENTIFIER NOT NULL,
        [CreatedAtUtc] DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc] DATETIMEOFFSET   NULL,
        [IsDeleted]    BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Courses_Code] UNIQUE NONCLUSTERED ([Code] ASC),
        CONSTRAINT [CK_Courses_Credits] CHECK ([Credits] = 3),
        CONSTRAINT [FK_Courses_Professors] FOREIGN KEY ([ProfessorId]) REFERENCES [dbo].[Professors] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- 5. TABLA: Enrollments
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Enrollments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Enrollments] (
        [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [StudentId]    UNIQUEIDENTIFIER NOT NULL,
        [CourseId]     UNIQUEIDENTIFIER NOT NULL,
        [EnrolledAtUtc]DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [CreatedAtUtc] DATETIMEOFFSET   NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc] DATETIMEOFFSET   NULL,
        [IsDeleted]    BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Enrollments] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Enrollments_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Enrollments_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id]) ON DELETE NO ACTION
    );
END
GO

-- 6. ÍNDICES DE RENDIMIENTO E INTEGRIDAD
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Courses_ProfessorId' AND object_id = OBJECT_ID(N'[dbo].[Courses]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Courses_ProfessorId] ON [dbo].[Courses] ([ProfessorId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Enrollments_StudentId' AND object_id = OBJECT_ID(N'[dbo].[Enrollments]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Enrollments_StudentId] ON [dbo].[Enrollments] ([StudentId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Enrollments_CourseId' AND object_id = OBJECT_ID(N'[dbo].[Enrollments]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Enrollments_CourseId] ON [dbo].[Enrollments] ([CourseId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'UQ_Enrollments_ActiveStudentCourse' AND object_id = OBJECT_ID(N'[dbo].[Enrollments]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_Enrollments_ActiveStudentCourse]
    ON [dbo].[Enrollments] ([StudentId] ASC, [CourseId] ASC)
    WHERE [IsDeleted] = 0;
END
GO

-- ==========================================================================================
-- 7. DATOS SEMILLA CANÓNICOS (Idempotentes)
-- ==========================================================================================

-- Variables con GUIDs deterministas para relaciones
DECLARE @Prof1Id UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Prof2Id UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Prof3Id UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Prof4Id UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @Prof5Id UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';

-- Insertar los 5 Profesores
IF NOT EXISTS (SELECT 1 FROM [dbo].[Professors] WHERE [Id] = @Prof1Id)
    INSERT INTO [dbo].[Professors] ([Id], [FullName], [Email]) VALUES (@Prof1Id, N'Prof. Andrés Gómez', N'andres.gomez@universidad.edu.co');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Professors] WHERE [Id] = @Prof2Id)
    INSERT INTO [dbo].[Professors] ([Id], [FullName], [Email]) VALUES (@Prof2Id, N'Prof. Beatriz Morales', N'beatriz.morales@universidad.edu.co');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Professors] WHERE [Id] = @Prof3Id)
    INSERT INTO [dbo].[Professors] ([Id], [FullName], [Email]) VALUES (@Prof3Id, N'Prof. Carlos Herrera', N'carlos.herrera@universidad.edu.co');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Professors] WHERE [Id] = @Prof4Id)
    INSERT INTO [dbo].[Professors] ([Id], [FullName], [Email]) VALUES (@Prof4Id, N'Prof. Diana Ramírez', N'diana.ramirez@universidad.edu.co');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Professors] WHERE [Id] = @Prof5Id)
    INSERT INTO [dbo].[Professors] ([Id], [FullName], [Email]) VALUES (@Prof5Id, N'Prof. Eduardo Torres', N'eduardo.torres@universidad.edu.co');

-- Insertar las 10 Materias (2 por cada profesor, 3 créditos c/u)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'MAT-101')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A1111111-0000-0000-0000-000000000001', N'MAT-101', N'Cálculo Diferencial', 3, @Prof1Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'MAT-102')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A1111111-0000-0000-0000-000000000002', N'MAT-102', N'Álgebra Lineal', 3, @Prof1Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'FIS-201')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A2222222-0000-0000-0000-000000000001', N'FIS-201', N'Física Mecánica', 3, @Prof2Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'FIS-202')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A2222222-0000-0000-0000-000000000002', N'FIS-202', N'Electromagnetismo', 3, @Prof2Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'PRG-301')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A3333333-0000-0000-0000-000000000001', N'PRG-301', N'Fundamentos de Programación', 3, @Prof3Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'PRG-302')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A3333333-0000-0000-0000-000000000002', N'PRG-302', N'Estructuras de Datos', 3, @Prof3Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'BD-401')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A4444444-0000-0000-0000-000000000001', N'BD-401', N'Bases de Datos Relacionales', 3, @Prof4Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'BD-402')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A4444444-0000-0000-0000-000000000002', N'BD-402', N'Modelado y Gestión de Datos', 3, @Prof4Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'SO-501')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A5555555-0000-0000-0000-000000000001', N'SO-501', N'Sistemas Operativos', 3, @Prof5Id);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Courses] WHERE [Code] = N'RED-502')
    INSERT INTO [dbo].[Courses] ([Id], [Code], [Name], [Credits], [ProfessorId]) VALUES ('A5555555-0000-0000-0000-000000000002', N'RED-502', N'Redes de Computadores', 3, @Prof5Id);

-- Insertar Usuario Administrador por defecto (Contraseña: Admin123!)
-- PBKDF2 hash formato estándar de la aplicación
DECLARE @AdminId UNIQUEIDENTIFIER = '99999999-9999-9999-9999-999999999999';
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Id] = @AdminId)
BEGIN
    INSERT INTO [dbo].[Users] ([Id], [FullName], [Email], [PasswordHash], [Role])
    VALUES (@AdminId, N'Administrador del Sistema', N'admin@universidad.edu.co', N'AQAAAAIAAYagAAAAENKz08vF0W+dK4QG+c8q8jF8qfGqO1e1+q7L5u8i7J9=', N'Administrador');
END
GO
