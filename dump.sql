-- =============================================
-- Script para criação do banco e tabelas
-- =============================================

-- Criar banco de dados (se não existir)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SQL_FALUNP')
BEGIN
    CREATE DATABASE SQL_FALUNP;
    PRINT 'Banco SQL_FALUNP criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Banco SQL_FALUNP já existe.';
END
GO

USE SQL_FALUNP;
GO

-- ... (o restante da criação das tabelas permanece igual) ...
-- =============================================
-- Tabela: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE dbo.Users (
        Id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Password NVARCHAR(256) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        UpdatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY (Id)
    );
    
    -- Índice único para email
    CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email 
    ON dbo.Users (Email);
    
    PRINT 'Tabela Users criada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Tabela Users já existe.';
END
GO

-- =============================================
-- Tabela: Students
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Students' AND xtype='U')
BEGIN
    CREATE TABLE dbo.Students (
        Id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        BirthDate DATETIME2 NOT NULL,
        CPF NVARCHAR(11) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Password NVARCHAR(256) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        UpdatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        CONSTRAINT PK_Students PRIMARY KEY (Id)
    );
    
    -- Índice único para email
    CREATE UNIQUE NONCLUSTERED INDEX IX_Students_Email 
    ON dbo.Students (Email);
    
    -- Índice único para CPF
    CREATE UNIQUE NONCLUSTERED INDEX IX_Students_CPF 
    ON dbo.Students (CPF);
    
    PRINT 'Tabela Students criada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Tabela Students já existe.';
END
GO

-- =============================================
-- Tabela: Classes
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Classes' AND xtype='U')
BEGIN
    CREATE TABLE dbo.Classes (
        Id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        UpdatedAt DATETIME2 DEFAULT GETDATE() NOT NULL,
        CONSTRAINT PK_Classes PRIMARY KEY (Id)
    );
    
    -- Índice único para nome da classe
    CREATE UNIQUE NONCLUSTERED INDEX IX_Classes_Name 
    ON dbo.Classes (Name);
    
    PRINT 'Tabela Classes criada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Tabela Classes já existe.';
END
GO

-- =============================================
-- Tabela: Enrollments (Matrículas)
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Enrollments' AND xtype='U')
BEGIN
    CREATE TABLE dbo.Enrollments (
        StudentId UNIQUEIDENTIFIER NOT NULL,
        ClassId UNIQUEIDENTIFIER NOT NULL,
        RegistrationDate DATETIME2 DEFAULT GETDATE() NOT NULL,
        CONSTRAINT PK_Enrollments PRIMARY KEY (StudentId, ClassId),
        CONSTRAINT FK_Enrollments_Students_StudentId 
            FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Enrollments_Classes_ClassId 
            FOREIGN KEY (ClassId) REFERENCES dbo.Classes(Id) ON DELETE CASCADE
    );
    
    -- Índice para melhor performance nas consultas por ClassId
    CREATE NONCLUSTERED INDEX IX_Enrollments_ClassId 
    ON dbo.Enrollments (ClassId);
    
    PRINT 'Tabela Enrollments criada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Tabela Enrollments já existe.';
END
GO

-- =============================================
-- Tabela: __EFMigrationsHistory (para Entity Framework)
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE dbo.[__EFMigrationsHistory] (
        MigrationId NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32) NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
    
    PRINT 'Tabela __EFMigrationsHistory criada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Tabela __EFMigrationsHistory já existe.';
END
GO

PRINT 'Todas as tabelas foram criadas/verificadas com sucesso!';
GO

-- =============================================
-- Script para inserção de dados iniciais
-- =============================================

USE [SQL_FALUNP];
GO

PRINT 'Iniciando inserção de dados iniciais...';
GO

-- =============================================
-- Inserir Usuários
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@falunp.com.br')
BEGIN
    INSERT INTO Users (Id, Name, Email, Password, CreatedAt, UpdatedAt) 
    VALUES (
        'd7dbf939-1bf6-40e2-89c8-6919165892d0',
        'Administrador Sistema', 
        'admin@falunp.com.br', 
        '$2a$12$KJoKGv1UDVCi7dXpg4Rvxe8.pKcG3w9PaSE16uEaRWh0LnY2VaMvW', -- Senha: Admin@123
        GETDATE(),
        GETDATE()
    );
    PRINT 'Usuário admin inserido com sucesso.';
END
ELSE
BEGIN
    PRINT 'Usuário admin já existe.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'professor@falunp.com.br')
BEGIN
    INSERT INTO Users (Id, Name, Email, Password, CreatedAt, UpdatedAt) 
    VALUES (
        NEWID(),
        'Professor Silva', 
        'professor@falunp.com.br', 
        '$2a$12$KJoKGv1UDVCi7dXpg4Rvxe8.pKcG3w9PaSE16uEaRWh0LnY2VaMvW', -- Senha: Admin@123
        GETDATE(),
        GETDATE()
    );
    PRINT 'Usuário professor inserido com sucesso.';
END
GO

-- =============================================
-- Inserir Students (Alunos)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Students WHERE Email = 'aluno1@falunp.com.br')
BEGIN
    INSERT INTO Students (Id, Name, BirthDate, CPF, Email, Password, CreatedAt, UpdatedAt) 
    VALUES (
        'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
        'João da Silva', 
        '2000-05-15',
        '12345678901',
        'aluno1@falunp.com.br', 
        '$2a$12$KJoKGv1UDVCi7dXpg4Rvxe8.pKcG3w9PaSE16uEaRWh0LnY2VaMvW', -- Senha: Admin@123
        GETDATE(),
        GETDATE()
    );
    PRINT 'Aluno 1 inserido com sucesso.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Students WHERE Email = 'aluno2@falunp.com.br')
BEGIN
    INSERT INTO Students (Id, Name, BirthDate, CPF, Email, Password, CreatedAt, UpdatedAt) 
    VALUES (
        'b2c3d4e5-f6g7-8901-bcde-f23456789012',
        'Maria Santos', 
        '2001-08-22',
        '98765432109',
        'aluno2@falunp.com.br', 
        '$2a$12$KJoKGv1UDVCi7dXpg4Rvxe8.pKcG3w9PaSE16uEaRWh0LnY2VaMvW', -- Senha: Admin@123
        GETDATE(),
        GETDATE()
    );
    PRINT 'Aluno 2 inserido com sucesso.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Students WHERE Email = 'aluno3@falunp.com.br')
BEGIN
    INSERT INTO Students (Id, Name, BirthDate, CPF, Email, Password, CreatedAt, UpdatedAt) 
    VALUES (
        'c3d4e5f6-g7h8-9012-cdef-345678901234',
        'Pedro Oliveira', 
        '1999-12-03',
        '45678912345',
        'aluno3@falunp.com.br', 
        '$2a$12$KJoKGv1UDVCi7dXpg4Rvxe8.pKcG3w9PaSE16uEaRWh0LnY2VaMvW', -- Senha: Admin@123
        GETDATE(),
        GETDATE()
    );
    PRINT 'Aluno 3 inserido com sucesso.';
END
GO

-- =============================================
-- Inserir Classes (Turmas) - CORRIGIDO
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Classes WHERE Name = 'Matemática Discreta')
BEGIN
    INSERT INTO Classes (Id, Name, Description, CreatedAt, UpdatedAt)
    VALUES (
        '04ef27c1-fc9e-44e3-83eb-e589fa0f6777',
        'Matemática Discreta',
        'Matemática com conceitos mais avançados e lógica formal',
        GETDATE(),
        GETDATE()
    );
    PRINT 'Turma Matemática Discreta inserida com sucesso.';
END
ELSE
BEGIN
    PRINT 'Turma Matemática Discreta já existe.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Classes WHERE Name = 'Desenvolvimento .NET')
BEGIN
    INSERT INTO Classes (Id, Name, Description, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'Desenvolvimento .NET',
        'Desenvolvimento de aplicações modernas com .NET Core e C#',
        GETDATE(),
        GETDATE()
    );
    PRINT 'Turma Desenvolvimento .NET inserida com sucesso.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Classes WHERE Name = 'Arquitetura de Software')
BEGIN
    INSERT INTO Classes (Id, Name, Description, CreatedAt, UpdatedAt)
    VALUES (
        NEWID(),
        'Arquitetura de Software',
        'Padrões arquiteturais, DDD, Clean Architecture e microsserviços',
        GETDATE(),
        GETDATE()
    );
    PRINT 'Turma Arquitetura de Software inserida com sucesso.';
END
GO

-- =============================================
-- Inserir Enrollments (Matrículas) - CORRIGIDO
-- =============================================
-- Matricular alunos em Matemática Discreta
IF NOT EXISTS (SELECT 1 FROM Enrollments WHERE StudentId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890' AND ClassId = '04ef27c1-fc9e-44e3-83eb-e589fa0f6777')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', '04ef27c1-fc9e-44e3-83eb-e589fa0f6777', GETDATE());
    PRINT 'Aluno 1 matriculado em Matemática Discreta.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Enrollments WHERE StudentId = 'b2c3d4e5-f6g7-8901-bcde-f23456789012' AND ClassId = '04ef27c1-fc9e-44e3-83eb-e589fa0f6777')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    VALUES ('b2c3d4e5-f6g7-8901-bcde-f23456789012', '04ef27c1-fc9e-44e3-83eb-e589fa0f6777', GETDATE());
    PRINT 'Aluno 2 matriculado em Matemática Discreta.';
END
GO

-- Matricular alunos em outras turmas (usando subconsultas)
IF NOT EXISTS (SELECT 1 FROM Enrollments e 
               INNER JOIN Classes c ON e.ClassId = c.Id 
               WHERE e.StudentId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890' 
               AND c.Name = 'Desenvolvimento .NET')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    SELECT 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', Id, GETDATE()
    FROM Classes WHERE Name = 'Desenvolvimento .NET';
    PRINT 'Aluno 1 matriculado em Desenvolvimento .NET.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Enrollments e 
               INNER JOIN Classes c ON e.ClassId = c.Id 
               WHERE e.StudentId = 'c3d4e5f6-g7h8-9012-cdef-345678901234' 
               AND c.Name = 'Desenvolvimento .NET')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    SELECT 'c3d4e5f6-g7h8-9012-cdef-345678901234', Id, GETDATE()
    FROM Classes WHERE Name = 'Desenvolvimento .NET';
    PRINT 'Aluno 3 matriculado em Desenvolvimento .NET.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Enrollments e 
               INNER JOIN Classes c ON e.ClassId = c.Id 
               WHERE e.StudentId = 'b2c3d4e5-f6g7-8901-bcde-f23456789012' 
               AND c.Name = 'Arquitetura de Software')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    SELECT 'b2c3d4e5-f6g7-8901-bcde-f23456789012', Id, GETDATE()
    FROM Classes WHERE Name = 'Arquitetura de Software';
    PRINT 'Aluno 2 matriculado em Arquitetura de Software.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Enrollments e 
               INNER JOIN Classes c ON e.ClassId = c.Id 
               WHERE e.StudentId = 'c3d4e5f6-g7h8-9012-cdef-345678901234' 
               AND c.Name = 'Arquitetura de Software')
BEGIN
    INSERT INTO Enrollments (StudentId, ClassId, RegistrationDate)
    SELECT 'c3d4e5f6-g7h8-9012-cdef-345678901234', Id, GETDATE()
    FROM Classes WHERE Name = 'Arquitetura de Software';
    PRINT 'Aluno 3 matriculado em Arquitetura de Software.';
END
GO

PRINT '=============================================';
PRINT 'Inserção de dados concluída com sucesso!';
PRINT '=============================================';
PRINT '';
PRINT '📊 RESUMO DOS DADOS INSERIDOS:';
PRINT '👥 2 Usuários (admin, professor)';
PRINT '🎓 3 Students (alunos)';
PRINT '📚 3 Classes (Matemática, .NET, Arquitetura)';
PRINT '📝 6 Matrículas distribuídas entre alunos e turmas';
PRINT '';
PRINT '🔑 Credenciais de acesso:';
PRINT '   Email: admin@falunp.com.br';
PRINT '   Senha: Admin@123';
PRINT '   Email: aluno1@falunp.com.br';
PRINT '   Senha: Admin@123';
PRINT '';
PRINT '✅ Pronto para usar o sistema!';
GO