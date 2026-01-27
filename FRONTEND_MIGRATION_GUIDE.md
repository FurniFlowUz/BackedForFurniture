# Frontend Migration Guide - Contracts List Fixes

## Critical Change Required

### Status Update API Call (BREAKING CHANGE)

**❌ OLD CODE (BROKEN)**:
```typescript
// This no longer works!
const updateStatus = async (contractId: number, newStatus: string) => {
  await api.put(`/api/contracts/${contractId}/status`, newStatus);
};
```

**✅ NEW CODE (REQUIRED)**:
```typescript
// Wrap status in DTO object
const updateStatus = async (contractId: number, newStatus: string) => {
  await api.put(`/api/contracts/${contractId}/status`, {
    status: newStatus  // ✅ Must be wrapped in object
  });
};
```

---

## Optional Improvements

### 1. Display Category Names (Instead of IDs)

**Before**:
```tsx
// Showed: "1, 2" (category IDs)
<TableCell>
  {contract.categoryIds?.join(', ') || '-'}
</TableCell>
```

**After**:
```tsx
// Shows: "Kitchen, Bedroom" (category names)
<TableCell>
  {contract.categoryNames?.join(', ') || '-'}
</TableCell>
```

### 2. Display Seller Name

**Before**:
```tsx
// Showed: "-" (missing data)
<TableCell>
  {contract.sellerName || '-'}
</TableCell>
```

**After** (Automatic!):
```tsx
// Shows: "Alice Smith" (resolved by backend)
<TableCell>
  {contract.sellerName || '-'}
</TableCell>
```

---

## TypeScript Interface Updates

### Update ContractSummaryDto Interface

```typescript
interface ContractSummaryDto {
  id: number;
  contractNumber: string;
  customerName: string;

  // ✅ NEW FIELDS
  sellerName?: string;           // Seller full name
  categoryNames: string[];       // Display-ready category names

  // Existing fields
  categoryIds: number[];
  totalAmount: number;
  advancePaymentAmount: number;
  remainingAmount: number;
  productionDurationDays: number;
  paymentStatus: string;
  status: string;
  requiresApproval: boolean;
  createdAt: string;
}
```

### Add UpdateContractStatusDto Interface

```typescript
interface UpdateContractStatusDto {
  status: 'New' | 'Active' | 'Completed' | 'Cancelled';
}
```

---

## Error Handling Examples

### Example 1: Status Update with Proper Error Messages

```typescript
const handleStatusChange = async (contractId: number, newStatus: string) => {
  try {
    await api.put(`/api/contracts/${contractId}/status`, {
      status: newStatus
    });

    toast.success('Status updated successfully');
    await refreshContracts();

  } catch (error: any) {
    // Handle specific error codes
    if (error.response?.status === 400) {
      // Business rule violation or validation error
      const message = error.response.data.message;
      const errors = error.response.data.errors;

      if (errors && errors.length > 0) {
        toast.error(errors[0]);  // Show first error
      } else {
        toast.error(message || 'Invalid status update');
      }
    } else if (error.response?.status === 404) {
      toast.error('Contract not found');
    } else {
      toast.error('Failed to update contract status');
      console.error('Status update error:', error);
    }
  }
};
```

### Example 2: Axios Implementation

```typescript
import axios from 'axios';

const contractsApi = {
  updateStatus: async (id: number, status: string) => {
    const response = await axios.put(
      `/api/contracts/${id}/status`,
      { status },  // ✅ Wrapped in object
      {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        }
      }
    );
    return response.data;
  }
};
```

### Example 3: React Query Implementation

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';

const useUpdateContractStatus = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      api.put(`/api/contracts/${id}/status`, { status }),

    onSuccess: () => {
      queryClient.invalidateQueries(['contracts']);
      toast.success('Status updated successfully');
    },

    onError: (error: any) => {
      const message = error.response?.data?.message || 'Failed to update status';
      toast.error(message);
    }
  });
};

// Usage:
const { mutate: updateStatus } = useUpdateContractStatus();
updateStatus({ id: contractId, status: 'Active' });
```

---

## Common Error Messages You'll See

### Validation Errors (400 Bad Request)

```json
{
  "success": false,
  "message": "Invalid contract status",
  "errors": [
    "The status value 'InvalidStatus' is not valid. Valid values are: New, Active, Completed, Cancelled"
  ]
}
```

**Frontend should display**: "The status value 'InvalidStatus' is not valid..."

### Business Rule Errors (400 Bad Request)

```json
{
  "success": false,
  "message": "Cannot update the status of a completed contract.",
  "errors": ["Cannot update the status of a completed contract."]
}
```

**Frontend should display**: "Cannot update the status of a completed contract."

### Not Found (404 Not Found)

```json
{
  "success": false,
  "message": "Contract with id '999' was not found.",
  "errors": ["Contract with id '999' was not found."]
}
```

**Frontend should display**: "Contract with id '999' was not found."

---

## Testing Checklist

### Before Deployment

- [ ] Update status API call to use DTO format
- [ ] Test status updates for all valid transitions
- [ ] Test error handling for invalid status changes
- [ ] Verify category names display correctly
- [ ] Verify seller names display correctly
- [ ] Test with contracts that have no seller (CreatedBy null)
- [ ] Test with contracts that have no categories
- [ ] Test error messages display properly to users

### Valid Status Transitions to Test

```
✅ New → Active
✅ New → Cancelled
✅ Active → Completed
✅ Active → Cancelled
```

### Invalid Status Transitions to Test (Should Show Error)

```
❌ New → Completed (Error: "Cannot complete a contract directly from 'New' status. Activate the contract first.")
❌ Completed → Any (Error: "Cannot update the status of a completed contract.")
❌ Cancelled → Any (Error: "Cannot update the status of a cancelled contract.")
❌ Active → Active (Error: "Contract is already in 'Active' status.")
```

---

## Migration Steps

1. **Update API Call** (REQUIRED):
   - Change all status update calls to use DTO format
   - Add error handling for 400/404 responses

2. **Update TypeScript Interfaces**:
   - Add `sellerName?: string` to ContractSummaryDto
   - Add `categoryNames: string[]` to ContractSummaryDto

3. **Update UI Components** (OPTIONAL):
   - Change table columns to display `categoryNames` instead of `categoryIds`
   - Seller name should automatically appear (no code change needed)

4. **Test Thoroughly**:
   - Test all status transitions
   - Test error scenarios
   - Verify data displays correctly

---

## Quick Copy-Paste Solutions

### React Component Example

```tsx
import { useState } from 'react';
import { toast } from 'react-toastify';

const ContractsTable = ({ contracts, onRefresh }) => {
  const [updating, setUpdating] = useState(false);

  const handleStatusChange = async (contractId: number, newStatus: string) => {
    setUpdating(true);

    try {
      await api.put(`/api/contracts/${contractId}/status`, {
        status: newStatus  // ✅ Wrapped in DTO
      });

      toast.success('Status updated successfully');
      await onRefresh();

    } catch (error: any) {
      const message = error.response?.data?.message || 'Failed to update status';
      toast.error(message);
    } finally {
      setUpdating(false);
    }
  };

  return (
    <Table>
      <thead>
        <tr>
          <th>Contract #</th>
          <th>Customer</th>
          <th>Seller</th>           {/* ✅ Now populated */}
          <th>Categories</th>        {/* ✅ Now shows names */}
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        {contracts.map(contract => (
          <tr key={contract.id}>
            <td>{contract.contractNumber}</td>
            <td>{contract.customerName}</td>
            <td>{contract.sellerName || '-'}</td>  {/* ✅ Automatic */}
            <td>{contract.categoryNames?.join(', ') || '-'}</td>  {/* ✅ Names */}
            <td>
              <Select
                value={contract.status}
                onChange={(e) => handleStatusChange(contract.id, e.target.value)}
                disabled={updating || contract.status === 'Completed' || contract.status === 'Cancelled'}
              >
                <option value="New">New</option>
                <option value="Active">Active</option>
                <option value="Completed">Completed</option>
                <option value="Cancelled">Cancelled</option>
              </Select>
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};
```

---

## Need Help?

- **Full Documentation**: See `CONTRACTS_LIST_FIXES.md`
- **API Testing**: Use Swagger at `http://localhost:5000/swagger`
- **Example Requests**: See Postman collection (if available)

---

**Status**: Ready for frontend integration

**Critical**: Update status API call format before testing
**Optional**: Update display to use new category/seller fields
