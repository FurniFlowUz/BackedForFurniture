import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Select } from '@/components/ui/Select';
import {
  FileText,
  Download,
  Calendar,
  BarChart3,
  TrendingUp,
  Package,
  Users,
  DollarSign,
} from 'lucide-react';

type ReportType = 'sales' | 'production' | 'inventory' | 'financial';
type ReportPeriod = 'today' | 'week' | 'month' | 'quarter' | 'year';

export function Reports() {
  const [selectedType, setSelectedType] = useState<ReportType>('sales');
  const [selectedPeriod, setSelectedPeriod] = useState<ReportPeriod>('month');

  const reportTypes = [
    {
      value: 'sales',
      label: 'Sales Report',
      icon: TrendingUp,
      description: 'Contracts and orders overview',
    },
    {
      value: 'production',
      label: 'Production Report',
      icon: Package,
      description: 'Manufacturing tasks and progress',
    },
    {
      value: 'inventory',
      label: 'Inventory Report',
      icon: BarChart3,
      description: 'Materials stock and usage',
    },
    {
      value: 'financial',
      label: 'Financial Report',
      icon: DollarSign,
      description: 'Revenue and payments',
    },
  ];

  const periodOptions = [
    { value: 'today', label: 'Today' },
    { value: 'week', label: 'This Week' },
    { value: 'month', label: 'This Month' },
    { value: 'quarter', label: 'This Quarter' },
    { value: 'year', label: 'This Year' },
  ];

  const handleGenerateReport = () => {
    console.log('Generating report:', { type: selectedType, period: selectedPeriod });
    // Here you would call the API to generate the report
  };

  const handleDownloadReport = (format: 'pdf' | 'excel') => {
    console.log('Downloading report as:', format);
    // Here you would call the API to download the report
  };

  // Sample data for demonstration
  const salesReportData = {
    totalContracts: 24,
    totalOrders: 45,
    totalRevenue: '245,800,000',
    avgContractValue: '10,241,667',
    topClient: 'MebFurniture LLC',
    topProduct: 'Kitchen Cabinets',
  };

  const productionReportData = {
    totalTasks: 156,
    completedTasks: 123,
    inProgressTasks: 20,
    pendingTasks: 13,
    avgCompletionTime: '4.5 days',
    productionEfficiency: '78.8%',
  };

  const inventoryReportData = {
    totalMaterials: 45,
    lowStockItems: 5,
    outOfStock: 2,
    totalValue: '125,400,000',
    mostUsed: 'Plywood 18mm',
    reorderNeeded: 8,
  };

  const financialReportData = {
    totalRevenue: '245,800,000',
    totalPaid: '200,600,000',
    pendingPayments: '45,200,000',
    avgPaymentTime: '18 days',
    collectionRate: '81.6%',
    outstandingBalance: '18,400,000',
  };

  const renderReportContent = () => {
    switch (selectedType) {
      case 'sales':
        return (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="bg-blue-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Contracts</p>
              <p className="text-2xl font-bold text-blue-700 mt-2">{salesReportData.totalContracts}</p>
            </div>
            <div className="bg-green-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Orders</p>
              <p className="text-2xl font-bold text-green-700 mt-2">{salesReportData.totalOrders}</p>
            </div>
            <div className="bg-purple-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Revenue</p>
              <p className="text-2xl font-bold text-purple-700 mt-2">
                {salesReportData.totalRevenue}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-yellow-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Avg. Contract Value</p>
              <p className="text-2xl font-bold text-yellow-700 mt-2">
                {salesReportData.avgContractValue}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-indigo-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Top Client</p>
              <p className="text-lg font-bold text-indigo-700 mt-2">{salesReportData.topClient}</p>
            </div>
            <div className="bg-pink-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Top Product</p>
              <p className="text-lg font-bold text-pink-700 mt-2">{salesReportData.topProduct}</p>
            </div>
          </div>
        );

      case 'production':
        return (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="bg-blue-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Tasks</p>
              <p className="text-2xl font-bold text-blue-700 mt-2">{productionReportData.totalTasks}</p>
            </div>
            <div className="bg-green-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Completed Tasks</p>
              <p className="text-2xl font-bold text-green-700 mt-2">{productionReportData.completedTasks}</p>
            </div>
            <div className="bg-yellow-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">In Progress</p>
              <p className="text-2xl font-bold text-yellow-700 mt-2">{productionReportData.inProgressTasks}</p>
            </div>
            <div className="bg-red-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Pending Tasks</p>
              <p className="text-2xl font-bold text-red-700 mt-2">{productionReportData.pendingTasks}</p>
            </div>
            <div className="bg-purple-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Avg. Completion Time</p>
              <p className="text-2xl font-bold text-purple-700 mt-2">{productionReportData.avgCompletionTime}</p>
            </div>
            <div className="bg-indigo-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Production Efficiency</p>
              <p className="text-2xl font-bold text-indigo-700 mt-2">{productionReportData.productionEfficiency}</p>
            </div>
          </div>
        );

      case 'inventory':
        return (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="bg-blue-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Materials</p>
              <p className="text-2xl font-bold text-blue-700 mt-2">{inventoryReportData.totalMaterials}</p>
            </div>
            <div className="bg-yellow-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Low Stock Items</p>
              <p className="text-2xl font-bold text-yellow-700 mt-2">{inventoryReportData.lowStockItems}</p>
            </div>
            <div className="bg-red-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Out of Stock</p>
              <p className="text-2xl font-bold text-red-700 mt-2">{inventoryReportData.outOfStock}</p>
            </div>
            <div className="bg-green-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Inventory Value</p>
              <p className="text-2xl font-bold text-green-700 mt-2">
                {inventoryReportData.totalValue}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-purple-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Most Used Material</p>
              <p className="text-lg font-bold text-purple-700 mt-2">{inventoryReportData.mostUsed}</p>
            </div>
            <div className="bg-orange-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Reorder Needed</p>
              <p className="text-2xl font-bold text-orange-700 mt-2">{inventoryReportData.reorderNeeded}</p>
            </div>
          </div>
        );

      case 'financial':
        return (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="bg-green-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Revenue</p>
              <p className="text-2xl font-bold text-green-700 mt-2">
                {financialReportData.totalRevenue}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-blue-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Total Paid</p>
              <p className="text-2xl font-bold text-blue-700 mt-2">
                {financialReportData.totalPaid}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-yellow-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Pending Payments</p>
              <p className="text-2xl font-bold text-yellow-700 mt-2">
                {financialReportData.pendingPayments}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
            <div className="bg-purple-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Avg. Payment Time</p>
              <p className="text-2xl font-bold text-purple-700 mt-2">{financialReportData.avgPaymentTime}</p>
            </div>
            <div className="bg-indigo-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Collection Rate</p>
              <p className="text-2xl font-bold text-indigo-700 mt-2">{financialReportData.collectionRate}</p>
            </div>
            <div className="bg-red-50 p-4 rounded-lg">
              <p className="text-sm text-gray-600">Outstanding Balance</p>
              <p className="text-2xl font-bold text-red-700 mt-2">
                {financialReportData.outstandingBalance}
                <span className="text-sm font-normal ml-1">UZS</span>
              </p>
            </div>
          </div>
        );
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Reports & Analytics</h1>
        <p className="mt-2 text-gray-600">
          Generate and download comprehensive reports for your business
        </p>
      </div>

      {/* Report Type Selection */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {reportTypes.map((type) => {
          const Icon = type.icon;
          const isSelected = selectedType === type.value;
          return (
            <button
              key={type.value}
              onClick={() => setSelectedType(type.value as ReportType)}
              className={`p-4 rounded-lg border-2 transition-all text-left ${
                isSelected
                  ? 'border-primary-600 bg-primary-50'
                  : 'border-gray-200 bg-white hover:border-gray-300'
              }`}
            >
              <Icon
                className={`h-8 w-8 mb-2 ${
                  isSelected ? 'text-primary-600' : 'text-gray-400'
                }`}
              />
              <h3 className="font-semibold text-gray-900">{type.label}</h3>
              <p className="text-sm text-gray-600 mt-1">{type.description}</p>
            </button>
          );
        })}
      </div>

      {/* Filters and Actions */}
      <Card>
        <CardHeader>
          <CardTitle>Report Filters</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col md:flex-row gap-4 items-end">
            <div className="flex-1">
              <Select
                label="Time Period"
                value={selectedPeriod}
                onChange={(e) => setSelectedPeriod(e.target.value as ReportPeriod)}
                options={periodOptions}
              />
            </div>
            <div className="flex gap-2">
              <Button onClick={handleGenerateReport} className="flex items-center">
                <FileText className="h-4 w-4 mr-2" />
                Generate Report
              </Button>
              <Button
                onClick={() => handleDownloadReport('pdf')}
                variant="outline"
                className="flex items-center"
              >
                <Download className="h-4 w-4 mr-2" />
                PDF
              </Button>
              <Button
                onClick={() => handleDownloadReport('excel')}
                variant="outline"
                className="flex items-center"
              >
                <Download className="h-4 w-4 mr-2" />
                Excel
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Report Content */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle>
              {reportTypes.find((t) => t.value === selectedType)?.label} -{' '}
              {periodOptions.find((p) => p.value === selectedPeriod)?.label}
            </CardTitle>
            <div className="flex items-center text-sm text-gray-500">
              <Calendar className="h-4 w-4 mr-1" />
              Generated on: {new Date().toLocaleDateString()}
            </div>
          </div>
        </CardHeader>
        <CardContent>{renderReportContent()}</CardContent>
      </Card>
    </div>
  );
}
