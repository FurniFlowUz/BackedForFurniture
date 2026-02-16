-- Fix for Entity Framework Core migrations history table
-- If the Categories table exists (indicating the database was created manually or via EnsureCreated),
-- but the __EFMigrationsHistory table is missing or doesn't have the initial migration,
-- this script will create the history table and insert the initial migration record.

-- 1. Create __EFMigrationsHistory if it doesn't exist
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
    BEGIN
        CREATE TABLE [__EFMigrationsHistory] (
            [MigrationId] nvarchar(150) NOT NULL,
            [ProductVersion] nvarchar(32) NOT NULL,
            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
        );
    END
END

-- 2. Insert InitialCreate migration if not present
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260216131707_InitialCreate')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20260216131707_InitialCreate', '8.0.0');
    END
END
