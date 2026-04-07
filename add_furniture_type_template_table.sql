-- Add FurnitureTypeTemplate table and update FurnitureTypes table
-- Run this manually in SQL Server Management Studio or Azure Data Studio

USE FurniFlowUzDb;
GO

-- Check if table exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FurnitureTypeTemplates')
BEGIN
    -- Create FurnitureTypeTemplates table
    CREATE TABLE [dbo].[FurnitureTypeTemplates] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [CategoryId] INT NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [DefaultMaterial] NVARCHAR(200) NULL,
        [DefaultNotes] NVARCHAR(2000) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2(7) NOT NULL,
        [UpdatedAt] DATETIME2(7) NULL,
        [CreatedBy] INT NULL,
        [UpdatedBy] INT NULL,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [DeletedAt] DATETIME2(7) NULL,
        [DeletedBy] INT NULL,
        CONSTRAINT [FK_FurnitureTypeTemplates_Categories_CategoryId]
            FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]) ON DELETE CASCADE
    );

    PRINT 'FurnitureTypeTemplates table created successfully';
END
ELSE
BEGIN
    PRINT 'FurnitureTypeTemplates table already exists';
END
GO

-- Check if TemplateId column exists in FurnitureTypes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'FurnitureTypes' AND COLUMN_NAME = 'TemplateId')
BEGIN
    -- Add TemplateId column to FurnitureTypes
    ALTER TABLE [dbo].[FurnitureTypes]
    ADD [TemplateId] INT NULL;

    -- Add foreign key constraint
    ALTER TABLE [dbo].[FurnitureTypes]
    ADD CONSTRAINT [FK_FurnitureTypes_FurnitureTypeTemplates_TemplateId]
        FOREIGN KEY ([TemplateId]) REFERENCES [FurnitureTypeTemplates]([Id]);

    PRINT 'TemplateId column added to FurnitureTypes table';
END
ELSE
BEGIN
    PRINT 'TemplateId column already exists in FurnitureTypes table';
END
GO

-- Check if Quantity column exists in FurnitureTypes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'FurnitureTypes' AND COLUMN_NAME = 'Quantity')
BEGIN
    -- Add Quantity column to FurnitureTypes
    ALTER TABLE [dbo].[FurnitureTypes]
    ADD [Quantity] INT NOT NULL DEFAULT 1;

    PRINT 'Quantity column added to FurnitureTypes table';
END
ELSE
BEGIN
    PRINT 'Quantity column already exists in FurnitureTypes table';
END
GO

-- Insert some sample templates for testing
IF NOT EXISTS (SELECT * FROM [FurnitureTypeTemplates])
BEGIN
    INSERT INTO [FurnitureTypeTemplates]
        ([Name], [CategoryId], [Description], [DefaultMaterial], [DefaultNotes], [IsActive], [DisplayOrder], [CreatedAt], [IsDeleted])
    VALUES
        ('2 eshikli shkaf', 2, 'Ikki eshikli standart shkaf', 'LDSP 18mm', 'Standart ilgichlar va tutqichlar', 1, 1, GETUTCDATE(), 0),
        ('3 eshikli shkaf', 2, 'Uch eshikli katta shkaf', 'LDSP 18mm', 'Kuchli ilgichlar va metall tutqichlar', 1, 2, GETUTCDATE(), 0),
        ('4 eshikli shkaf', 2, 'To''rt eshikli juda katta shkaf', 'LDSP 18mm', 'Kuchli ilgichlar, metall tutqichlar va bo''linma', 1, 3, GETUTCDATE(), 0),
        ('Stol ustma ustki', 1, 'Oshxona uchun ustma shkaf', 'LDSP 16mm', 'Yengil ilgichlar', 1, 1, GETUTCDATE(), 0),
        ('Stol ostki', 1, 'Oshxona uchun ostki shkaf', 'LDSP 18mm', 'Kuchli oyoqlar', 1, 2, GETUTCDATE(), 0);

    PRINT 'Sample templates inserted successfully';
END
ELSE
BEGIN
    PRINT 'Sample templates already exist';
END
GO

PRINT 'Migration completed successfully!';
GO
