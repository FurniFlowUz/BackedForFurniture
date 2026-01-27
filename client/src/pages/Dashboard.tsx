import { useAuth } from '@/contexts/AuthContext';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';
import {
  FileText,
  ShoppingCart,
  ClipboardList,
  Package,
  TrendingUp,
  DollarSign,
  CheckCircle,
  Clock,
} from 'lucide-react';

export function Dashboard() {
  const { user } = useAuth();

  const stats = [
    {
      title: 'Total Contracts',
      value: '24',
      icon: FileText,
      color: 'bg-blue-500',
      change: '+12%',
    },
    {
      title: 'Active Orders',
      value: '18',
      icon: ShoppingCart,
      color: 'bg-green-500',
      change: '+8%',
    },
    {
      title: 'Pending Tasks',
      value: '32',
      icon: Clock,
      color: 'bg-yellow-500',
      change: '-5%',
    },
    {
      title: 'Completed Tasks',
      value: '156',
      icon: CheckCircle,
      color: 'bg-purple-500',
      change: '+23%',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          Welcome back, {user?.fullName}!
        </h1>
        <p className="mt-2 text-gray-600">
          Here's what's happening in your furniture manufacturing workflow
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => {
          const Icon = stat.icon;
          return (
            <Card key={stat.title}>
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">{stat.title}</p>
                    <p className="text-2xl font-bold text-gray-900 mt-2">{stat.value}</p>
                    <p className="text-xs text-gray-500 mt-1">
                      <span className={stat.change.startsWith('+') ? 'text-green-600' : 'text-red-600'}>
                        {stat.change}
                      </span>{' '}
                      from last month
                    </p>
                  </div>
                  <div className={`${stat.color} p-3 rounded-lg`}>
                    <Icon className="h-6 w-6 text-white" />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Recent Activity */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Recent Contracts</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {[1, 2, 3].map((i) => (
                <div key={i} className="flex items-center justify-between py-3 border-b last:border-0">
                  <div>
                    <p className="font-medium text-gray-900">Contract #CTR-2025-00{i}</p>
                    <p className="text-sm text-gray-500">Client Name - 2025-01-{15 + i}</p>
                  </div>
                  <span className="px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">
                    Active
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Recent Orders</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {[1, 2, 3].map((i) => (
                <div key={i} className="flex items-center justify-between py-3 border-b last:border-0">
                  <div>
                    <p className="font-medium text-gray-900">Order #ORD-2025-00{i}</p>
                    <p className="text-sm text-gray-500">Kitchen Cabinet - Qty: {i * 2}</p>
                  </div>
                  <span className="px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 text-blue-800">
                    In Production
                  </span>
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
                <p className="text-2xl font-bold text-blue-700 mt-2">8</p>
                <p className="text-xs text-gray-500">Contracts</p>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <p className="text-sm text-gray-600">Revenue</p>
                <p className="text-2xl font-bold text-green-700 mt-2">45.2M</p>
                <p className="text-xs text-gray-500">UZS</p>
              </div>
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <p className="text-sm text-gray-600">Avg. Deal Size</p>
                <p className="text-2xl font-bold text-purple-700 mt-2">5.6M</p>
                <p className="text-xs text-gray-500">UZS</p>
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
                <p className="text-2xl font-bold text-yellow-700 mt-2">12</p>
              </div>
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <p className="text-sm text-gray-600">In Progress</p>
                <p className="text-2xl font-bold text-blue-700 mt-2">20</p>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <p className="text-sm text-gray-600">Completed</p>
                <p className="text-2xl font-bold text-green-700 mt-2">156</p>
              </div>
              <div className="text-center p-4 bg-red-50 rounded-lg">
                <p className="text-sm text-gray-600">Blocked</p>
                <p className="text-2xl font-bold text-red-700 mt-2">3</p>
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
                  <Clock className="h-5 w-5 text-yellow-600 mr-2" />
                  <span className="text-sm font-medium">Pending Requests</span>
                </div>
                <span className="text-lg font-bold text-yellow-700">8</span>
              </div>
              <div className="flex items-center justify-between p-3 bg-green-50 rounded-lg">
                <div className="flex items-center">
                  <CheckCircle className="h-5 w-5 text-green-600 mr-2" />
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
