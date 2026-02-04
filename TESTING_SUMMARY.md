# FurniFlowUz Backend + Frontend Testing Summary

## ✅ Backend Status

### Server Information
- **Backend URL**: `http://localhost:5000`
- **Status**: ✅ Running (PID: 6160)
- **Build**: ✅ Successful (0 warnings, 0 errors)
- **Database**: ✅ Connected (SQL Server LocalDB: FurniFlowUzDb)

### Database Migration
- **FurnitureTypeTemplates Table**: ✅ Created
- **Sample Data**: ✅ 5 templates inserted
  - 3 templates for Category 2 (Shkaf-kupe): 2 eshikli, 3 eshikli, 4 eshikli
  - 2 templates for Category 1 (Oshxona mebeli): Stol ustma ustki, Stol ostki
- **FurnitureTypes.TemplateId**: ✅ Column added
- **FurnitureTypes.Quantity**: ⚠️ Column add failed (SET OPTIONS issue - can be fixed manually)

### API Endpoints Status

#### ✅ Working Endpoints
- `POST /api/Auth/login` - ✅ Returns JWT token
- `GET /api/Categories` - ✅ Returns 4 categories

#### ⚠️ FurnitureTypeTemplate Endpoints
**IMPORTANT DISCOVERY**:
The controller route is `/api/FurnitureTypeTemplate` (SINGULAR), not `/api/FurnitureTypeTemplates` (PLURAL)

**Available in Swagger**:
- GET `/api/FurnitureTypeTemplate`
- GET `/api/FurnitureTypeTemplate/{id}`
- GET `/api/FurnitureTypeTemplate/category/{categoryId}`
- GET `/api/FurnitureTypeTemplate/category/{categoryId}/active`
- POST `/api/FurnitureTypeTemplate`
- PUT `/api/FurnitureTypeTemplate/{id}`
- DELETE `/api/FurnitureTypeTemplate/{id}`
- PATCH `/api/FurnitureTypeTemplate/{id}/toggle-active`

**Issue**: Frontend `templateService.ts` uses PLURAL routes which don't match backend

### Test Credentials
```
Email: bek@gmail.com
Password: Bek12345
Role: Constructor
User ID: 2003
```

**Valid JWT Token** (expires: 2026-01-29T16:32:44Z):
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyMDAzIiwiZW1haWwiOiJiZWtAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ29uc3RydWN0b3IiLCJ1c2VySWQiOiIyMDAzIiwiZmlyc3ROYW1lIjoiQmVrIiwibGFzdE5hbWUiOiIiLCJqdGkiOiJiMmE0YzkyNy02ZWU5LTQ0NGUtOWQ3OC1lODBlYjAwOWY2YzMiLCJleHAiOjE3Njk3MDM5NjQsImlzcyI6IkZ1cm5pRmxvd1V6IiwiYXVkIjoiRnVybmlGbG93VXoifQ.buYXNRvrsXowVqfEhf5Wg8N47_5n6td9riHd31U6Kdw
```

---

## 🔧 Frontend Configuration

### Location
`C:\Users\User\Downloads\workflow-weaver-60-main\workflow-weaver-60-main`

### API Configuration
**File**: `src/lib/api.ts`
**Current Setting**: `const API_BASE_URL = 'http://localhost:5000/api';` ✅ Correct

### Template Service
**File**: `src/services/templateService.ts`
**Issue**: Uses `/FurnitureTypeTemplates` (PLURAL) routes
**Required Fix**: Change all routes to `/FurnitureTypeTemplate` (SINGULAR)

---

## 🐛 Issues Found & Fixes Required

### Issue #1: Route Mismatch (CRITICAL)
**Problem**: Frontend uses `/FurnitureTypeTemplates` but backend serves `/FurnitureTypeTemplate`

**Fix**: Update `src/services/templateService.ts`

**Before**:
```typescript
'/FurnitureTypeTemplates'
'/FurnitureTypeTemplates/${id}'
'/FurnitureTypeTemplates/category/${categoryId}'
'/FurnitureTypeTemplates/category/${categoryId}/active'
```

**After**:
```typescript
'/FurnitureTypeTemplate'
'/FurnitureTypeTemplate/${id}'
'/FurnitureTypeTemplate/category/${categoryId}'
'/FurnitureTypeTemplate/category/${categoryId}/active'
```

### Issue #2: FurnitureTypes.Quantity Column
**Problem**: Failed to add with error: "SET options have incorrect settings"

**Fix**: Run manually in SQL Server:
```sql
USE FurniFlowUzDb;
SET QUOTED_IDENTIFIER ON;
ALTER TABLE [dbo].[FurnitureTypes]
ADD [Quantity] INT NOT NULL DEFAULT 1;
```

---

## 📋 Testing Checklist

### Backend Testing
- [x] Backend starts without errors
- [x] Database connection works
- [x] Auth/login endpoint works
- [x] JWT token is generated
- [x] Categories endpoint works
- [x] FurnitureTypeTemplates table exists
- [x] Sample templates inserted
- [ ] FurnitureTypeTemplate endpoints return data (blocked by route mismatch)

### Frontend Testing
- [ ] Update templateService.ts routes to singular form
- [ ] Start frontend dev server
- [ ] Login works
- [ ] Can fetch categories
- [ ] Can fetch templates by category
- [ ] Production Manager can create templates
- [ ] Constructor can view active templates
- [ ] Template selection works

---

## 🚀 Quick Start Guide

### 1. Start Backend
```bash
cd C:\Users\User\Desktop\Projects\backendforfurninture\src\FurniFlowUz.API
dotnet run
```
Backend will run on: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### 2. Fix Frontend Routes
Edit `C:\Users\User\Downloads\workflow-weaver-60-main\workflow-weaver-60-main\src\services\templateService.ts`

Change all `/FurnitureTypeTemplates` to `/FurnitureTypeTemplate`

### 3. Start Frontend
```bash
cd C:\Users\User\Downloads\workflow-weaver-60-main\workflow-weaver-60-main
npm install  # if not already installed
npm run dev
```
Frontend will run on: `http://localhost:5173` (default Vite port)

### 4. Test the Integration
1. Open browser: `http://localhost:5173`
2. Login with: `bek@gmail.com / Bek12345`
3. Navigate to templates page
4. Should see 5 templates (3 for Shkaf, 2 for Kitchen)

---

## 📊 Sample Data in Database

### Categories
1. Oshxona mebeli (Kitchen furniture)
2. Shkaf-kupe (Wardrobe systems)
3. Yotoqxona mebeli (Bedroom furniture)
4. Ofis mebellari (Office furniture)

### Templates
| ID | Name | Category | Material | Notes |
|----|------|----------|----------|-------|
| 1 | 2 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | Standart ilgichlar va tutqichlar |
| 2 | 3 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | Kuchli ilgichlar va metall tutqichlar |
| 3 | 4 eshikli shkaf | Shkaf-kupe (2) | LDSP 18mm | Kuchli ilgichlar, metall tutqichlar va bo'linma |
| 4 | Stol ustma ustki | Oshxona mebeli (1) | LDSP 16mm | Yengil ilgichlar |
| 5 | Stol ostki | Oshxona mebeli (1) | LDSP 18mm | Kuchli oyoqlar |

---

## 🔗 Useful Links
- Backend Swagger: http://localhost:5000/swagger
- Backend Health Check: http://localhost:5000/api/Categories
- Frontend (after start): http://localhost:5173
- SQL Migration Script: `C:\Users\User\Desktop\Projects\backendforfurninture\add_furniture_type_template_table.sql`

---

## ⚠️ Known Issues
1. **Quantity column**: Not added due to SQL SET OPTIONS error (manual fix required)
2. **Route mismatch**: Frontend uses plural, backend uses singular (frontend fix required)
3. **HTTPS**: Backend running on HTTP only (not HTTPS), frontend updated to match
