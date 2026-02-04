# ✅ FurniFlowUz - READY TO TEST

## 🚀 Both Servers Are Running!

### Backend
- **URL**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Status**: ✅ Running (PID: 6036)

### Frontend
- **URL**: http://localhost:8081
- **Status**: ✅ Running
- **Framework**: React + Vite

---

## 👥 Test Users

### 1. Production Manager
```
Email: abror@gmail.com
Password: Abror12345
Role: ProductionManager
```

### 2. Seller
```
Email: sales@furniflowauz.com
Password: Sales12345
Role: Seller
```

### 3. Constructor
```
Email: bek@gmail.com
Password: Bek12345
Role: Constructor
```

---

## 📋 WORKFLOW TESTING STEPS

Open browser: **http://localhost:8081**

### STEP 1: Production Manager - View Templates ✅

1. Login with: `abror@gmail.com` / `Abror12345`
2. Navigate to Templates page
3. You should see **5 existing templates**:
   - **Category 2 (Shkaf-kupe)**:
     - 2 eshikli shkaf
     - 3 eshikli shkaf
     - 4 eshikli shkaf
   - **Category 1 (Oshxona mebeli)**:
     - Stol ustma ustki
     - Stol ostki

4. (Optional) Create new template:
   - Click "Create Template"
   - Select category
   - Enter name, material, notes
   - Set active = true
   - Save

**Expected**: Templates are displayed grouped by category, can toggle active/inactive

---

### STEP 2: Seller - Create Order

1. Logout from Production Manager
2. Login with: `sales@furniflowauz.com` / `Sales12345`
3. Navigate to Orders page
4. Click "Create Order"
5. Fill form:
   - Select Customer (or create new)
   - **Select Category: "Shkaf-kupe" (Category 2)** ⭐
   - Enter deadline date
   - Enter notes: "Mijoz 3 ta 2-eshikli shkaf buyurtma qildi"
6. Save order
7. Assign Constructor and Production Manager:
   - **Constructor**: Bek (bek@gmail.com)
   - **Production Manager**: Abror (abror@gmail.com)

**Expected**: Order created successfully with Category 2 assigned

---

### STEP 3: Constructor - Use Templates & Complete Specs 🔥

1. Logout from Seller
2. Login with: `bek@gmail.com` / `Bek12345`
3. Navigate to "My Orders" or "Assigned Orders"
4. Click on the order you just created
5. **You should see**:
   - Order details
   - Category: "Shkaf-kupe"
   - **Available Templates button/section** ⭐

6. **View Templates for this Category**:
   - Click "View Templates" or similar
   - You should see 3 templates:
     - 2 eshikli shkaf
     - 3 eshikli shkaf
     - 4 eshikli shkaf

7. **Create FurnitureType using Template**:
   - Click "Add Furniture Type" or "+"
   - **Select Template**: "2 eshikli shkaf" ⭐
   - Form should auto-fill with:
     - Name: "2 eshikli shkaf"
     - Material: "LDSP 18mm"
     - Notes: "Standart ilgichlar va tutqichlar"
   - Set Quantity: 3
   - Save

8. **Add Details** (for the FurnitureType):
   - Click on FurnitureType
   - Click "Add Detail"
   - Enter detail info:
     ```
     Name: Eshik paneli
     Width: 600mm
     Height: 2000mm
     Thickness: 18mm
     Quantity: 2
     Material: LDSP 18mm
     ```
   - Add more details as needed

9. **Upload Drawings**:
   - Click "Upload Drawing"
   - Select image file (PNG/JPG) or PDF
   - Upload

10. **Add Technical Specification**:
    - Enter notes about assembly, hardware, etc.
    - "Standart ilgichlar, metall tutqichlar, vertikal qirqim"

11. **Click "Razmer Tayyor" Button** ⭐:
    - This marks specifications as complete
    - FurnitureType is locked (can't edit)
    - If all FurnitureTypes in order are complete:
      - Order status → "SpecificationsReady"
      - Production Manager receives notification

**Expected**:
- ✅ Templates are visible for order's category
- ✅ Template selection pre-fills form data
- ✅ Can add details, drawings, specifications
- ✅ "Razmer tayyor" button works
- ✅ FurnitureType is locked after completion

---

### STEP 4: Production Manager - Receive Notification

1. Logout from Constructor
2. Login with: `abror@gmail.com` / `Abror12345`
3. **Check Notifications** (bell icon or Notifications page)
4. You should see notification:
   - "Order specifications completed"
   - "Order #XXX is ready for production"
5. Click notification to view order
6. Review technical specifications
7. View all details, drawings, technical specs

**Expected**:
- ✅ Notification received when order is ready
- ✅ Can view completed specifications
- ✅ Can review all technical details

---

## 🔧 Backend API Verification

If you want to test API directly:

### Get Production Manager Token
```bash
curl -X POST http://localhost:5000/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"abror@gmail.com","password":"Abror12345"}'
```

### Get Templates for Shkaf Category
```bash
curl -H "Authorization: Bearer {TOKEN}" \
  http://localhost:5000/api/FurnitureTypeTemplate/category/2/active
```

Expected response:
```json
{
  "success": true,
  "message": "Active templates retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "2 eshikli shkaf",
      "categoryId": 2,
      "categoryName": "Shkaf-kupe",
      "defaultMaterial": "LDSP 18mm",
      "defaultNotes": "Standart ilgichlar va tutqichlar",
      "isActive": true
    },
    ...
  ]
}
```

---

## ✅ What's Working

1. ✅ Backend API running on port 5000
2. ✅ Frontend running on port 8081
3. ✅ Database with 5 templates
4. ✅ FurnitureTypeTemplate endpoints working
5. ✅ Authentication working for all 3 roles
6. ✅ Categories exist (4 categories)
7. ✅ Templates exist (5 templates)

---

## 🎯 KEY FEATURES TO TEST

### 1. Template Selection by Category ⭐
When Constructor opens an order, templates should be **filtered by the order's category**.

- Order with Category 2 (Shkaf) → Shows 3 Shkaf templates
- Order with Category 1 (Kitchen) → Shows 2 Kitchen templates

### 2. Template Pre-fill ⭐
When Constructor selects a template:
- Name auto-fills
- Material auto-fills
- Notes auto-fill

Constructor can modify these values before saving.

### 3. Bulk Completion (Variant 2) ⭐
Constructor can submit all data in one request:
```
POST /api/Constructor/complete-with-data/{furnitureTypeId}
{
  "details": [...],
  "notes": "..."
}
```

This saves:
- All details
- Technical specification
- Locks everything
- Updates order status

---

## 🐛 Known Issues

1. **FurnitureTypes.Quantity column**: May not exist (SQL error during migration)
   - **Fix**: Run manually in SQL Server:
   ```sql
   USE FurniFlowUzDb;
   SET QUOTED_IDENTIFIER ON;
   ALTER TABLE [dbo].[FurnitureTypes]
   ADD [Quantity] INT NOT NULL DEFAULT 1;
   ```

2. **HTTPS**: Backend running on HTTP only (not HTTPS)
   - Frontend already configured for HTTP
   - No issue for local testing

---

## 📁 Important Files

### Backend
- Controller: `src\FurniFlowUz.API\Controllers\FurnitureTypeTemplateController.cs`
- Service: `src\FurniFlowUz.Application\Services\FurnitureTypeTemplateService.cs`
- Repository: `src\FurniFlowUz.Infrastructure\Repositories\FurnitureTypeTemplateRepository.cs`

### Frontend
- Service: `src\services\templateService.ts` ✅ Fixed (singular routes)
- API Config: `src\lib\api.ts` ✅ Correct (http://localhost:5000)

### Documentation
- `WORKFLOW_TEST_GUIDE.md` - Detailed API testing guide
- `TESTING_SUMMARY.md` - Technical implementation details
- `add_furniture_type_template_table.sql` - Database migration script

---

## 🚀 START TESTING NOW!

1. Open browser: **http://localhost:8081**
2. Follow workflow steps above
3. Test template selection feature
4. Verify "Razmer tayyor" works
5. Check Production Manager notifications

**Everything is ready!** 🎉

---

## 📞 Support

If you encounter issues:
1. Check backend logs: `C:\Users\User\Desktop\Projects\backendforfurninture\src\FurniFlowUz.API\backend.log`
2. Check frontend console: Browser DevTools (F12)
3. Verify API responses in Network tab
4. Check Swagger: http://localhost:5000/swagger

---

**Last Updated**: 2026-01-28 19:40
**Status**: ✅ READY FOR TESTING
