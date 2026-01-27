# FurniFlowUz Frontend

Modern React TypeScript frontend for the FurniFlowUz Furniture Manufacturing ERP System.

## Features

### Complete Workflow Support for All 7 Roles

#### 1. **Seller**
- ✅ Create and manage contracts
- ✅ Create orders for contracts
- ✅ View contract and order status
- ✅ Track sales performance

#### 2. **Constructor**
- ✅ Create technical specifications for orders
- ✅ Upload technical drawings (PDF, PNG, JPG)
- ✅ View orders pending specifications

#### 3. **Production Manager**
- ✅ Create production tasks with sequencing
- ✅ Assign tasks to team members
- ✅ Set task priorities and estimates
- ✅ Monitor production progress

#### 4. **Team Leader & Worker**
- ✅ View assigned tasks
- ✅ Update task status
- ✅ Request materials for tasks
- ✅ Track actual hours

#### 5. **Warehouse Manager**
- ✅ Manage material inventory
- ✅ Approve/reject material requests
- ✅ Issue materials with quantity tracking
- ✅ Monitor low stock items

#### 6. **Director**
- ✅ Comprehensive dashboard
- ✅ View all contracts, orders, and tasks
- ✅ Monitor business metrics
- ✅ Oversee all operations

## Tech Stack

- **React 18** - Modern React with hooks
- **TypeScript** - Type-safe development
- **Vite** - Lightning-fast build tool
- **React Router v6** - Client-side routing
- **TanStack Query (React Query)** - Server state management
- **React Hook Form** - Form handling with validation
- **Zod** - Schema validation
- **Axios** - HTTP client
- **Tailwind CSS** - Utility-first CSS framework
- **Lucide React** - Beautiful icons
- **Sonner** - Toast notifications
- **date-fns** - Date manipulation

## Project Structure

```
client/
├── src/
│   ├── components/
│   │   ├── ui/              # Reusable UI components
│   │   │   ├── Button.tsx
│   │   │   ├── Input.tsx
│   │   │   ├── Select.tsx
│   │   │   ├── Card.tsx
│   │   │   └── Modal.tsx
│   │   └── layout/          # Layout components
│   │       ├── Sidebar.tsx
│   │       └── DashboardLayout.tsx
│   ├── pages/               # Page components
│   │   ├── Login.tsx
│   │   ├── Dashboard.tsx
│   │   ├── contracts/       # Contract pages
│   │   │   ├── ContractsList.tsx
│   │   │   └── CreateContract.tsx
│   │   ├── orders/          # Order pages
│   │   │   └── CreateOrder.tsx
│   │   ├── technical-specs/ # Technical spec pages
│   │   │   └── CreateTechnicalSpec.tsx
│   │   ├── production-tasks/# Production task pages
│   │   │   └── CreateProductionTask.tsx
│   │   └── materials/       # Material pages
│   │       ├── IssueMaterial.tsx
│   │       └── CreateMaterialRequest.tsx
│   ├── services/            # API services
│   │   ├── api.ts           # Base API client
│   │   ├── authService.ts
│   │   ├── contractService.ts
│   │   ├── orderService.ts
│   │   ├── technicalSpecService.ts
│   │   ├── productionTaskService.ts
│   │   └── materialService.ts
│   ├── contexts/            # React contexts
│   │   └── AuthContext.tsx
│   ├── types/               # TypeScript types
│   │   └── index.ts         # All type definitions
│   ├── utils/               # Utility functions
│   ├── App.tsx              # Main app component
│   ├── main.tsx            # Entry point
│   └── index.css           # Global styles
├── public/                  # Static assets
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
├── tailwind.config.js
└── README.md
```

## Getting Started

### Prerequisites

- Node.js 18+ and npm/yarn
- Backend API running on http://localhost:5000

### Installation

1. **Install dependencies:**

```bash
cd client
npm install
```

2. **Configure environment variables:**

```bash
# Copy the example env file
copy .env.example .env

# Edit .env and set your API URL
# VITE_API_URL=http://localhost:5000/api
```

3. **Start the development server:**

```bash
npm run dev
```

The app will be available at http://localhost:3000

### Build for Production

```bash
npm run build
```

The production-ready files will be in the `dist/` directory.

### Preview Production Build

```bash
npm run preview
```

## Demo Credentials

Use these credentials to test different roles:

| Role | Username | Password |
|------|----------|----------|
| **Director** | director | password123 |
| **Seller** | seller | password123 |
| **Constructor** | constructor | password123 |
| **Production Manager** | pm | password123 |
| **Team Leader** | teamlead | password123 |
| **Worker** | worker | password123 |
| **Warehouse Manager** | warehouse | password123 |

## Key Features

### 1. **Authentication & Authorization**
- JWT token-based authentication
- Role-based access control
- Automatic token refresh
- Secure route protection

### 2. **Form Validation**
- Client-side validation with Zod schemas
- Real-time error messages
- Type-safe form handling
- Custom validation rules

### 3. **Data Management**
- Optimistic updates
- Automatic cache invalidation
- Pagination support
- Real-time data synchronization

### 4. **User Experience**
- Responsive design (mobile-friendly)
- Loading states
- Error handling
- Toast notifications
- Intuitive navigation

### 5. **File Uploads**
- Technical drawing uploads
- Progress tracking
- File type validation
- Size limits

## API Integration

The frontend communicates with the backend through a centralized API client:

```typescript
// Example API call
import { contractService } from '@/services/contractService';

// Create a contract
const contract = await contractService.create({
  clientName: 'John Doe',
  clientPhone: '+998901234567',
  clientAddress: 'Tashkent',
  totalAmount: 10000000,
  advancePayment: 5000000,
  startDate: '2025-01-20',
  endDate: '2025-03-20',
});
```

All API calls automatically:
- Include authentication headers
- Handle errors
- Redirect to login on 401
- Show toast notifications

## Customization

### Changing Colors

Edit `tailwind.config.js`:

```javascript
theme: {
  extend: {
    colors: {
      primary: {
        // Change these to your brand colors
        500: '#0ea5e9',
        600: '#0284c7',
        700: '#0369a1',
      },
    },
  },
},
```

### Adding New Routes

1. Create page component in `src/pages/`
2. Add route in `src/App.tsx`:

```typescript
<Route
  path="/your-route"
  element={
    <PrivateRoute>
      <YourComponent />
    </PrivateRoute>
  }
/>
```

3. Add navigation link in `src/components/layout/Sidebar.tsx`

## Troubleshooting

### API Connection Issues

**Problem:** "Network Error" or API calls failing

**Solution:**
1. Verify backend is running on http://localhost:5000
2. Check `.env` file has correct `VITE_API_URL`
3. Restart the dev server after changing `.env`

### Authentication Issues

**Problem:** Redirected to login after successful login

**Solution:**
1. Check browser console for errors
2. Verify JWT token is being stored in localStorage
3. Check backend CORS configuration

### Build Errors

**Problem:** TypeScript errors during build

**Solution:**
```bash
# Clear node_modules and reinstall
rm -rf node_modules
npm install

# Run type checking
npm run build
```

## Development Tips

### Hot Module Replacement (HMR)

Vite provides instant updates without page refresh. Save any file and see changes immediately.

### React DevTools

Install React DevTools browser extension for debugging components and state.

### API Debugging

Use browser Network tab to inspect API calls. All requests include:
- Request headers (with auth token)
- Request payload
- Response data
- Status codes

## Performance Optimization

- **Code Splitting**: Routes are automatically code-split
- **Tree Shaking**: Unused code is removed in production
- **Asset Optimization**: Images and assets are optimized
- **Caching**: TanStack Query caches API responses
- **Lazy Loading**: Components load on demand

## Browser Support

- Chrome/Edge (latest 2 versions)
- Firefox (latest 2 versions)
- Safari (latest 2 versions)

## Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## License

Copyright © 2025 FurniFlowUz. All rights reserved.

## Support

For issues or questions:
- Check the documentation
- Review backend API documentation
- Check browser console for errors
- Verify backend is running

---

**Built with ❤️ for FurniFlowUz**
