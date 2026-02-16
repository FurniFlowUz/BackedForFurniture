import { apiClient } from './api';
import {
  DirectorDashboardDto,
  ContractStatsDto,
  OrderStatsDto,
  RecentActivityDto,
  PendingItemDto,
  EmployeeDto,
  ApiResponse,
} from '@/types';

class DashboardService {
  // Director Dashboard - main stats
  async getDirectorDashboard(): Promise<DirectorDashboardDto> {
    const response = await apiClient.get<ApiResponse<DirectorDashboardDto>>('/director/dashboard');
    return response.data;
  }

  // Contract statistics
  async getContractStats(): Promise<ContractStatsDto> {
    const response = await apiClient.get<ApiResponse<ContractStatsDto>>('/contracts/stats');
    return response.data;
  }

  // Order statistics
  async getOrderStats(): Promise<OrderStatsDto> {
    const response = await apiClient.get<ApiResponse<OrderStatsDto>>('/orders/stats');
    return response.data;
  }

  // Recent activities
  async getRecentActivities(limit: number = 10): Promise<RecentActivityDto[]> {
    const response = await apiClient.get<ApiResponse<RecentActivityDto[]>>(
      `/seller/activities?limit=${limit}`
    );
    return response.data;
  }

  // Pending items
  async getPendingItems(): Promise<PendingItemDto[]> {
    const response = await apiClient.get<ApiResponse<PendingItemDto[]>>('/seller/pending-items');
    return response.data;
  }

  // Employees count
  async getEmployees(): Promise<EmployeeDto[]> {
    const response = await apiClient.get<ApiResponse<EmployeeDto[]>>('/employees');
    return response.data;
  }

  // Combined dashboard data for Director
  async getDashboardData() {
    const [contractStats, orderStats, activities, pendingItems, employees] = await Promise.all([
      this.getContractStats().catch(() => null),
      this.getOrderStats().catch(() => null),
      this.getRecentActivities(5).catch(() => []),
      this.getPendingItems().catch(() => []),
      this.getEmployees().catch(() => []),
    ]);

    return {
      contractStats,
      orderStats,
      activities,
      pendingItems,
      employeesCount: employees.length,
    };
  }
}

export const dashboardService = new DashboardService();
