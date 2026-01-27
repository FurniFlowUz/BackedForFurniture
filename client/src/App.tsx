import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'sonner';

import { AuthProvider, useAuth } from './contexts/AuthContext';
import { DashboardLayout } from './components/layout/DashboardLayout';
import { Login } from './pages/Login';
import { Dashboard } from './pages/Dashboard';
import { Finance } from './pages/finance/Finance';
import { Reports } from './pages/reports/Reports';
import { ContractsList } from './pages/contracts/ContractsList';
import { CreateContract } from './pages/contracts/CreateContract';
import { OrdersList } from './pages/orders/OrdersList';
import { CreateOrder } from './pages/orders/CreateOrder';
import { CreateTechnicalSpec } from './pages/technical-specs/CreateTechnicalSpec';
import { ProductionTasksList } from './pages/production-tasks/ProductionTasksList';
import { CreateProductionTask } from './pages/production-tasks/CreateProductionTask';
import { MaterialsList } from './pages/materials/MaterialsList';
import { IssueMaterial } from './pages/materials/IssueMaterial';
import { CreateMaterialRequest } from './pages/materials/CreateMaterialRequest';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <DashboardLayout>{children}</DashboardLayout>;
}

function PublicRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
}

function AppRoutes() {
  return (
    <Routes>
      {/* Public Routes */}
      <Route
        path="/login"
        element={
          <PublicRoute>
            <Login />
          </PublicRoute>
        }
      />

      {/* Private Routes */}
      <Route
        path="/dashboard"
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        }
      />

      {/* Finance */}
      <Route
        path="/finance"
        element={
          <PrivateRoute>
            <Finance />
          </PrivateRoute>
        }
      />

      {/* Reports */}
      <Route
        path="/reports"
        element={
          <PrivateRoute>
            <Reports />
          </PrivateRoute>
        }
      />

      {/* Contracts */}
      <Route
        path="/contracts"
        element={
          <PrivateRoute>
            <ContractsList />
          </PrivateRoute>
        }
      />
      <Route
        path="/contracts/create"
        element={
          <PrivateRoute>
            <CreateContract />
          </PrivateRoute>
        }
      />

      {/* Orders */}
      <Route
        path="/orders"
        element={
          <PrivateRoute>
            <OrdersList />
          </PrivateRoute>
        }
      />
      <Route
        path="/orders/create"
        element={
          <PrivateRoute>
            <CreateOrder />
          </PrivateRoute>
        }
      />

      {/* Technical Specifications */}
      <Route
        path="/technical-specs/create"
        element={
          <PrivateRoute>
            <CreateTechnicalSpec />
          </PrivateRoute>
        }
      />

      {/* Production Tasks */}
      <Route
        path="/production-tasks"
        element={
          <PrivateRoute>
            <ProductionTasksList />
          </PrivateRoute>
        }
      />
      <Route
        path="/production-tasks/create"
        element={
          <PrivateRoute>
            <CreateProductionTask />
          </PrivateRoute>
        }
      />

      {/* Materials */}
      <Route
        path="/materials"
        element={
          <PrivateRoute>
            <MaterialsList />
          </PrivateRoute>
        }
      />
      <Route
        path="/materials/requests/:id/issue"
        element={
          <PrivateRoute>
            <IssueMaterial />
          </PrivateRoute>
        }
      />
      <Route
        path="/materials/requests/create"
        element={
          <PrivateRoute>
            <CreateMaterialRequest />
          </PrivateRoute>
        }
      />

      {/* Default Route */}
      <Route path="/" element={<Navigate to="/dashboard" replace />} />

      {/* 404 Not Found */}
      <Route
        path="*"
        element={
          <div className="flex flex-col items-center justify-center h-screen">
            <h1 className="text-6xl font-bold text-gray-900 mb-4">404</h1>
            <p className="text-xl text-gray-600 mb-8">Page not found</p>
            <a href="/dashboard" className="text-primary-600 hover:text-primary-700 font-medium">
              Go back to dashboard
            </a>
          </div>
        }
      />
    </Routes>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <AppRoutes />
          <Toaster position="top-right" richColors />
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
