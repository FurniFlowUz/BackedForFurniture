import { useAuth } from '@/contexts/AuthContext';
import { useQuery } from '@tanstack/react-query';
import { dashboardService } from '@/services/dashboardService';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';
import {
  FileText,
  ShoppingCart,
  Users,
  TrendingUp,
  ChevronRight,
  ShoppingBag,
  CheckSquare,
  Package,
} from 'lucide-react';
import { formatDistanceToNow } from 'date-fns';
import { clsx } from 'clsx';
import { PendingItemPriority } from '@/types';

// Activity type to icon mapping
const getActivityIcon = (type: string) => {
  switch (type) {
    case 'OrderCreated':
    case 'OrderUpdated':
    case 'order':
      return ShoppingBag;
    case 'ContractCreated':
    case 'ContractUpdated':
    case 'contract':
      return FileText;
    case 'TaskAssigned':
    case 'TaskCompleted':
    case 'task':
      return CheckSquare;
    case 'MaterialApproved':
    case 'MaterialRequest':
    case 'material':
      return Package;
    default:
      return FileText;
  }
};

// Priority to color mapping
const getPriorityColor = (priority: PendingItemPriority | string): string => {
  switch (priority) {
    case 'Urgent':
    case 'high':
      return 'bg-red-500';
    case 'High':
    case 'medium':
      return 'bg-orange-500';
    case 'Medium':
      return 'bg-yellow-500';
    case 'Low':
    case 'low':
      return 'bg-blue-400';
    default:
      return 'bg-gray-400';
  }
};

// Format due date
const formatDueDate = (dueDate?: string): string => {
  if (!dueDate) return '';
  const date = new Date(dueDate);
  const now = new Date();
  const diffDays = Math.ceil((date.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

  if (diffDays === 0) return 'Today';
  if (diffDays === 1) return 'Tomorrow';
  if (diffDays > 1 && diffDays <= 7) return 'This week';
  return date.toLocaleDateString();
};

// Format currency
const formatCurrency = (amount: number): string => {
  if (amount >= 1000000) {
    return `$${(amount / 1000000).toFixed(1)}M`;
  }
  if (amount >= 1000) {
    return `$${(amount / 1000).toFixed(0)}K`;
  }
  return `$${amount}`;
};

// Mock data - rasmdagi dizayn uchun
const mockActivities = [
  { id: 1, type: 'order', title: 'Order #ORD-2024-015 created', createdAt: new Date(Date.now() - 5 * 60 * 1000).toISOString() },
  { id: 2, type: 'task', title: 'Dimensions updated for Category A', createdAt: new Date(Date.now() - 60 * 60 * 1000).toISOString() },
  { id: 3, type: 'task', title: 'Task assigned to John Doe', createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString() },
  { id: 4, type: 'material', title: 'Material request approved', createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString() },
];

const mockPendingItems = [
  { id: 1, title: 'Review order dimensions', priority: 'Urgent' as PendingItemPriority, dueDate: new Date().toISOString() },
  { id: 2, title: 'Approve material request', priority: 'High' as PendingItemPriority, dueDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString() },
  { id: 3, title: 'Assign tasks to team', priority: 'Medium' as PendingItemPriority, dueDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString() },
  { id: 4, title: 'Update inventory count', priority: 'Low' as PendingItemPriority, dueDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString() },
];

export function Dashboard() {
  const { user } = useAuth();

  // Fetch dashboard data
  const { data: dashboardData, isLoading } = useQuery({
    queryKey: ['dashboard-data'],
    queryFn: () => dashboardService.getDashboardData(),
    refetchInterval: 60000,
    retry: 1,
  });

  // API yoki mock data
  const totalContracts = dashboardData?.contractStats?.activeContracts || 156;
  const activeOrders = dashboardData?.orderStats?.inProgress || 34;
  const employeesCount = dashboardData?.employeesCount || 87;
  const monthlyRevenue = dashboardData?.contractStats?.totalRevenue || 1200000;
  const contractsChange = dashboardData?.contractStats?.activeContractsChangePercentage ?? 12;
  const revenueChange = dashboardData?.contractStats?.revenueChangePercentage ?? 8;

  const activities = dashboardData?.activities?.length ? dashboardData.activities : mockActivities;
  const pendingItems = dashboardData?.pendingItems?.length ? dashboardData.pendingItems : mockPendingItems;

  const stats = [
    {
      title: 'Total Contracts',
      value: totalContracts,
      icon: FileText,
      bgColor: 'bg-amber-50',
      iconColor: 'text-amber-600',
      change: contractsChange,
      changeText: 'this month',
    },
    {
      title: 'Active Orders',
      value: activeOrders,
      icon: ShoppingCart,
      bgColor: 'bg-emerald-50',
      iconColor: 'text-emerald-600',
    },
    {
      title: 'Employees',
      value: employeesCount,
      icon: Users,
      bgColor: 'bg-sky-50',
      iconColor: 'text-sky-600',
    },
    {
      title: 'Monthly Revenue',
      value: formatCurrency(monthlyRevenue),
      icon: TrendingUp,
      bgColor: 'bg-emerald-50',
      iconColor: 'text-emerald-600',
      change: revenueChange,
      isPercentage: true,
    },
  ];

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div>
          <div className="h-8 bg-gray-200 rounded w-64 animate-pulse"></div>
          <div className="h-4 bg-gray-200 rounded w-40 mt-2 animate-pulse"></div>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="bg-white rounded-lg shadow-md p-6 animate-pulse">
              <div className="h-4 bg-gray-200 rounded w-24 mb-4"></div>
              <div className="h-8 bg-gray-200 rounded w-16 mb-2"></div>
              <div className="h-3 bg-gray-200 rounded w-20"></div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">
          Welcome back, {user?.fullName?.split(' ')[0] || 'Admin'}!
        </h1>
        <p className="text-sm text-gray-500 mt-1">Direktor Dashboard</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="hover:shadow-lg transition-shadow cursor-pointer">
              <CardContent className="pt-6">
                <div className="flex items-start justify-between">
                  <div className="space-y-2">
                    <p className="text-sm font-medium text-gray-500">{stat.title}</p>
                    <p className="text-3xl font-bold text-gray-900">
                      {typeof stat.value === 'number' ? stat.value.toLocaleString() : stat.value}
                    </p>
                    {stat.change !== undefined && (
                      <p className="text-sm">
                        <span className={stat.change >= 0 ? 'text-emerald-600' : 'text-red-600'}>
                          {stat.change >= 0 ? '+' : ''}{stat.isPercentage ? `${stat.change}%` : stat.change}
                        </span>
                        {stat.changeText && (
                          <span className="text-gray-500 ml-1">{stat.changeText}</span>
                        )}
                      </p>
                    )}
                  </div>
                  <div className={clsx('p-3 rounded-xl', stat.bgColor)}>
                    <Icon className={clsx('h-6 w-6', stat.iconColor)} />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Recent Activity & Pending Items */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent Activity */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <div>
              <CardTitle className="text-lg font-semibold">Recent Activity</CardTitle>
              <p className="text-sm text-gray-500 mt-1">Your latest actions and updates</p>
            </div>
            <button className="flex items-center text-sm text-blue-600 hover:text-blue-700 font-medium">
              See more
              <ChevronRight className="h-4 w-4 ml-1" />
            </button>
          </CardHeader>
          <CardContent className="pt-4">
            <div className="space-y-1">
              {activities.map((activity) => {
                const Icon = getActivityIcon(activity.type);
                return (
                  <div
                    key={activity.id}
                    className="flex items-center justify-between py-3 hover:bg-gray-50 rounded-lg px-2 -mx-2 transition-colors cursor-pointer"
                  >
                    <div className="flex items-center gap-3">
                      <div className="p-2 bg-gray-100 rounded-lg">
                        <Icon className="h-4 w-4 text-gray-600" />
                      </div>
                      <span className="text-sm text-gray-900">{activity.title}</span>
                    </div>
                    <span className="text-xs text-gray-500">
                      {formatDistanceToNow(new Date(activity.createdAt), { addSuffix: true })}
                    </span>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>

        {/* Pending Items */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <div>
              <CardTitle className="text-lg font-semibold">Pending Items</CardTitle>
              <p className="text-sm text-gray-500 mt-1">Items requiring your attention</p>
            </div>
            <button className="flex items-center text-sm text-blue-600 hover:text-blue-700 font-medium">
              See more
              <ChevronRight className="h-4 w-4 ml-1" />
            </button>
          </CardHeader>
          <CardContent className="pt-4">
            <div className="space-y-1">
              {pendingItems.map((item) => (
                <div
                  key={item.id}
                  className="flex items-center justify-between py-3 hover:bg-gray-50 rounded-lg px-2 -mx-2 transition-colors cursor-pointer"
                >
                  <div className="flex items-center gap-3">
                    <div className={clsx('w-2.5 h-2.5 rounded-full', getPriorityColor(item.priority))} />
                    <span className="text-sm text-gray-900">{item.title}</span>
                  </div>
                  <span className="text-xs text-gray-500">{formatDueDate(item.dueDate)}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Role-specific content */}
      {user?.role === 'Seller' && (
        <Card>
          <CardHeader>
            <CardTitle>Sales Performance</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-3 gap-4">
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <p className="text-sm text-gray-600">This Month</p>
                <p className="text-2xl font-bold text-blue-700 mt-2">{totalContracts}</p>
                <p className="text-xs text-gray-500">Contracts</p>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <p className="text-sm text-gray-600">Revenue</p>
                <p className="text-2xl font-bold text-green-700 mt-2">{formatCurrency(monthlyRevenue)}</p>
                <p className="text-xs text-gray-500">UZS</p>
              </div>
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <p className="text-sm text-gray-600">Pending Orders</p>
                <p className="text-2xl font-bold text-purple-700 mt-2">
                  {dashboardData?.contractStats?.pendingOrders || 12}
                </p>
                <p className="text-xs text-gray-500">Orders</p>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {user?.role === 'ProductionManager' && (
        <Card>
          <CardHeader>
            <CardTitle>Production Overview</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-4 gap-4">
              <div className="text-center p-4 bg-yellow-50 rounded-lg">
                <p className="text-sm text-gray-600">Pending</p>
                <p className="text-2xl font-bold text-yellow-700 mt-2">
                  {dashboardData?.orderStats?.created || 12}
                </p>
              </div>
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <p className="text-sm text-gray-600">In Progress</p>
                <p className="text-2xl font-bold text-blue-700 mt-2">{activeOrders}</p>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <p className="text-sm text-gray-600">Completed</p>
                <p className="text-2xl font-bold text-green-700 mt-2">
                  {dashboardData?.orderStats?.completed || 156}
                </p>
              </div>
              <div className="text-center p-4 bg-gray-50 rounded-lg">
                <p className="text-sm text-gray-600">Total</p>
                <p className="text-2xl font-bold text-gray-700 mt-2">
                  {dashboardData?.orderStats?.totalOrders || 202}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {user?.role === 'WarehouseManager' && (
        <Card>
          <CardHeader>
            <CardTitle>Inventory Status</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 bg-red-50 rounded-lg">
                <div className="flex items-center">
                  <Package className="h-5 w-5 text-red-600 mr-2" />
                  <span className="text-sm font-medium">Low Stock Items</span>
                </div>
                <span className="text-lg font-bold text-red-700">5</span>
              </div>
              <div className="flex items-center justify-between p-3 bg-yellow-50 rounded-lg">
                <div className="flex items-center">
                  <FileText className="h-5 w-5 text-yellow-600 mr-2" />
                  <span className="text-sm font-medium">Pending Requests</span>
                </div>
                <span className="text-lg font-bold text-yellow-700">{pendingItems.length}</span>
              </div>
              <div className="flex items-center justify-between p-3 bg-green-50 rounded-lg">
                <div className="flex items-center">
                  <CheckSquare className="h-5 w-5 text-green-600 mr-2" />
                  <span className="text-sm font-medium">Issued Today</span>
                </div>
                <span className="text-lg font-bold text-green-700">12</span>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
