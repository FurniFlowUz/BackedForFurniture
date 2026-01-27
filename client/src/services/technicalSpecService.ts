import { apiClient } from './api';
import { TechnicalSpecification, CreateTechnicalSpecificationDto } from '@/types';

export const technicalSpecService = {
  async getByOrder(orderId: string): Promise<TechnicalSpecification | null> {
    try {
      return await apiClient.get(`/technical-specifications/order/${orderId}`);
    } catch {
      return null;
    }
  },

  async create(data: CreateTechnicalSpecificationDto): Promise<TechnicalSpecification> {
    return apiClient.post('/technical-specifications', data);
  },

  async update(id: string, data: Partial<CreateTechnicalSpecificationDto>): Promise<TechnicalSpecification> {
    return apiClient.put(`/technical-specifications/${id}`, data);
  },

  async uploadDrawing(id: string, file: File, onProgress?: (progress: number) => void): Promise<TechnicalSpecification> {
    return apiClient.uploadFile(`/technical-specifications/${id}/drawing`, file, onProgress);
  },

  async deleteDrawing(id: string): Promise<void> {
    return apiClient.delete(`/technical-specifications/${id}/drawing`);
  },
};
