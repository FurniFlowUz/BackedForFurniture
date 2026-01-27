import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  FileText,
  ShoppingCart,
  Wrench,
  Users,
  ClipboardList,
  Package,
  Settings,
  LogOut,
  DollarSign,
  BarChart3
} from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { UserRole } from '@/types';

interface NavItem {
  to: string;
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  roles: UserRole[];
}

const navItems: NavItem[] = [
  {
    to: '/dashboard',
    icon: LayoutDashboard,
    label: 'Dashboard',
    roles: Object.values(UserRole),
  },
  {
    to: '/orders',
    icon: ShoppingCart,
    label: 'Orders',
    roles: [UserRole.Director, UserRole.Seller, UserRole.Constructor, UserRole.ProductionManager],
  },
  {
    to: '/finance',
    icon: DollarSign,
    label: 'Finance',
    roles: [UserRole.Director, UserRole.Seller],
  },
  {
    to: '/contracts',
    icon: FileText,
    label: 'Contracts',
    roles: [UserRole.Director, UserRole.Seller],
  },
  {
    to: '/reports',
    icon: BarChart3,
    label: 'Reports',
    roles: [UserRole.Director, UserRole.Seller, UserRole.ProductionManager],
  },
  {
    to: '/production-tasks',
    icon: ClipboardList,
    label: 'Production Tasks',
    roles: [UserRole.Director, UserRole.ProductionManager, UserRole.TeamLeader, UserRole.Worker],
  },
  {
    to: '/materials',
    icon: Package,
    label: 'Materials',
    roles: [UserRole.Director, UserRole.WarehouseManager, UserRole.ProductionManager],
  },
  {
    to: '/teams',
    icon: Users,
    label: 'Teams',
    roles: [UserRole.Director, UserRole.ProductionManager],
  },
];

export function Sidebar() {
  const { user, logout } = useAuth();

  const filteredNavItems = navItems.filter((item) =>
    user?.role ? item.roles.includes(user.role) : false
  );

  return (
    <div className="flex flex-col h-full bg-gray-900 text-white w-64">
      {/* Logo */}
      <div className="flex items-center justify-center h-16 border-b border-gray-800">
        <Wrench className="h-8 w-8 text-primary-400" />
        <span className="ml-2 text-xl font-bold">FurniFlowUz</span>
      </div>

      {/* User Info */}
      <div className="px-4 py-4 border-b border-gray-800">
        <p className="text-sm font-medium">{user?.fullName}</p>
        <p className="text-xs text-gray-400">{user?.role}</p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
        {filteredNavItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-colors ${
                  isActive
                    ? 'bg-primary-600 text-white'
                    : 'text-gray-300 hover:bg-gray-800 hover:text-white'
                }`
              }
            >
              <Icon className="h-5 w-5 mr-3" />
              {item.label}
            </NavLink>
          );
        })}
      </nav>

      {/* Footer */}
      <div className="px-2 py-4 border-t border-gray-800 space-y-1">
        <NavLink
          to="/settings"
          className={({ isActive }) =>
            `flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-colors ${
              isActive
                ? 'bg-primary-600 text-white'
                : 'text-gray-300 hover:bg-gray-800 hover:text-white'
            }`
          }
        >
          <Settings className="h-5 w-5 mr-3" />
          Settings
        </NavLink>
        <button
          onClick={logout}
          className="flex items-center w-full px-4 py-3 text-sm font-medium text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition-colors"
        >
          <LogOut className="h-5 w-5 mr-3" />
          Logout
        </button>
      </div>
    </div>
  );
}
