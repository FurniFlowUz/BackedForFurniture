import { apiClient } from './api';
import { Contract, CreateContractDto, PaginatedResponse } from '@/types';

export const contractService = {
  async getAll(page = 1, pageSize = 10): Promise<PaginatedResponse<Contract>> {
    return apiClient.get(`/contracts?page=${page}&pageSize=${pageSize}`);
  },

  async getById(id: string): Promise<Contract> {
    return apiClient.get(`/contracts/${id}`);
  },

  async create(data: CreateContractDto): Promise<Contract> {
    return apiClient.post('/contracts', data);
  },

  async update(id: string, data: Partial<CreateContractDto>): Promise<Contract> {
    return apiClient.put(`/contracts/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    return apiClient.delete(`/contracts/${id}`);
  },

  async getMyContracts(page = 1, pageSize = 10): Promise<PaginatedResponse<Contract>> {
    return apiClient.get(`/contracts/my?page=${page}&pageSize=${pageSize}`);
  },
};
