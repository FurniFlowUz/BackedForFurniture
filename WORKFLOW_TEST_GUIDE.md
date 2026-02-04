# FurniFlowUz Workflow Test Guide

## Test Users

### Production Manager
- **Email**: abror@gmail.com
- **Password**: Abror12345
- **Role**: ProductionManager (Role 4)

### Seller
- **Email**: sales@furniflowauz.com
- **Password**: Sales12345
- **Role**: Seller (Role 2)

### Constructor
- **Email**: bek@gmail.com
- **Password**: Bek12345
- **Role**: Constructor (Role 3)

---

## Workflow Test Steps

### 1. Production Manager - Categories and Templates

**Login**: abror@gmail.com / Abror12345

**Tasks**:
1. ✅ View existing categories (already exist):
   - Category 1: Oshxona mebeli (Kitchen furniture)
   - Category 2: Shkaf-kupe (Wardrobe)
   - Category 3: Yotoqxona mebeli (Bedroom furniture)
   - Category 4: Ofis mebellari (Office furniture)

2. ✅ View/Create FurnitureTypeTemplates (already created):
   - Template 1: "2 eshikli shkaf" (Category 2 - Shkaf-kupe)
   - Template 2: "3 eshikli shkaf" (Category 2 - Shkaf-kupe)
   - Template 3: "4 eshikli shkaf" (Category 2 - Shkaf-kupe)
   - Template 4: "Stol ustma ustki" (Category 1 - Kitchen)
   - Template 5: "Stol ostki" (Category 1 - Kitchen)

**API Endpoints to Test**:
```bash
# Get all categories
GET http://localhost:5000/api/Categories
Authorization: Bearer {PM_TOKEN}

# Get all templates
GET http://localhost:5000/api/FurnitureTypeTemplate
Authorization: Bearer {PM_TOKEN}

# Get templates for Category 2 (Shkaf)
GET http://localhost:5000/api/FurnitureTypeTemplate/category/2/active
Authorization: Bearer {PM_TOKEN}

# Create new template (if needed)
POST http://localhost:5000/api/FurnitureTypeTemplate
Authorization: Bearer {PM_TOKEN}
Content-Type: application/json
{
  "name": "5 eshikli shkaf",
  "categoryId": 2,
  "description": "Besh eshikli juda katta shkaf",
  "defaultMaterial": "LDSP 18mm",
  "defaultNotes": "Kuchli ilgichlar va premium tutqichlar",
  "isActive": true,
  "displayOrder": 4
}
```

---

### 2. Seller - Create Order

**Login**: sales@furniflowauz.com / Sales12345

**Tasks**:
1. View customers
2. Select or create customer
3. Create contract with categories
4. Create order for a specific category (e.g., Shkaf-kupe - Category 2)
5. Assign Constructor and Production Manager

**API Endpoints**:
```bash
# Get customers
GET http://localhost:5000/api/Customers
Authorization: Bearer {SELLER_TOKEN}

# Create contract (if needed)
POST http://localhost:5000/api/Contracts
Authorization: Bearer {SELLER_TOKEN}

# Create order
POST http://localhost:5000/api/Orders
Authorization: Bearer {SELLER_TOKEN}
{
  "contractId": 1,
  "categoryId": 2,
  "notes": "Mijoz 3 ta shkaf buyurtma qildi",
  "deadlineDate": "2026-02-15T00:00:00Z"
}

# Assign Constructor to Order
POST http://localhost:5000/api/Seller/orders/{orderId}/assign
{
  "constructorId": 2003,
  "productionManagerId": 2004
}
```

---

### 3. Constructor - Add Specifications

**Login**: bek@gmail.com / Bek12345

**Tasks**:
1. View assigned orders
2. Select order with Category 2 (Shkaf)
3. See available templates for that category
4. Create FurnitureTypes (using templates or custom)
5. For each FurnitureType:
   - Add Details (dimensions, material)
   - Upload Drawings
   - Add Technical Specifications
6. Click "Razmer tayyor" (Complete)

**API Endpoints**:
```bash
# Get Constructor's assigned orders
GET http://localhost:5000/api/Constructor/my-orders
Authorization: Bearer {CONSTRUCTOR_TOKEN}

# Get templates for order's category
GET http://localhost:5000/api/FurnitureTypeTemplate/category/{categoryId}/active
Authorization: Bearer {CONSTRUCTOR_TOKEN}

# Create FurnitureType (using template or custom)
POST http://localhost:5000/api/Constructor/furniture-types
{
  "orderId": 1,
  "name": "2 eshikli shkaf",
  "material": "LDSP 18mm",
  "quantity": 2,
  "templateId": 1,
  "notes": "Template dan foydalanildi"
}

# Add Detail
POST http://localhost:5000/api/Constructor/details
{
  "furnitureTypeId": 1,
  "name": "Eshik paneli",
  "width": 600.00,
  "height": 2000.00,
  "thickness": 18.00,
  "quantity": 2,
  "material": "LDSP 18mm",
  "notes": "Vertikal qirqim"
}

# Upload Drawing
POST http://localhost:5000/api/Constructor/drawings
Content-Type: multipart/form-data
furnitureTypeId=1&file={drawing_file}

# Create Technical Specification
POST http://localhost:5000/api/Constructor/technical-specifications
{
  "furnitureTypeId": 1,
  "notes": "Standart ilgichlar, metall tutqichlar"
}

# Complete FurnitureType (Razmer tayyor)
POST http://localhost:5000/api/Constructor/complete/{furnitureTypeId}

# OR Complete with data (Variant 2 - bulk submission)
POST http://localhost:5000/api/Constructor/complete-with-data/{furnitureTypeId}
{
  "details": [
    {
      "name": "Eshik paneli",
      "width": 600,
      "height": 2000,
      "thickness": 18,
      "quantity": 2,
      "material": "LDSP 18mm",
      "notes": "Vertikal qirqim"
    },
    {
      "name": "Yonboshar",
      "width": 500,
      "height": 2000,
      "thickness": 18,
      "quantity": 2,
      "material": "LDSP 18mm",
      "notes": "Vertikal qirqim"
    }
  ],
  "notes": "Standart ilgichlar va metall tutqichlar"
}
```

---

### 4. Production Manager - Review Specifications

**Login**: abror@gmail.com / Abror12345

**Tasks**:
1. Receive notification when Order status becomes "SpecificationsReady"
2. View order specifications
3. Review technical specifications
4. Proceed with production planning

**API Endpoints**:
```bash
# Get notifications
GET http://localhost:5000/api/Notifications/my
Authorization: Bearer {PM_TOKEN}

# Get order details
GET http://localhost:5000/api/Orders/{orderId}
Authorization: Bearer {PM_TOKEN}

# Get FurnitureTypes for order
GET http://localhost:5000/api/Constructor/furniture-types/order/{orderId}
Authorization: Bearer {PM_TOKEN}
```

---

## Current Database State

### Categories (4 total)
1. Oshxona mebeli
2. Shkaf-kupe
3. Yotoqxona mebeli
4. Ofis mebellari

### Templates (5 total)
| ID | Name | Category | Material | Active |
|----|------|----------|----------|--------|
| 1 | 2 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | ✅ |
| 2 | 3 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | ✅ |
| 3 | 4 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | ✅ |
| 4 | Stol ustma ustki | Oshxona (1) | LDSP 16mm | ✅ |
| 5 | Stol ostki | Oshxona (1) | LDSP 18mm | ✅ |

---

## Frontend Testing (Manual)

### Start Frontend
```bash
cd C:\Users\User\Downloads\workflow-weaver-60-main\workflow-weaver-60-main
npm install
npm run dev
```

Frontend will run on: http://localhost:5173

### Test Workflow in Browser

1. **Production Manager**:
   - Login: abror@gmail.com / Abror12345
   - Navigate to Templates page
   - View existing templates
   - Create new template (optional)
   - Toggle template active/inactive

2. **Seller**:
   - Login: sales@furniflowauz.com / Sales12345
   - Navigate to Orders page
   - Create new order
   - Select Category (e.g., Shkaf-kupe)
   - Assign Constructor and Production Manager

3. **Constructor**:
   - Login: bek@gmail.com / Bek12345
   - Navigate to My Orders
   - Open order
   - See category and templates
   - Create FurnitureType (select template or custom)
   - Add details, drawings, specifications
   - Click "Razmer tayyor" button
   - Verify order status changes

4. **Production Manager**:
   - Login: abror@gmail.com / Abror12345
   - Check notifications
   - View order with completed specifications
   - Review technical specs

---

## Expected Behavior

1. ✅ Production Manager can create categories and templates
2. ✅ Templates are grouped by category
3. ✅ Seller can create order with specific category
4. ✅ Constructor sees only active templates for order's category
5. ✅ Constructor can use template to pre-fill furniture type data
6. ✅ Constructor can add details, drawings, specifications
7. ✅ "Razmer tayyor" button locks specifications
8. ✅ Order status changes to "SpecificationsReady" when all FurnitureTypes complete
9. ✅ Production Manager receives notification
10. ✅ Production Manager can review specifications

---

## Quick API Test Commands

```bash
# Production Manager Token
PM_TOKEN=$(curl -s -X POST http://localhost:5000/api/Auth/login -H "Content-Type: application/json" -d '{"email":"abror@gmail.com","password":"Abror12345"}' | grep -o '"token":"[^"]*' | cut -d'"' -f4)

# Get templates for Shkaf category
curl -H "Authorization: Bearer $PM_TOKEN" http://localhost:5000/api/FurnitureTypeTemplate/category/2/active

# Constructor Token
CONS_TOKEN=$(curl -s -X POST http://localhost:5000/api/Auth/login -H "Content-Type: application/json" -d '{"email":"bek@gmail.com","password":"Bek12345"}' | grep -o '"token":"[^"]*' | cut -d'"' -f4)

# Get Constructor's orders
curl -H "Authorization: Bearer $CONS_TOKEN" http://localhost:5000/api/Constructor/my-orders

# Get templates from Constructor perspective
curl -H "Authorization: Bearer $CONS_TOKEN" http://localhost:5000/api/FurnitureTypeTemplate/category/2/active
```

---

## Next Steps

1. Start frontend: `npm run dev`
2. Test each role's workflow in browser
3. Verify template selection works
4. Verify "Razmer tayyor" completes specifications
5. Verify Production Manager receives notification
