# FurniFlowUz Frontend Architecture

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Browser (http://localhost:3000)          │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │              React Application                      │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │         Authentication Layer                  │  │    │
│  │  │  (AuthContext + JWT Token Management)         │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │           Routing Layer                       │  │    │
│  │  │  (React Router - Public/Private Routes)       │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │            Pages Layer                        │  │    │
│  │  │  • Login                                      │  │    │
│  │  │  • Dashboard                                  │  │    │
│  │  │  • Contracts (List, Create, View)             │  │    │
│  │  │  • Orders (List, Create, View)                │  │    │
│  │  │  • Technical Specs (Create, View)             │  │    │
│  │  │  • Production Tasks (List, Create, View)      │  │    │
│  │  │  • Materials (List, Request, Issue)           │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │         Components Layer                      │  │    │
│  │  │  • Layout (Sidebar, DashboardLayout)          │  │    │
│  │  │  • UI (Button, Input, Select, Card, Modal)    │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │          Services Layer                       │  │    │
│  │  │  (API Communication)                          │  │    │
│  │  │  • authService                                │  │    │
│  │  │  • contractService                            │  │    │
│  │  │  • orderService                               │  │    │
│  │  │  • technicalSpecService                       │  │    │
│  │  │  • productionTaskService                      │  │    │
│  │  │  • materialService                            │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  │  ┌──────────────────────────────────────────────┐  │    │
│  │  │       HTTP Client (Axios)                     │  │    │
│  │  │  • Auto JWT injection                         │  │    │
│  │  │  • Error handling                             │  │    │
│  │  │  • Response transformation                    │  │    │
│  │  └──────────────────────────────────────────────┘  │    │
│  │                                                      │    │
│  └────────────────────────────────────────────────────┘    │
│                                                              │
└─────────────────────────────────────────────────────────────┘
                              ↓
                          HTTP/HTTPS
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Backend API (http://localhost:5000/api)         │
│                      .NET 8 Web API                          │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 Data Flow

### 1. User Authentication Flow
```
User enters credentials
        ↓
Login Component validates
        ↓
authService.login() called
        ↓
POST /api/auth/login
        ↓
Backend validates & returns JWT
        ↓
Token stored in localStorage
        ↓
User redirected to Dashboard
        ↓
All subsequent API calls include token
```

### 2. Create Contract Flow (Seller)
```
Seller clicks "New Contract"
        ↓
CreateContract form loads
        ↓
Seller fills form
        ↓
React Hook Form validates (client-side)
        ↓
Zod schema validates data
        ↓
contractService.create() called
        ↓
POST /api/contracts (with JWT header)
        ↓
Backend validates & creates contract
        ↓
TanStack Query invalidates cache
        ↓
Success toast shown
        ↓
Redirect to Contracts List
```

### 3. Material Request Flow (Worker → Warehouse)
```
Worker views assigned task
        ↓
Clicks "Request Materials"
        ↓
CreateMaterialRequest form loads
        ↓
Worker selects material & quantity
        ↓
Form validates
        ↓
materialRequestService.create() called
        ↓
POST /api/material-requests
        ↓
Request created with "Pending" status
        ↓
--- Worker's part done ---
        ↓
Warehouse Manager logs in
        ↓
Views pending requests
        ↓
Clicks "Issue"
        ↓
IssueMaterial form loads
        ↓
Shows request details & stock
        ↓
Warehouse Manager enters quantity
        ↓
materialRequestService.issue() called
        ↓
POST /api/material-requests/{id}/issue
        ↓
Backend updates stock & request status
        ↓
Material issued successfully
```

---

## 🔐 Security Architecture

### JWT Token Flow
```
1. Login → Backend issues JWT
2. Token stored in localStorage
3. API client reads token
4. Every request includes: Authorization: Bearer {token}
5. Backend validates token
6. If invalid/expired → 401 response
7. Frontend catches 401 → clears token → redirects to login
```

### Protected Routes
```
src/App.tsx
    ↓
PrivateRoute component checks:
    • Is user authenticated? (token exists)
    • Is token valid?
    ↓
If YES → Render page in DashboardLayout
If NO → Redirect to /login
```

### Role-Based Access
```
Sidebar component filters navigation by user.role
    ↓
Director → sees all menu items
Seller → sees Contracts, Orders
Constructor → sees Orders (for specs)
Production Manager → sees Tasks, Teams
Worker → sees My Tasks
Warehouse Manager → sees Materials, Requests
```

---

## 🎨 Component Hierarchy

```
App
└── BrowserRouter
    └── AuthProvider
        └── QueryClientProvider
            ├── PublicRoute
            │   └── Login
            │
            └── PrivateRoute
                └── DashboardLayout
                    ├── Sidebar
                    │   ├── Logo
                    │   ├── UserInfo
                    │   ├── Navigation
                    │   │   └── NavLinks (filtered by role)
                    │   └── Logout Button
                    │
                    └── Main Content
                        └── Pages
                            ├── Dashboard
                            │   └── Stats Cards
                            │   └── Recent Activity
                            │
                            ├── ContractsList
                            │   └── Card
                            │       └── Table
                            │           └── Contract Rows
                            │
                            ├── CreateContract
                            │   └── Card
                            │       └── Form
                            │           ├── Input (Client Name)
                            │           ├── Input (Phone)
                            │           ├── Input (Address)
                            │           ├── Input (Total Amount)
                            │           ├── Input (Advance Payment)
                            │           ├── Input (Start Date)
                            │           ├── Input (End Date)
                            │           └── Button (Submit)
                            │
                            └── ... (other pages follow similar pattern)
```

---

## 🔄 State Management

### Global State (AuthContext)
```typescript
{
  user: User | null,
  isAuthenticated: boolean,
  isLoading: boolean,
  login: (credentials) => Promise<void>,
  logout: () => void
}
```

### Server State (TanStack Query)
```typescript
// Cached queries
- ['contracts'] - List of contracts
- ['contracts', id] - Single contract
- ['orders'] - List of orders
- ['production-tasks'] - List of tasks
- ['my-tasks'] - User's assigned tasks
- ['materials'] - List of materials
- ['material-requests'] - Material requests

// Mutations trigger cache invalidation
createContract() → invalidates ['contracts']
createOrder() → invalidates ['orders'], ['contracts']
issueM material() → invalidates ['materials'], ['material-requests']
```

### Form State (React Hook Form)
```typescript
// Each form manages its own state
{
  values: FormData,
  errors: ValidationErrors,
  touched: TouchedFields,
  isSubmitting: boolean,
  isValid: boolean
}
```

---

## 📦 Module Dependencies

```
React Application
    ├── react-router-dom (Routing)
    ├── @tanstack/react-query (Server state)
    ├── react-hook-form (Form state)
    ├── zod (Validation schemas)
    ├── axios (HTTP client)
    ├── tailwindcss (Styling)
    ├── lucide-react (Icons)
    ├── sonner (Toasts)
    └── date-fns (Date formatting)
```

---

## 🎯 Form Validation Architecture

```
User Input
    ↓
React Hook Form captures
    ↓
Zod Schema validates
    ↓
┌─────────────────────────────┐
│ Validation Rules            │
├─────────────────────────────┤
│ • Type checking             │
│ • Required fields           │
│ • Min/max lengths           │
│ • Custom validators         │
│ • Cross-field validation    │
└─────────────────────────────┘
    ↓
If INVALID → Show errors
If VALID → Submit to API
```

### Example: Contract Validation
```typescript
contractSchema = z.object({
  clientName: z.string().min(2),
  clientPhone: z.string().min(9),
  totalAmount: z.number().min(0),
  advancePayment: z.number().min(0),
  startDate: z.string().min(1),
  endDate: z.string().min(1),
})
.refine((data) => {
  return new Date(data.endDate) > new Date(data.startDate);
}, {
  message: 'End date must be after start date',
  path: ['endDate'],
})
.refine((data) => {
  return data.advancePayment <= data.totalAmount;
}, {
  message: 'Advance cannot exceed total',
  path: ['advancePayment'],
});
```

---

## 🚀 Build & Deployment Architecture

### Development
```
npm run dev
    ↓
Vite dev server starts
    ↓
Hot Module Replacement (HMR) enabled
    ↓
http://localhost:3000
    ↓
Proxies /api → http://localhost:5000
```

### Production Build
```
npm run build
    ↓
TypeScript compilation
    ↓
Vite builds & optimizes
    ↓
Tree shaking (removes unused code)
    ↓
Code splitting (per route)
    ↓
Asset optimization
    ↓
Output to dist/
    ↓
dist/
  ├── index.html
  ├── assets/
  │   ├── index-[hash].js (main bundle)
  │   ├── vendor-[hash].js (dependencies)
  │   └── [route]-[hash].js (lazy chunks)
  └── vite.svg
```

---

## 🔧 Configuration Files

```
client/
├── vite.config.ts          # Build configuration
├── tsconfig.json           # TypeScript configuration
├── tailwind.config.js      # CSS framework config
├── postcss.config.js       # CSS processing
├── .env                    # Environment variables
└── package.json            # Dependencies & scripts
```

---

## 📱 Responsive Design Architecture

```
Tailwind Breakpoints:
    sm: 640px   → Mobile landscape
    md: 768px   → Tablet
    lg: 1024px  → Desktop
    xl: 1280px  → Large desktop

Grid System:
    Mobile    → grid-cols-1
    Tablet    → grid-cols-2
    Desktop   → grid-cols-4

Layout:
    Mobile    → Sidebar hidden (future: hamburger menu)
    Tablet    → Sidebar visible, content scales
    Desktop   → Full layout with generous spacing
```

---

## 🎯 Performance Optimizations

1. **Code Splitting**
   - Each route loads separately
   - Reduces initial bundle size

2. **Query Caching**
   - API responses cached for 5 minutes
   - Reduces unnecessary network calls

3. **Lazy Loading**
   - Components load on demand
   - Faster initial page load

4. **Tree Shaking**
   - Unused code removed in production
   - Smaller bundle size

5. **Asset Optimization**
   - Images optimized
   - CSS minimized
   - JS minified

---

## 📊 Type Safety Architecture

```
Backend DTOs
    ↓
Converted to TypeScript types
    ↓
src/types/index.ts
    ↓
Used throughout application:
    • Service function parameters
    • Form data types
    • Component props
    • API responses
    ↓
Compile-time type checking
    ↓
Prevents runtime errors
```

---

## 🎉 Summary

This architecture provides:

✅ **Security** - JWT auth, protected routes, role-based access
✅ **Performance** - Code splitting, caching, lazy loading
✅ **Type Safety** - Full TypeScript coverage
✅ **Maintainability** - Clear separation of concerns
✅ **Scalability** - Modular structure, easy to extend
✅ **UX** - Form validation, loading states, error handling
✅ **Developer Experience** - Hot reload, type hints, clean code

**Ready for production! 🚀**
