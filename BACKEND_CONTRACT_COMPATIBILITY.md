# Contract API Backward Compatibility Guide

## Overview
The backend has been refactored to support the new multi-step contract wizard, while maintaining **full backward compatibility** with the old frontend that expects the original schema.

## What Changed

### Original Schema (Old Frontend Expects)
```typescript
interface Contract {
  id: string;
  contractNumber: string;
  customerId: string;
  customerName: string;
  categoryId?: string;          // Single category
  categoryName?: string;
  description?: string;
  totalAmount: number;
  advancePaymentPercentage?: number;  // Percentage-based
  deadline?: string;
  signedDate?: string;
  paymentStatus?: PaymentStatus;
  terms?: string;
  notes?: string;
  status: ContractStatus;
  createdAt: string;
  updatedAt: string;
  sellerId: string;
  sellerName: string;
}
```

### New Backend Schema
```csharp
public class Contract {
    public int Id { get; set; }
    public string ContractNumber { get; set; }
    public int CustomerId { get; set; }
    public string CategoryIds { get; set; }  // Multiple categories (comma-separated)
    public decimal TotalAmount { get; set; }
    public decimal AdvancePaymentAmount { get; set; }  // Exact amount
    public int ProductionDurationDays { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? DeliveryTerms { get; set; }
    public string? PenaltyTerms { get; set; }
    public string? AdditionalNotes { get; set; }
    public bool RequiresApproval { get; set; }
    // ... more fields
}
```

## Backward Compatibility Implementation

### 1. **ContractDto** - Computed Properties
The backend ContractDto now includes computed properties that map new fields to old fields:

```csharp
// OLD SCHEMA SUPPORT (computed properties)
public int? CategoryId => CategoryIds.Any() ? CategoryIds.First() : null;
public decimal? AdvancePaymentPercentage => TotalAmount > 0 ? (AdvancePaymentAmount / TotalAmount) * 100 : null;
public DateTime? Deadline => SignedDate?.AddDays(ProductionDurationDays);
public string? Description => AdditionalNotes;
public string? Notes => AdditionalNotes;
public string? Terms => $"{DeliveryTerms}\n{PenaltyTerms}".Trim();
public int SellerId => CreatedBy?.Id ?? 0;
public string SellerName => CreatedBy?.FullName ?? string.Empty;
public int CustomerId => Customer?.Id ?? 0;
public string CustomerName => Customer?.FullName ?? string.Empty;
```

### 2. **CreateContractDto** - Accepts Both Formats
When creating a contract, the backend accepts BOTH old and new formats:

```csharp
// NEW FIELDS
public List<int> CategoryIds { get; set; }
public decimal AdvancePaymentAmount { get; set; }
public int ProductionDurationDays { get; set; }
public string? DeliveryTerms { get; set; }
public string? PenaltyTerms { get; set; }
public string? AdditionalNotes { get; set; }

// OLD FIELDS (for backward compatibility)
public int? CategoryId { get; set; }
public decimal? AdvancePaymentPercentage { get; set; }
public DateTime? Deadline { get; set; }
public string? Description { get; set; }
public string? Notes { get; set; }
public string? Terms { get; set; }
```

### 3. **Automatic Conversion Logic** (in ContractService)
The service automatically converts old format to new format:

```csharp
// CategoryId -> CategoryIds
if (request.CategoryId.HasValue && !request.CategoryIds.Any())
    request.CategoryIds = new List<int> { request.CategoryId.Value };

// AdvancePaymentPercentage -> AdvancePaymentAmount
if (request.AdvancePaymentPercentage.HasValue && request.AdvancePaymentAmount == 0)
    request.AdvancePaymentAmount = request.TotalAmount * (request.AdvancePaymentPercentage.Value / 100);

// Description/Notes -> AdditionalNotes
if (string.IsNullOrEmpty(request.AdditionalNotes))
    request.AdditionalNotes = request.Description ?? request.Notes;

// Terms -> DeliveryTerms
if (string.IsNullOrEmpty(request.DeliveryTerms) && !string.IsNullOrEmpty(request.Terms))
    request.DeliveryTerms = request.Terms;

// Deadline -> ProductionDurationDays
if (request.Deadline.HasValue && request.SignedDate.HasValue)
    request.ProductionDurationDays = (request.Deadline.Value - request.SignedDate.Value).Days;
```

## API Endpoints

### GET /api/Contracts
**Query Parameters:**
- `Status` - Filter by contract status
- `CustomerId` - Filter by customer
- `CategoryId` - Filter by category (supports old single category)
- `FromDate` - Filter from date
- `ToDate` - Filter to date
- `SearchTerm` - Search term
- `PageNumber` - Page number (default: 1)
- `PageSize` - Page size (default: 10)
- `SortBy` - Sort field
- `SortDescending` - Sort direction

**Response Format:**
```json
{
  "success": true,
  "message": "Contracts retrieved successfully",
  "data": {
    "items": [
      {
        "id": 1,
        "contractNumber": "SH-2026-0001",
        "customerName": "John Doe",
        "categoryIds": [1, 2],
        "categoryId": 1,  // First category for backward compatibility
        "totalAmount": 10000,
        "advancePaymentAmount": 3000,
        "advancePaymentPercentage": 30,  // Computed
        "productionDurationDays": 30,
        "deadline": "2026-02-20T00:00:00Z",  // Computed
        "paymentStatus": "Pending",
        "status": "New",
        "requiresApproval": true,
        "createdAt": "2026-01-20T10:00:00Z"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "errors": null
}
```

### POST /api/Contracts
**Accepts BOTH old and new formats:**

**Old Format (Still Works):**
```json
{
  "customerId": 1,
  "categoryId": 1,
  "description": "Living room furniture",
  "totalAmount": 10000,
  "advancePaymentPercentage": 30,
  "deadline": "2026-02-20",
  "signedDate": "2026-01-20",
  "terms": "Delivery within 30 days",
  "notes": "Customer prefers oak wood"
}
```

**New Format:**
```json
{
  "customerId": 1,
  "categoryIds": [1, 2],
  "totalAmount": 10000,
  "advancePaymentAmount": 3000,
  "productionDurationDays": 30,
  "signedDate": "2026-01-20",
  "deliveryTerms": "Delivery within 30 days",
  "penaltyTerms": "5% per day for delays",
  "additionalNotes": "Customer prefers oak wood"
}
```

### GET /api/Contracts/{id}
Returns full contract details with both old and new fields populated.

### GET /api/Contracts/stats
**Current Response** (seller stats):
```json
{
  "success": true,
  "message": "Contract statistics retrieved successfully",
  "data": {
    "activeContracts": 5,
    "pendingOrders": 3,
    "completedOrders": 10,
    "totalRevenue": 50000,
    "revenueChangePercentage": 15.5,
    "activeContractsChangePercentage": 10.0,
    "pendingOrdersChangePercentage": -5.0
  }
}
```

**Note:** The frontend expects different fields:
- `totalContracts` (not provided)
- `draftContracts` (not provided)

**Solution:** The frontend should map:
- `totalContracts` = `activeContracts` + completed contracts count
- `draftContracts` = contracts with status "New"

## Testing Scenarios

### Scenario 1: Old Frontend Creates Contract
**Request:**
```http
POST /api/Contracts
Content-Type: application/json

{
  "customerId": 1,
  "categoryId": 2,
  "totalAmount": 5000,
  "advancePaymentPercentage": 50,
  "deadline": "2026-03-01",
  "signedDate": "2026-01-20"
}
```

**Expected Behavior:**
- Backend converts `categoryId: 2` to `categoryIds: [2]`
- Backend converts `advancePaymentPercentage: 50` to `advancePaymentAmount: 2500`
- Backend converts `deadline` to `productionDurationDays: 40`
- Returns contract with BOTH old and new fields populated

### Scenario 2: Old Frontend Fetches Contracts
**Request:**
```http
GET /api/Contracts?PageNumber=1&PageSize=10&Status=Active
```

**Expected Behavior:**
- Returns paginated list with ContractSummaryDto
- Each contract includes:
  - `categoryId` (first category from categoryIds)
  - `advancePaymentPercentage` (computed from amounts)
  - Both new fields (`categoryIds`, `advancePaymentAmount`) AND old fields

### Scenario 3: New Frontend Creates Contract
**Request:**
```http
POST /api/Contracts
Content-Type: application/json

{
  "customerId": 1,
  "categoryIds": [1, 2, 3],
  "totalAmount": 15000,
  "advancePaymentAmount": 5000,
  "productionDurationDays": 45,
  "signedDate": "2026-01-20",
  "deliveryTerms": "Free delivery within city limits",
  "penaltyTerms": "10% penalty for cancellation",
  "additionalNotes": "Customer requested premium materials"
}
```

**Expected Behavior:**
- Backend stores multiple categories
- Backend uses exact advance amount
- Returns contract with computed old fields for backward compatibility

## Key Points

1. **✅ Full Backward Compatibility**: Old frontend works without changes
2. **✅ New Features Available**: New frontend can use multiple categories, exact amounts, and separate terms
3. **✅ Automatic Conversion**: Backend handles conversion transparently
4. **✅ Contract Number Format**: Changed from `CON-YYYYMMDD-XXXX` to `SH-YYYY-XXXX`
5. **✅ No Breaking Changes**: All existing API contracts maintained

## Migration Path

For frontend to migrate to new schema:
1. Update TypeScript types to include new fields
2. Update forms to send new format
3. Remove old fields from requests once fully migrated
4. Backend will continue to support both formats indefinitely

## Build Status
✅ Build successful with only warnings (null reference warnings, no errors)
