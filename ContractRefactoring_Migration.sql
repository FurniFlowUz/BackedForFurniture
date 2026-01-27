-- Contract Refactoring Migration Script
-- This script updates the Contracts table schema to support the new multi-step wizard

USE FurniFlowUzDb;
GO

-- Step 1: Add new columns
ALTER TABLE Contracts
ADD CategoryIds NVARCHAR(500) NOT NULL DEFAULT '',
    ProductionDurationDays INT NOT NULL DEFAULT 30,
    DeliveryTerms NVARCHAR(2000) NULL,
    PenaltyTerms NVARCHAR(2000) NULL,
    AdditionalNotes NVARCHAR(2000) NULL,
    RequiresApproval BIT NOT NULL DEFAULT 1;
GO

-- Step 2: Migrate existing data from old columns to new columns
-- Check if old columns exist before migrating
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'CategoryId')
BEGIN
    UPDATE Contracts
    SET CategoryIds = CAST(CategoryId AS NVARCHAR(50));
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Deadline')
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'SignedDate')
BEGIN
    UPDATE Contracts
    SET ProductionDurationDays = CASE
            WHEN Deadline IS NOT NULL AND SignedDate IS NOT NULL
            THEN DATEDIFF(DAY, SignedDate, Deadline)
            ELSE 30
        END;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Terms')
BEGIN
    UPDATE Contracts SET DeliveryTerms = Terms WHERE Terms IS NOT NULL;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Notes')
   OR EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Description')
BEGIN
    UPDATE Contracts
    SET AdditionalNotes = COALESCE(Notes, Description);
END
GO

-- Step 3: Update AdvancePaymentAmount from percentage if needed
UPDATE Contracts
SET AdvancePaymentAmount = TotalAmount * (AdvancePaymentPercentage / 100.0)
WHERE AdvancePaymentAmount = 0 OR AdvancePaymentAmount IS NULL;
GO

-- Step 4: Make AdvancePaymentAmount required
ALTER TABLE Contracts
ALTER COLUMN AdvancePaymentAmount DECIMAL(18,2) NOT NULL;
GO

-- Step 5: Drop old columns (after data migration)
-- Note: Comment these out if you want to keep old data for rollback

-- Drop foreign keys and indexes before dropping columns
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Contracts_Categories_CategoryId')
BEGIN
    ALTER TABLE Contracts DROP CONSTRAINT FK_Contracts_Categories_CategoryId;
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_CategoryId' AND object_id = OBJECT_ID('Contracts'))
BEGIN
    DROP INDEX IX_Contracts_CategoryId ON Contracts;
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_Deadline' AND object_id = OBJECT_ID('Contracts'))
BEGIN
    DROP INDEX IX_Contracts_Deadline ON Contracts;
END
GO

-- Now drop the columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'CategoryId')
BEGIN
    ALTER TABLE Contracts DROP COLUMN CategoryId;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'AdvancePaymentPercentage')
BEGIN
    ALTER TABLE Contracts DROP COLUMN AdvancePaymentPercentage;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Deadline')
BEGIN
    ALTER TABLE Contracts DROP COLUMN Deadline;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Description')
BEGIN
    ALTER TABLE Contracts DROP COLUMN Description;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Notes')
BEGIN
    ALTER TABLE Contracts DROP COLUMN Notes;
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Terms')
BEGIN
    ALTER TABLE Contracts DROP COLUMN Terms;
END
GO

-- Step 6: Update contract numbers to new format (SH-YYYY-XXXX)
-- This will only update future contracts, existing ones keep their numbers
-- The backend GenerateContractNumberAsync will use the new format

PRINT 'Migration completed successfully!';
GO
