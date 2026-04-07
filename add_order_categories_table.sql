-- Create OrderCategories table for Many-to-Many relationship between Order and Category
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderCategories' AND xtype='U')
BEGIN
    CREATE TABLE [OrderCategories] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [OrderId] INT NOT NULL,
        [CategoryId] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_OrderCategories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderCategories_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]) ON DELETE NO ACTION
    );

    -- Create indexes
    CREATE INDEX [IX_OrderCategories_OrderId] ON [OrderCategories]([OrderId]);
    CREATE INDEX [IX_OrderCategories_CategoryId] ON [OrderCategories]([CategoryId]);
    CREATE UNIQUE INDEX [IX_OrderCategories_OrderId_CategoryId] ON [OrderCategories]([OrderId], [CategoryId]);

    PRINT 'OrderCategories table created successfully';
END
ELSE
BEGIN
    PRINT 'OrderCategories table already exists';
END
GO

-- Migrate existing data: Copy categories from Contract to OrderCategories for existing orders
INSERT INTO [OrderCategories] ([OrderId], [CategoryId], [CreatedAt])
SELECT
    o.[Id] AS OrderId,
    CAST(value AS INT) AS CategoryId,
    GETUTCDATE() AS CreatedAt
FROM [Orders] o
INNER JOIN [Contracts] c ON o.[ContractId] = c.[Id]
CROSS APPLY STRING_SPLIT(c.[CategoryIds], ',')
WHERE c.[CategoryIds] IS NOT NULL
  AND c.[CategoryIds] != ''
  AND NOT EXISTS (
      SELECT 1 FROM [OrderCategories] oc
      WHERE oc.[OrderId] = o.[Id] AND oc.[CategoryId] = CAST(value AS INT)
  );

PRINT 'Existing order categories migrated successfully';
GO
