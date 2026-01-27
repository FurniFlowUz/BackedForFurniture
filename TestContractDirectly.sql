-- Test Contract Creation Directly in Database
-- This tests that the new schema can store data correctly

USE FurniFlowUzDb;
GO

-- Insert a test contract with the NEW schema
INSERT INTO Contracts (
    ContractNumber,
    CustomerId,
    CategoryIds,  -- NEW: comma-separated list
    TotalAmount,
    AdvancePaymentAmount,  -- NEW: exact amount
    RemainingAmount,
    ProductionDurationDays,  -- NEW: days instead of deadline
    SignedDate,
    DeliveryTerms,  -- NEW: split from Terms
    PenaltyTerms,  -- NEW: split from Terms
    AdditionalNotes,  -- NEW: combined Notes/Description
    RequiresApproval,  -- NEW
    PaymentStatus,
    Status,
    CreatedAt,
    IsDeleted
)
VALUES (
    'SH-2026-TEST1',
    1,  -- First customer
    '1,2',  -- Multiple categories
    10000.00,
    3000.00,  -- 30% advance
    7000.00,
    45,  -- 45 days production
    GETDATE(),
    'Delivery within 45 days of production completion',
    'Late delivery penalty: 1% per day',
    'Test contract for backward compatibility validation',
    1,  -- Requires approval
    0,  -- Pending
    0,  -- New
    GETDATE(),
    0  -- Not deleted
);
GO

-- Verify the contract was created
SELECT
    ContractNumber,
    CustomerId,
    CategoryIds,
    TotalAmount,
    AdvancePaymentAmount,
    ProductionDurationDays,
    DeliveryTerms,
    PenaltyTerms,
    AdditionalNotes
FROM Contracts
WHERE ContractNumber = 'SH-2026-TEST1';
GO

PRINT 'Test contract created successfully!';
