-- Details jadvalini tekshirish
SELECT TOP 20 
    d.Id, 
    d.Name, 
    d.FurnitureTypeId, 
    ft.Name as FurnitureTypeName,
    ft.OrderId
FROM Details d
JOIN FurnitureTypes ft ON d.FurnitureTypeId = ft.Id
ORDER BY d.Id DESC;

-- FurnitureTypes jadvalini tekshirish - qaysilarida Details bor
SELECT 
    ft.Id,
    ft.Name,
    ft.OrderId,
    (SELECT COUNT(*) FROM Details d WHERE d.FurnitureTypeId = ft.Id) as DetailsCount
FROM FurnitureTypes ft
WHERE (SELECT COUNT(*) FROM Details d WHERE d.FurnitureTypeId = ft.Id) > 0;
