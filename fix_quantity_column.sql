-- Add Quantity column to FurnitureTypes table
USE FurniFlowUzDb;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Check if Quantity column exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'FurnitureTypes' AND COLUMN_NAME = 'Quantity')
BEGIN
    -- Add Quantity column
    ALTER TABLE [dbo].[FurnitureTypes]
    ADD [Quantity] INT NOT NULL CONSTRAINT DF_FurnitureTypes_Quantity DEFAULT 1;

    PRINT 'Quantity column added successfully to FurnitureTypes table';
END
ELSE
BEGIN
    PRINT 'Quantity column already exists in FurnitureTypes table';
END
GO
