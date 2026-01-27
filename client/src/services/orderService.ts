import { apiClient } from './api';
import { Order, CreateOrderDto, PaginatedResponse } from '@/types';

export const orderService = {
  async getAll(page = 1, pageSize = 10): Promise<PaginatedResponse<Order>> {
    return apiClient.get(`/orders?page=${page}&pageSize=${pageSize}`);
  },

  async getById(id: string): Promise<Order> {
    return apiClient.get(`/orders/${id}`);
  },

  async create(data: CreateOrderDto): Promise<Order> {
    return apiClient.post('/orders', data);
  },

  async update(id: string, data: Partial<CreateOrderDto>): Promise<Order> {
    return apiClient.put(`/orders/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    return apiClient.delete(`/orders/${id}`);
  },

  async getByContract(contractId: string): Promise<Order[]> {
    return apiClient.get(`/orders/contract/${contractId}`);
  },
};
