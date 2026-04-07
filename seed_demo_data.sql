-- ============================================================
-- FURNIFLOW DEMO DATA SEED SCRIPT
-- Seeds complete workflow data for client presentation
-- ============================================================

BEGIN;

-- Reset sequences to correct values based on existing data
SELECT setval('"Users_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Users"), 1));
SELECT setval('"Departments_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Departments"), 1), false);
SELECT setval('"Positions_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Positions"), 1), false);
SELECT setval('"Employees_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Employees"), 1), false);
SELECT setval('"Customers_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Customers"), 1));
SELECT setval('"Contracts_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Contracts"), 1));
SELECT setval('"Orders_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Orders"), 1));
SELECT setval('"FurnitureTypes_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "FurnitureTypes"), 1));
SELECT setval('"Details_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Details"), 1));
SELECT setval('"Teams_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Teams"), 1));
SELECT setval('"TechnicalSpecifications_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "TechnicalSpecifications"), 1));
SELECT setval('"CategoryAssignments_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "CategoryAssignments"), 1));
SELECT setval('"DetailTasks_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "DetailTasks"), 1));
SELECT setval('"WarehouseTransactions_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "WarehouseTransactions"), 1), false);
SELECT setval('"Notifications_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "Notifications"), 1));
SELECT setval('"OrderCategories_Id_seq"', GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "OrderCategories"), 1));

-- ============================================================
-- 1. ADD MORE USERS (Workers, Team Leaders)
-- ============================================================
-- Password hash for "Demo123!" (reusing existing hash)
INSERT INTO "Users" ("FirstName", "LastName", "Email", "PhoneNumber", "PasswordHash", "Role", "IsActive", "CreatedAt", "IsDeleted")
VALUES
  ('Sardor', 'Aliyev', 'sardor@furniflowauz.com', '+998901234580', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 6, true, NOW(), false),
  ('Bobur', 'Karimov', 'bobur@furniflowauz.com', '+998901234581', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 6, true, NOW(), false),
  ('Anvar', 'Toshmatov', 'anvar@furniflowauz.com', '+998901234582', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 6, true, NOW(), false),
  ('Jasur', 'Rahimov', 'jasur@furniflowauz.com', '+998901234583', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 6, true, NOW(), false),
  ('Dilshod', 'Umarov', 'dilshod@furniflowauz.com', '+998901234584', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 5, true, NOW(), false),
  ('Sherzod', 'Nazarov', 'sherzod@furniflowauz.com', '+998901234585', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 6, true, NOW(), false),
  ('Otabek', 'Sobirov', 'otabek@furniflowauz.com', '+998901234586', '$2a$11$kX6G1TxCkI0yiGpFQAQwBeFjmOgPS00UTtT6dHyR6Aog5k2..UEJm', 3, true, NOW(), false);

-- ============================================================
-- 2. DEPARTMENTS
-- ============================================================
INSERT INTO "Departments" ("Name", "Description", "CreatedAt", "IsDeleted")
VALUES
  ('Boshqaruv', 'Korxona boshqaruvi va rahbariyat', NOW(), false),
  ('Savdo', 'Savdo va mijozlar bilan ishlash bo''limi', NOW(), false),
  ('Konstruktorlik', 'Mebel loyihalash va dizayn bo''limi', NOW(), false),
  ('Ishlab chiqarish', 'Mebel ishlab chiqarish bo''limi', NOW(), false),
  ('Ombor', 'Materiallar va zaxiralarni boshqarish', NOW(), false),
  ('Sifat nazorati', 'Sifat tekshiruvi bo''limi', NOW(), false);

-- ============================================================
-- 3. POSITIONS
-- ============================================================
INSERT INTO "Positions" ("Name", "Description", "CreatedAt", "IsDeleted")
VALUES
  ('Direktor', 'Korxona direktori', NOW(), false),
  ('Savdo menejeri', 'Savdo bo''limi menejeri', NOW(), false),
  ('Bosh konstruktor', 'Bosh mebel konstruktori', NOW(), false),
  ('Konstruktor', 'Mebel konstruktori', NOW(), false),
  ('Ishlab chiqarish boshlig''i', 'Ishlab chiqarish bo''limi boshlig''i', NOW(), false),
  ('Jamoa rahbari', 'Ishlab chiqarish jamoa rahbari', NOW(), false),
  ('Usta', 'Mebel ustasi', NOW(), false),
  ('Yordamchi usta', 'Yordamchi mebel ustasi', NOW(), false),
  ('Ombor menejeri', 'Ombor boshqaruvchisi', NOW(), false);

-- ============================================================
-- 4. EMPLOYEES (linked to Users)
-- ============================================================
INSERT INTO "Employees" ("UserId", "FullName", "Phone", "PositionId", "DepartmentId", "IsActive", "ActiveTasks", "CompletedTasks", "OnTimePercent", "CreatedAt", "IsDeleted")
VALUES
  (1, 'Admin User', '+998901234567', 1, 1, true, 0, 0, 100.00, NOW(), false),
  (2, 'Sales Person', '+998901234568', 2, 2, true, 0, 0, 95.00, NOW(), false),
  (3, 'Constructor User', '+998901234569', 3, 3, true, 2, 5, 92.00, NOW(), false),
  (4, 'Production Manager', '+998901234570', 5, 4, true, 1, 3, 98.00, NOW(), false),
  (5, 'Warehouse Manager', '+998901234571', 9, 5, true, 0, 0, 100.00, NOW(), false),
  (6, 'Team Leader', '+998901234572', 6, 4, true, 3, 8, 88.00, NOW(), false),
  (7, 'Worker User', '+998901234573', 7, 4, true, 1, 12, 85.00, NOW(), false),
  (8, 'Sardor Aliyev', '+998901234580', 7, 4, true, 2, 15, 90.00, NOW(), false),
  (9, 'Bobur Karimov', '+998901234581', 7, 4, true, 1, 10, 87.00, NOW(), false),
  (10, 'Anvar Toshmatov', '+998901234582', 8, 4, true, 0, 8, 82.00, NOW(), false),
  (11, 'Jasur Rahimov', '+998901234583', 8, 4, true, 1, 6, 91.00, NOW(), false),
  (12, 'Dilshod Umarov', '+998901234584', 6, 4, true, 2, 11, 93.00, NOW(), false),
  (13, 'Sherzod Nazarov', '+998901234585', 7, 4, true, 1, 9, 86.00, NOW(), false),
  (14, 'Otabek Sobirov', '+998901234586', 4, 3, true, 1, 4, 94.00, NOW(), false);

-- ============================================================
-- 5. MORE CUSTOMERS
-- ============================================================
INSERT INTO "Customers" ("FullName", "PhoneNumber", "Email", "Address", "Notes", "CreatedAt", "IsDeleted")
VALUES
  ('Aziz Yusupov', '+998904444444', 'aziz@example.com', 'Tashkent, Sergeli tumani, Yangi Sergeli 15', 'Yangi uy uchun oshxona mebeli kerak', NOW(), false),
  ('Kamola Abdullayeva', '+998905555555', 'kamola@example.com', 'Samarqand, Registon ko''chasi 25', 'Ofis mebellari loyihasi', NOW(), false);

-- ============================================================
-- 6. MORE CONTRACTS
-- ============================================================
INSERT INTO "Contracts" ("ContractNumber", "CustomerId", "CategoryIds", "TotalAmount", "AdvancePaymentAmount", "RemainingAmount", "PaymentStatus", "Status", "DeadlineDate", "DeliveryTerms", "PenaltyTerms", "RequiresApproval", "CreatedAt", "IsDeleted", "CreatedBy")
VALUES
  ('SH-2026-0002', 1, '2', 8500000, 3400000, 5100000, 1, 1, NOW() + INTERVAL '45 days', 'Bepul yetkazib berish', 'Kunlik 0.1%', false, NOW() - INTERVAL '10 days', false, 2),
  ('SH-2026-0003', 2, '3', 12000000, 6000000, 6000000, 1, 1, NOW() + INTERVAL '60 days', 'Bepul yetkazib berish', 'Kunlik 0.5%', false, NOW() - INTERVAL '5 days', false, 2),
  ('SH-2026-0004', 5, '1,4', 15000000, 7500000, 7500000, 0, 1, NOW() + INTERVAL '30 days', 'Pullik yetkazib berish', 'Kunlik 0.1%', true, NOW() - INTERVAL '2 days', false, 2),
  ('SH-2026-0005', 6, '2,3', 20000000, 10000000, 10000000, 1, 1, NOW() + INTERVAL '90 days', 'Bepul yetkazib berish', 'Jarima yo''q', false, NOW() - INTERVAL '1 day', false, 2);

-- ============================================================
-- 7. MORE ORDERS
-- ============================================================
INSERT INTO "Orders" ("OrderNumber", "CustomerId", "CategoryId", "ContractId", "DeadlineDate", "Status", "ProgressPercentage", "AssignedConstructorId", "AssignedProductionManagerId", "Notes", "CreatedAt", "IsDeleted", "CreatedBy")
VALUES
  ('ORD-20260407-0002', 1, 2, 2, NOW() + INTERVAL '45 days', 3, 40.00, 3, 4, 'Shkaf-kupe buyurtmasi - Alisher', NOW() - INTERVAL '10 days', false, 2),
  ('ORD-20260407-0003', 2, 3, 3, NOW() + INTERVAL '60 days', 2, 10.00, 14, 4, 'Yotoqxona to''plami - Nodira', NOW() - INTERVAL '5 days', false, 2),
  ('ORD-20260407-0004', 5, 1, 4, NOW() + INTERVAL '30 days', 4, 75.00, 3, 4, 'Oshxona mebeli - Aziz', NOW() - INTERVAL '15 days', false, 2),
  ('ORD-20260407-0005', 5, 4, 4, NOW() + INTERVAL '30 days', 1, 0.00, NULL, NULL, 'Ofis mebeli - Aziz', NOW() - INTERVAL '2 days', false, 2),
  ('ORD-20260407-0006', 6, 2, 5, NOW() + INTERVAL '90 days', 2, 5.00, 3, 4, 'Shkaf-kupe - Kamola', NOW() - INTERVAL '1 day', false, 2);

-- ============================================================
-- 8. MORE FURNITURE TYPES
-- ============================================================
INSERT INTO "FurnitureTypes" ("Name", "OrderId", "Quantity", "ProgressPercentage", "Notes", "CreatedAt", "IsDeleted")
VALUES
  ('Shkaf-kupe 2 eshikli', 2, 1, 60.00, '2 eshikli oynali shkaf-kupe', NOW() - INTERVAL '9 days', false),
  ('Shkaf-kupe 3 eshikli', 2, 1, 20.00, '3 eshikli klassik shkaf', NOW() - INTERVAL '9 days', false),
  ('Karavot', 3, 1, 10.00, 'Ikki kishilik karavot', NOW() - INTERVAL '4 days', false),
  ('Tumbochka', 3, 2, 0.00, 'Yotoq tumbochkasi', NOW() - INTERVAL '4 days', false),
  ('Oshxona garnituri', 4, 1, 90.00, 'Burchak oshxona garnituri', NOW() - INTERVAL '14 days', false),
  ('Oshxona stol', 4, 1, 60.00, 'Ovqatlanish stoli', NOW() - INTERVAL '14 days', false),
  ('Yozuv stoli', 5, 2, 0.00, 'Ofis yozuv stoli', NOW() - INTERVAL '1 day', false),
  ('Shkaf-kupe kompakt', 6, 1, 5.00, 'Kompakt shkaf', NOW() - INTERVAL '1 day', false);

-- ============================================================
-- 9. MORE DETAILS (Parts for each furniture type)
-- ============================================================
-- Shkaf-kupe 2 eshikli (FT 2)
INSERT INTO "Details" ("Name", "Width", "Height", "Thickness", "Quantity", "FurnitureTypeId", "Material", "Notes", "CreatedAt", "IsDeleted")
VALUES
  ('Yon devor', 60.00, 240.00, 18.00, 2, 2, 'ЛДСП Белый', 'Chap va o''ng yon panellar', NOW() - INTERVAL '8 days', false),
  ('Eshik (oynali)', 90.00, 230.00, 8.00, 2, 2, 'Oyna + alyuminiy rom', 'Silliq oyna', NOW() - INTERVAL '8 days', false),
  ('Polka', 88.00, 1.80, 18.00, 5, 2, 'ЛДСП Белый', 'Ichki polkalar', NOW() - INTERVAL '8 days', false),
  ('Taglik', 90.00, 60.00, 18.00, 1, 2, 'ЛДСП Белый', 'Pastki taglik', NOW() - INTERVAL '8 days', false),
  -- Shkaf-kupe 3 eshikli (FT 3)
  ('Yon devor', 60.00, 240.00, 18.00, 2, 3, 'ЛДСП Yong''oq', 'Chap va o''ng', NOW() - INTERVAL '8 days', false),
  ('Eshik', 60.00, 230.00, 18.00, 3, 3, 'ЛДСП Yong''oq', '3 ta eshik', NOW() - INTERVAL '8 days', false),
  -- Karavot (FT 4)
  ('Bosh qism', 160.00, 100.00, 30.00, 1, 4, 'Massiv daraxti', 'Bosh taxtasi', NOW() - INTERVAL '3 days', false),
  ('Oyoqlar', 10.00, 35.00, 10.00, 4, 4, 'Massiv daraxti', 'Karavot oyoqlari', NOW() - INTERVAL '3 days', false),
  -- Oshxona garnituri (FT 6)
  ('Yuqori shkaf', 80.00, 70.00, 30.00, 3, 6, 'МДФ Белый глянец', 'Oshxona yuqori shkaflari', NOW() - INTERVAL '13 days', false),
  ('Pastki shkaf', 80.00, 85.00, 60.00, 4, 6, 'МДФ Белый глянец', 'Oshxona pastki shkaflari', NOW() - INTERVAL '13 days', false),
  ('Stoleshnitsa', 300.00, 3.00, 60.00, 1, 6, 'Kvarts tosh', 'Ish yuzasi', NOW() - INTERVAL '13 days', false),
  -- Oshxona stol (FT 7)
  ('Stol yuzasi', 120.00, 3.00, 80.00, 1, 7, 'МДФ Белый', 'Stol ustki qismi', NOW() - INTERVAL '13 days', false),
  ('Oyoq', 8.00, 75.00, 8.00, 4, 7, 'Metall xrom', 'Metall oyoqlar', NOW() - INTERVAL '13 days', false);

-- ============================================================
-- 10. TECHNICAL SPECIFICATIONS
-- ============================================================
INSERT INTO "TechnicalSpecifications" ("FurnitureTypeId", "Notes", "IsLocked", "CompletedAt", "CreatedAt", "IsDeleted")
VALUES
  (2, 'ЛДСП 18mm oq, Blum furnitura, oynali eshiklar', true, NOW() - INTERVAL '7 days', NOW() - INTERVAL '8 days', false),
  (3, 'ЛДСП 18mm yong''oq rang, standart furnitura', true, NOW() - INTERVAL '7 days', NOW() - INTERVAL '8 days', false),
  (4, 'Massiv yong''oq daraxti, lak qoplama', false, NULL, NOW() - INTERVAL '3 days', false),
  (6, 'МДФ oq glyanets, kvarts stol, Blum furnitura', true, NOW() - INTERVAL '12 days', NOW() - INTERVAL '13 days', false),
  (7, 'МДФ oq, xrom oyoqlar', true, NOW() - INTERVAL '12 days', NOW() - INTERVAL '13 days', false);

-- ============================================================
-- 11. MORE TEAMS
-- ============================================================
INSERT INTO "Teams" ("Name", "TeamLeaderId", "IsActive", "Description", "CreatedAt", "IsDeleted")
VALUES
  ('Bo''yash Jamoasi', 12, true, 'Bo''yash va pardozlash ishlari', NOW() - INTERVAL '5 days', false),
  ('Yig''ish Jamoasi', 6, true, 'Mebel yig''ish va montaj ishlari', NOW() - INTERVAL '5 days', false);

-- Add team members (junction table)
INSERT INTO "TeamMember" ("UserId", "TeamId")
VALUES
  -- Arra Jamoasi (Team 1): Worker(7), Sardor(8), Bobur(9)
  (7, 1), (8, 1), (9, 1),
  -- Bo'yash Jamoasi (Team 2): Anvar(10), Jasur(11)
  (10, 2), (11, 2),
  -- Yig'ish Jamoasi (Team 3): Sherzod(13), Worker(7)
  (13, 3), (7, 3);

-- ============================================================
-- 12. MORE CATEGORY ASSIGNMENTS
-- ============================================================
INSERT INTO "CategoryAssignments" ("OrderId", "FurnitureTypeId", "TeamLeaderId", "TeamId", "Status", "AssignedAt", "StartedAt", "CompletedAt", "Notes", "CreatedAt", "IsDeleted")
VALUES
  -- Order 2: Shkaf-kupe 2 eshikli -> Arra Jamoasi (InProgress)
  (2, 2, 6, 1, 1, NOW() - INTERVAL '7 days', NOW() - INTERVAL '6 days', NULL, 'Shkaf-kupe 2 eshikli ishlab chiqarish', NOW() - INTERVAL '7 days', false),
  -- Order 2: Shkaf-kupe 3 eshikli -> Arra Jamoasi (Assigned)
  (2, 3, 6, 1, 0, NOW() - INTERVAL '5 days', NULL, NULL, 'Shkaf-kupe 3 eshikli - kutilmoqda', NOW() - INTERVAL '5 days', false),
  -- Order 4: Oshxona garnituri -> Bo'yash Jamoasi (InProgress, almost done)
  (4, 6, 12, 2, 1, NOW() - INTERVAL '12 days', NOW() - INTERVAL '11 days', NULL, 'Oshxona garnituri bo''yash va pardoz', NOW() - INTERVAL '12 days', false),
  -- Order 4: Oshxona stol -> Yig'ish Jamoasi (InProgress)
  (4, 7, 6, 3, 1, NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days', NULL, 'Stol yig''ish ishlari', NOW() - INTERVAL '10 days', false);

-- ============================================================
-- 13. MORE DETAIL TASKS (Various statuses for demo)
-- ============================================================
-- Assignment 2 (Shkaf-kupe 2 eshikli, Team 1)
INSERT INTO "DetailTasks" ("CategoryAssignmentId", "DetailId", "AssignedEmployeeId", "Sequence", "TaskDescription", "Status", "EstimatedDuration", "StartedAt", "CompletedAt", "Notes", "CreatedAt", "IsDeleted")
VALUES
  (2, 2, 8, 1, 'Yon devorlarni kesish (2 ta)', 4, '02:00:00', NOW() - INTERVAL '5 days', NOW() - INTERVAL '5 days' + INTERVAL '1 hour 45 min', 'Sifatli kesildi', NOW() - INTERVAL '6 days', false),
  (2, 3, 8, 2, 'Oynali eshiklarni tayyorlash', 2, '03:00:00', NOW() - INTERVAL '4 days', NULL, 'Jarayonda', NOW() - INTERVAL '5 days', false),
  (2, 4, 9, 3, 'Polkalarni kesish (5 ta)', 1, '01:30:00', NULL, NULL, NULL, NOW() - INTERVAL '5 days', false),
  (2, 5, 9, 4, 'Taglikni tayyorlash', 0, '00:45:00', NULL, NULL, NULL, NOW() - INTERVAL '5 days', false),
  -- Assignment 4 (Oshxona garnituri, Team 2)
  (4, 10, 10, 1, 'Yuqori shkaflarni bo''yash (3 ta)', 4, '04:00:00', NOW() - INTERVAL '8 days', NOW() - INTERVAL '7 days', 'Oq glyanets bo''yalgan', NOW() - INTERVAL '9 days', false),
  (4, 11, 11, 2, 'Pastki shkaflarni bo''yash (4 ta)', 4, '05:00:00', NOW() - INTERVAL '7 days', NOW() - INTERVAL '6 days', 'Oq glyanets bo''yalgan', NOW() - INTERVAL '8 days', false),
  (4, 12, 10, 3, 'Stoleshnitsa o''rnatish', 2, '02:00:00', NOW() - INTERVAL '3 days', NULL, 'Jarayonda', NOW() - INTERVAL '5 days', false),
  -- Assignment 5 (Oshxona stol, Team 3)
  (5, 13, 13, 1, 'Stol yuzasini tayyorlash', 4, '02:00:00', NOW() - INTERVAL '7 days', NOW() - INTERVAL '6 days', 'Tayyor', NOW() - INTERVAL '8 days', false),
  (5, 14, 13, 2, 'Metall oyoqlarni o''rnatish', 2, '01:00:00', NOW() - INTERVAL '2 days', NULL, 'Jarayonda', NOW() - INTERVAL '5 days', false);

-- ============================================================
-- 14. WAREHOUSE TRANSACTIONS (demo income/outcome)
-- ============================================================
INSERT INTO "WarehouseTransactions" ("Type", "WarehouseItemId", "Quantity", "TeamId", "CreatedByUserId", "TransactionDate", "Notes", "CreatedAt", "IsDeleted")
VALUES
  -- Income transactions
  (0, 1, 50.00, NULL, 5, NOW() - INTERVAL '20 days', 'Chipboard oq - yangi partiya', NOW() - INTERVAL '20 days', false),
  (0, 2, 200.00, NULL, 5, NOW() - INTERVAL '15 days', 'PVC krom lenta yetkazib berildi', NOW() - INTERVAL '15 days', false),
  (0, 5, 20.00, NULL, 5, NOW() - INTERVAL '10 days', 'Yoqim yangi partiya', NOW() - INTERVAL '10 days', false),
  -- Outcome transactions (materials to teams)
  (1, 1, 15.00, 1, 5, NOW() - INTERVAL '7 days', 'Arra jamoasiga chipboard', NOW() - INTERVAL '7 days', false),
  (1, 2, 50.00, 1, 5, NOW() - INTERVAL '7 days', 'Arra jamoasiga krom lenta', NOW() - INTERVAL '7 days', false),
  (1, 3, 20.00, 3, 5, NOW() - INTERVAL '5 days', 'Yig''ish jamoasiga petlalar', NOW() - INTERVAL '5 days', false),
  (1, 4, 10.00, 3, 5, NOW() - INTERVAL '5 days', 'Yig''ish jamoasiga tutqichlar', NOW() - INTERVAL '5 days', false),
  (1, 5, 5.00, 2, 5, NOW() - INTERVAL '3 days', 'Bo''yash jamoasiga yelim', NOW() - INTERVAL '3 days', false);

-- Update warehouse stock based on transactions
UPDATE "WarehouseItems" SET "CurrentStock" = "CurrentStock" - 15 WHERE "Id" = 1;
UPDATE "WarehouseItems" SET "CurrentStock" = "CurrentStock" - 50 WHERE "Id" = 2;
UPDATE "WarehouseItems" SET "CurrentStock" = "CurrentStock" - 20 WHERE "Id" = 3;
UPDATE "WarehouseItems" SET "CurrentStock" = "CurrentStock" - 10 WHERE "Id" = 4;
UPDATE "WarehouseItems" SET "CurrentStock" = "CurrentStock" - 5 WHERE "Id" = 5;

-- ============================================================
-- 15. NOTIFICATIONS (realistic notifications for demo)
-- ============================================================
INSERT INTO "Notifications" ("Type", "Title", "Message", "UserId", "IsRead", "RelatedEntityType", "RelatedEntityId", "CreatedAt", "IsDeleted")
VALUES
  -- Director notifications
  (6, 'Yangi shartnoma', 'SH-2026-0004 shartnomasi tasdiqlashni kutmoqda', 1, false, 'Contract', 4, NOW() - INTERVAL '2 days', false),
  (1, 'Muddat yaqinlashmoqda', 'ORD-20260407-0004 buyurtma muddati 5 kunga qoldi', 1, false, 'Order', 4, NOW() - INTERVAL '1 day', false),
  -- PM notifications
  (8, 'Yangi buyurtma', 'ORD-20260407-0006 buyurtma tayinlandi', 4, false, 'Order', 6, NOW() - INTERVAL '1 day', false),
  (4, 'Vazifa yakunlandi', 'Yon devorlarni kesish vazifasi yakunlandi', 4, true, 'DetailTask', 2, NOW() - INTERVAL '5 days', false),
  -- Team Leader notifications
  (4, 'Yangi tayinlov', 'Shkaf-kupe 3 eshikli sizga tayinlandi', 6, false, 'CategoryAssignment', 3, NOW() - INTERVAL '5 days', false),
  (4, 'Vazifa yakunlandi', 'Stol yuzasini tayyorlash - bajarildi', 6, true, 'DetailTask', 9, NOW() - INTERVAL '6 days', false),
  -- Constructor notifications
  (8, 'Yangi buyurtma', 'ORD-20260407-0003 buyurtma sizga tayinlandi', 14, false, 'Order', 3, NOW() - INTERVAL '5 days', false),
  -- Worker notifications
  (3, 'Yangi vazifa', 'Oynali eshiklarni tayyorlash vazifasi tayinlandi', 8, false, 'DetailTask', 3, NOW() - INTERVAL '4 days', false),
  (3, 'Yangi vazifa', 'Stoleshnitsa o''rnatish tayinlandi', 10, false, 'DetailTask', 8, NOW() - INTERVAL '3 days', false),
  -- Warehouse notifications
  (1, 'Kam zaxira', 'Wood Glue - Industrial zaxirasi kam (45 litr)', 5, false, 'WarehouseItem', 5, NOW() - INTERVAL '1 day', false);

-- ============================================================
-- 16. (Customers use soft delete, no IsActive column needed)
-- ============================================================

-- ============================================================
-- 17. ORDER CATEGORIES (link orders to categories properly)
-- ============================================================
INSERT INTO "OrderCategories" ("OrderId", "CategoryId", "CreatedAt")
VALUES
  (2, 2, NOW() - INTERVAL '10 days'),
  (3, 3, NOW() - INTERVAL '5 days'),
  (4, 1, NOW() - INTERVAL '15 days'),
  (5, 4, NOW() - INTERVAL '2 days'),
  (6, 2, NOW() - INTERVAL '1 day')
ON CONFLICT DO NOTHING;

COMMIT;
