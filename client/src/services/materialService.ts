import { apiClient } from './api';
import {
  Material,
  CreateMaterialDto,
  MaterialRequest,
  CreateMaterialRequestDto,
  IssueMaterialDto,
  PaginatedResponse
} from '@/types';

export const materialService = {
  async getAll(page = 1, pageSize = 10): Promise<PaginatedResponse<Material>> {
    return apiClient.get(`/materials?page=${page}&pageSize=${pageSize}`);
  },

  async getById(id: string): Promise<Material> {
    return apiClient.get(`/materials/${id}`);
  },

  async create(data: CreateMaterialDto): Promise<Material> {
    return apiClient.post('/materials', data);
  },

  async update(id: string, data: Partial<CreateMaterialDto>): Promise<Material> {
    return apiClient.put(`/materials/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    return apiClient.delete(`/materials/${id}`);
  },

  async getLowStock(): Promise<Material[]> {
    return apiClient.get('/materials/low-stock');
  },
};

export const materialRequestService = {
  async getAll(page = 1, pageSize = 10): Promise<PaginatedResponse<MaterialRequest>> {
    return apiClient.get(`/material-requests?page=${page}&pageSize=${pageSize}`);
  },

  async getById(id: string): Promise<MaterialRequest> {
    return apiClient.get(`/material-requests/${id}`);
  },

  async create(data: CreateMaterialRequestDto): Promise<MaterialRequest> {
    return apiClient.post('/material-requests', data);
  },

  async approve(id: string): Promise<MaterialRequest> {
    return apiClient.post(`/material-requests/${id}/approve`);
  },

  async reject(id: string): Promise<MaterialRequest> {
    return apiClient.post(`/material-requests/${id}/reject`);
  },

  async issue(id: string, data: IssueMaterialDto): Promise<MaterialRequest> {
    return apiClient.post(`/material-requests/${id}/issue`, data);
  },

  async getPending(): Promise<MaterialRequest[]> {
    return apiClient.get('/material-requests/pending');
  },

  async getByTask(taskId: string): Promise<MaterialRequest[]> {
    return apiClient.get(`/material-requests/task/${taskId}`);
  },
};
