import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';
import { DollarSign, TrendingUp, TrendingDown, CreditCard, Wallet } from 'lucide-react';

export function Finance() {
  const financialStats = [
    {
      title: 'Total Revenue',
      value: '245.8M',
      currency: 'UZS',
      icon: DollarSign,
      color: 'bg-green-500',
      change: '+15.3%',
      trend: 'up',
    },
    {
      title: 'Pending Payments',
      value: '45.2M',
      currency: 'UZS',
      icon: CreditCard,
      color: 'bg-yellow-500',
      change: '-8.1%',
      trend: 'down',
    },
    {
      title: 'Paid Amount',
      value: '200.6M',
      currency: 'UZS',
      icon: Wallet,
      color: 'bg-blue-500',
      change: '+12.5%',
      trend: 'up',
    },
    {
      title: 'Outstanding Balance',
      value: '18.4M',
      currency: 'UZS',
      icon: TrendingDown,
      color: 'bg-red-500',
      change: '+3.2%',
      trend: 'up',
    },
  ];

  const recentTransactions = [
    {
      id: 'TRX-001',
      contractNumber: 'CTR-2025-001',
      client: 'MebFurniture LLC',
      amount: '15,500,000',
      type: 'Payment Received',
      date: '2025-01-18',
      status: 'Completed',
    },
    {
      id: 'TRX-002',
      contractNumber: 'CTR-2025-002',
      client: 'InteriorDesign Co',
      amount: '8,200,000',
      type: 'Advance Payment',
      date: '2025-01-17',
      status: 'Completed',
    },
    {
      id: 'TRX-003',
      contractNumber: 'CTR-2025-003',
      client: 'HomeStyle LLC',
      amount: '22,300,000',
      type: 'Final Payment',
      date: '2025-01-16',
      status: 'Pending',
    },
  ];

  const contractFinancials = [
    {
      contractNumber: 'CTR-2025-001',
      totalAmount: '25,000,000',
      advancePayment: '7,500,000',
      paidAmount: '15,500,000',
      remainingBalance: '9,500,000',
      status: 'Active',
    },
    {
      contractNumber: 'CTR-2025-002',
      totalAmount: '18,000,000',
      advancePayment: '5,400,000',
      paidAmount: '8,200,000',
      remainingBalance: '9,800,000',
      status: 'Active',
    },
    {
      contractNumber: 'CTR-2025-003',
      totalAmount: '32,000,000',
      advancePayment: '9,600,000',
      paidAmount: '22,300,000',
      remainingBalance: '9,700,000',
      status: 'Completed',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Financial Management</h1>
        <p className="mt-2 text-gray-600">
          Track payments, revenue, and contract financial details
        </p>
      </div>

      {/* Financial Stats */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {financialStats.map((stat) => {
          const Icon = stat.icon;
          const TrendIcon = stat.trend === 'up' ? TrendingUp : TrendingDown;
          return (
            <Card key={stat.title}>
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">{stat.title}</p>
                    <p className="text-2xl font-bold text-gray-900 mt-2">
                      {stat.value}
                      <span className="text-sm font-normal text-gray-500 ml-1">{stat.currency}</span>
                    </p>
                    <div className="flex items-center mt-1">
                      <TrendIcon
                        className={`h-4 w-4 ${
                          stat.trend === 'up' ? 'text-green-600' : 'text-red-600'
                        }`}
                      />
                      <span
                        className={`text-xs ml-1 ${
                          stat.trend === 'up' ? 'text-green-600' : 'text-red-600'
                        }`}
                      >
                        {stat.change} from last month
                      </span>
                    </div>
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

      {/* Recent Transactions */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Transactions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Transaction ID
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Contract
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Client
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Amount (UZS)
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Type
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Date
                  </th>
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody>
                {recentTransactions.map((transaction) => (
                  <tr key={transaction.id} className="border-b border-gray-100 hover:bg-gray-50">
                    <td className="py-3 px-4 text-sm text-gray-900">{transaction.id}</td>
                    <td className="py-3 px-4 text-sm text-gray-900">
                      {transaction.contractNumber}
                    </td>
                    <td className="py-3 px-4 text-sm text-gray-900">{transaction.client}</td>
                    <td className="py-3 px-4 text-sm font-medium text-gray-900">
                      {transaction.amount}
                    </td>
                    <td className="py-3 px-4 text-sm text-gray-600">{transaction.type}</td>
                    <td className="py-3 px-4 text-sm text-gray-600">{transaction.date}</td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-2 py-1 text-xs font-semibold rounded-full ${
                          transaction.status === 'Completed'
                            ? 'bg-green-100 text-green-800'
                            : 'bg-yellow-100 text-yellow-800'
                        }`}
                      >
                        {transaction.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      {/* Contract Financial Details */}
      <Card>
        <CardHeader>
          <CardTitle>Contract Financial Overview</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 text-sm font-semibold text-gray-700">
                    Contract
                  </th>
                  <th className="text-right py-3 px-4 text-sm font-semibold text-gray-700">
                    Total Amount
                  </th>
                  <th className="text-right py-3 px-4 text-sm font-semibold text-gray-700">
                    Advance (30%)
                  </th>
                  <th className="text-right py-3 px-4 text-sm font-semibold text-gray-700">
                    Paid Amount
                  </th>
                  <th className="text-right py-3 px-4 text-sm font-semibold text-gray-700">
                    Remaining Balance
                  </th>
                  <th className="text-center py-3 px-4 text-sm font-semibold text-gray-700">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody>
                {contractFinancials.map((contract) => {
                  const paidPercentage = (
                    (parseInt(contract.paidAmount.replace(/,/g, '')) /
                      parseInt(contract.totalAmount.replace(/,/g, ''))) *
                    100
                  ).toFixed(0);

                  return (
                    <tr key={contract.contractNumber} className="border-b border-gray-100 hover:bg-gray-50">
                      <td className="py-3 px-4 text-sm font-medium text-gray-900">
                        {contract.contractNumber}
                      </td>
                      <td className="py-3 px-4 text-sm text-right text-gray-900">
                        {contract.totalAmount}
                      </td>
                      <td className="py-3 px-4 text-sm text-right text-gray-600">
                        {contract.advancePayment}
                      </td>
                      <td className="py-3 px-4 text-sm text-right font-medium text-green-700">
                        {contract.paidAmount}
                        <span className="ml-1 text-xs text-gray-500">({paidPercentage}%)</span>
                      </td>
                      <td className="py-3 px-4 text-sm text-right font-medium text-red-700">
                        {contract.remainingBalance}
                      </td>
                      <td className="py-3 px-4 text-center">
                        <span
                          className={`px-2 py-1 text-xs font-semibold rounded-full ${
                            contract.status === 'Completed'
                              ? 'bg-green-100 text-green-800'
                              : 'bg-blue-100 text-blue-800'
                          }`}
                        >
                          {contract.status}
                        </span>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
