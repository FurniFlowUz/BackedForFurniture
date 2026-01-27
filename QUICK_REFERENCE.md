# CreateContract API - Quick Reference

## API Endpoint
```
POST /api/Contracts
Content-Type: application/json
Authorization: Bearer {token}
```

## Two Ways to Create a Contract

### Option 1: With Existing Customer
```json
{
  "customerId": 1,
  "categoryIds": [1, 2],
  "totalAmount": 8000.00,
  "advancePaymentAmount": 2400.00,
  "productionDurationDays": 45,
  "signedDate": "2026-01-20T17:00:00",
  "deliveryTerms": "Delivery within 45 days",
  "penaltyTerms": "1% penalty per day for delays"
}
```

### Option 2: With New Customer (Inline Creation)
```json
{
  "newCustomer": {
    "fullName": "John Doe",
    "phoneNumber": "+998901234567",
    "address": "123 Main Street, Tashkent",
    "email": "john.doe@example.com",
    "notes": "VIP customer"
  },
  "categoryIds": [1],
  "totalAmount": 12000.00,
  "advancePaymentAmount": 3600.00,
  "productionDurationDays": 60,
  "signedDate": "2026-01-20T17:00:00"
}
```

## Required Fields

### When using `customerId`:
- ✅ `customerId` (integer > 0)
- ✅ `categoryIds` (array of integers, min 1)
- ✅ `totalAmount` (decimal > 0)

### When using `newCustomer`:
- ✅ `newCustomer.fullName` (string, 2-200 chars)
- ✅ `newCustomer.phoneNumber` (string, valid phone format)
- ✅ `categoryIds` (array of integers, min 1)
- ✅ `totalAmount` (decimal > 0)

### Always optional:
- `advancePaymentAmount` (defaults to 0)
- `productionDurationDays` (defaults to 30)
- `signedDate`
- `deliveryTerms`
- `penaltyTerms`
- `additionalNotes`

## Validation Rules

| Rule | Description |
|------|-------------|
| **Either/Or** | Must provide EITHER `customerId` OR `newCustomer`, not both, not neither |
| **CustomerId > 0** | If using `customerId`, it must be greater than 0 |
| **Customer Exists** | If using `customerId`, customer must exist in database |
| **Unique Phone** | If creating new customer, phone number must be unique |
| **Advance ≤ Total** | `advancePaymentAmount` cannot exceed `totalAmount` |
| **Categories Exist** | All `categoryIds` must exist in database |

## Response Codes

| Code | Meaning | Example |
|------|---------|---------|
| **201** | Created successfully | Contract and optionally customer created |
| **400** | Bad Request | Validation failed, invalid input |
| **401** | Unauthorized | Missing or invalid auth token |
| **404** | Not Found | CustomerId doesn't exist |
| **500** | Server Error | Unexpected error (check logs) |

## Error Examples

### CustomerId = 0
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Either CustomerId or NewCustomer must be provided, but not both."
  ]
}
```

### Customer Not Found
```json
{
  "success": false,
  "message": "Customer with id '99999' was not found.",
  "errors": ["Customer with id '99999' was not found."]
}
```

### Duplicate Phone Number
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Customer with phone number '+998901234567' already exists. Please use the existing customer."
  ]
}
```

### Both CustomerId and NewCustomer Provided
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Cannot provide both CustomerId and NewCustomer. Choose one."
  ]
}
```

## Success Response
```json
{
  "success": true,
  "message": "Contract created successfully",
  "data": {
    "id": 15,
    "contractNumber": "SH-2026-0015",
    "customerId": 1,
    "categoryIds": [1, 2],
    "totalAmount": 8000.00,
    "advancePaymentAmount": 2400.00,
    "remainingAmount": 5600.00,
    "productionDurationDays": 45,
    "status": "New",
    "paymentStatus": "Pending",
    "requiresApproval": true,
    "createdAt": "2026-01-20T17:00:00Z"
  }
}
```

## Frontend Implementation Guide

### TypeScript Interface
```typescript
interface CreateContractDto {
  // Option 1: Existing customer
  customerId?: number;

  // Option 2: New customer
  newCustomer?: {
    fullName: string;
    phoneNumber: string;
    address?: string;
    email?: string;
    notes?: string;
  };

  // Always required
  categoryIds: number[];
  totalAmount: number;

  // Optional
  advancePaymentAmount?: number;
  productionDurationDays?: number;
  signedDate?: string;
  deliveryTerms?: string;
  penaltyTerms?: string;
  additionalNotes?: string;
}
```

### React Example - Existing Customer
```tsx
const createContractWithExistingCustomer = async () => {
  const contract: CreateContractDto = {
    customerId: selectedCustomer.id,
    categoryIds: [1, 2],
    totalAmount: 8000,
    advancePaymentAmount: 2400,
    productionDurationDays: 45
  };

  try {
    const response = await api.post('/contracts', contract);
    console.log('Contract created:', response.data);
  } catch (error) {
    if (error.response?.status === 400) {
      showValidationErrors(error.response.data.errors);
    } else if (error.response?.status === 404) {
      showError('Customer not found');
    }
  }
};
```

### React Example - New Customer
```tsx
const createContractWithNewCustomer = async () => {
  const contract: CreateContractDto = {
    newCustomer: {
      fullName: customerForm.fullName,
      phoneNumber: customerForm.phone,
      address: customerForm.address,
      email: customerForm.email
    },
    categoryIds: [1],
    totalAmount: 12000,
    advancePaymentAmount: 3600
  };

  try {
    const response = await api.post('/contracts', contract);
    console.log('Contract and customer created:', response.data);
  } catch (error) {
    if (error.response?.status === 400) {
      // Handle validation errors
      const errors = error.response.data.errors;
      if (errors.some(e => e.includes('already exists'))) {
        showError('This phone number is already registered. Please use existing customer.');
      } else {
        showValidationErrors(errors);
      }
    }
  }
};
```

## Testing with cURL

### Test 1: Valid with Existing Customer
```bash
curl -X POST "http://localhost:5000/api/Contracts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "customerId": 1,
    "categoryIds": [1],
    "totalAmount": 5000.00
  }'
```

### Test 2: Valid with New Customer
```bash
curl -X POST "http://localhost:5000/api/Contracts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "newCustomer": {
      "fullName": "John Doe",
      "phoneNumber": "+998901234567"
    },
    "categoryIds": [1],
    "totalAmount": 5000.00
  }'
```

### Test 3: Invalid - CustomerId = 0 (Should fail with 400)
```bash
curl -X POST "http://localhost:5000/api/Contracts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "customerId": 0,
    "categoryIds": [1],
    "totalAmount": 5000.00
  }'
```

---

## Need Help?

- **API Documentation**: http://localhost:5000/swagger
- **Full Documentation**: See `CONTRACT_REFACTORING_COMPLETE.md`
- **Summary**: See `CONTRACT_REFACTORING_SUMMARY.md`
