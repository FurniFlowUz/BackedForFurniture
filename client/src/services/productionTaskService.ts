import { apiClient } from './api';
import {
  ProductionTask,
  CreateProductionTaskDto,
  AssignTaskDto,
  UpdateTaskStatusDto,
  CreateTaskDependencyDto,
  TaskDependency,
  PaginatedResponse
} from '@/types';

export const productionTaskService = {
  async getAll(page = 1, pageSize = 10): Promise<PaginatedResponse<ProductionTask>> {
    return apiClient.get(`/production-tasks?page=${page}&pageSize=${pageSize}`);
  },

  async getById(id: string): Promise<ProductionTask> {
    return apiClient.get(`/production-tasks/${id}`);
  },

  async create(data: CreateProductionTaskDto): Promise<ProductionTask> {
    return apiClient.post('/production-tasks', data);
  },

  async update(id: string, data: Partial<CreateProductionTaskDto>): Promise<ProductionTask> {
    return apiClient.put(`/production-tasks/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    return apiClient.delete(`/production-tasks/${id}`);
  },

  async getByOrder(orderId: string): Promise<ProductionTask[]> {
    return apiClient.get(`/production-tasks/order/${orderId}`);
  },

  async assign(id: string, data: AssignTaskDto): Promise<ProductionTask> {
    return apiClient.post(`/production-tasks/${id}/assign`, data);
  },

  async updateStatus(id: string, data: UpdateTaskStatusDto): Promise<ProductionTask> {
    return apiClient.patch(`/production-tasks/${id}/status`, data);
  },

  async getMyTasks(): Promise<ProductionTask[]> {
    return apiClient.get('/production-tasks/my');
  },

  async canStart(id: string): Promise<boolean> {
    const result = await apiClient.get<{ canStart: boolean }>(`/production-tasks/${id}/can-start`);
    return result.canStart;
  },

  async addDependency(data: CreateTaskDependencyDto): Promise<TaskDependency> {
    return apiClient.post('/production-tasks/dependencies', data);
  },

  async removeDependency(id: string): Promise<void> {
    return apiClient.delete(`/production-tasks/dependencies/${id}`);
  },
};
