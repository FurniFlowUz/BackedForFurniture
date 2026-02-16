-- Production Stages qo'shish/yangilash script
-- Mavjud stage'larni tekshirish va yangilarini qo'shish

-- Avval mavjud stage'larni ko'rish
SELECT * FROM ProductionStages ORDER BY SequenceOrder;

-- Agar stage'lar kam bo'lsa, quyidagilarni qo'shing:
-- DIQQAT: Mavjud Id'larni tekshiring va keyin qo'shing

-- Razmer (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Razmer')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Razmer', 0, 1, 2.0, N'O''lchov va kesish ishlari', 1, 0, GETUTCDATE());
END

-- Arra (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Arra')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Arra', 0, 2, 3.0, N'Arra bilan kesish', 1, 0, GETUTCDATE());
END

-- Shipon (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Shipon')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Shipon', 1, 3, 2.0, N'Shipon bilan ishlash', 1, 0, GETUTCDATE());
END

-- Pres (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Pres')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Pres', 1, 4, 2.0, N'Pres ishlari', 1, 0, GETUTCDATE());
END

-- Rover (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Rover')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Rover', 1, 5, 3.0, N'Rover stanogida ishlash', 1, 0, GETUTCDATE());
END

-- Kromka (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Kromka')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Kromka', 2, 6, 2.0, N'Qirralarni yopish', 1, 0, GETUTCDATE());
END

-- Shkurka (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Shkurka')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Shkurka', 3, 7, 3.0, N'Silliqlash ishlari', 1, 0, GETUTCDATE());
END

-- Pardozchi (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Pardozchi')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Pardozchi', 5, 8, 2.0, N'Pardozlash tayyorlash', 1, 0, GETUTCDATE());
END

-- Grunt (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Grunt')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Grunt', 5, 9, 2.0, N'Grunt qoplash', 1, 0, GETUTCDATE());
END

-- Grunt shkurka (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Grunt shkurka')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Grunt shkurka', 5, 10, 2.0, N'Grunt silliqlash', 1, 0, GETUTCDATE());
END

-- Kraska (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Kraska')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Kraska', 6, 11, 3.0, N'Bo''yash', 1, 0, GETUTCDATE());
END

-- Qurutish (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Qurutish')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Qurutish', 6, 12, 4.0, N'Quritish', 1, 0, GETUTCDATE());
END

-- OTK (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'OTK')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('OTK', 7, 13, 1.0, N'Sifat nazorati', 1, 0, GETUTCDATE());
END

-- Qadoqlash (agar yo'q bo'lsa)
IF NOT EXISTS (SELECT 1 FROM ProductionStages WHERE Name = 'Qadoqlash')
BEGIN
    INSERT INTO ProductionStages (Name, StageType, SequenceOrder, EstimatedDurationHours, Description, IsActive, IsDeleted, CreatedAt)
    VALUES ('Qadoqlash', 4, 14, 2.0, N'Qadoqlash ishlari', 1, 0, GETUTCDATE());
END

-- Natijani tekshirish
SELECT * FROM ProductionStages WHERE IsDeleted = 0 ORDER BY SequenceOrder;

-- Agar "Krom" nomli stage bor bo'lsa, uni "Kromka" ga o'zgartiring
UPDATE ProductionStages SET Name = 'Kromka' WHERE Name = 'Krom';

PRINT 'Production stages successfully added/updated!';
