import { apiClient } from './apiClient';
import { 
  Client, 
  CreateClientRequest, 
  UpdateClientRequest,
  ApiResponse 
} from '../types';

export class ClientService {
  async getAll(): Promise<ApiResponse<Client[]>> {
    return await apiClient.get<Client[]>('/clients');
  }

  async getActive(): Promise<ApiResponse<Client[]>> {
    return await apiClient.get<Client[]>('/clients/active');
  }

  async getById(id: string): Promise<ApiResponse<Client>> {
    return await apiClient.get<Client>(`/clients/${id}`);
  }

  async create(clientData: CreateClientRequest): Promise<ApiResponse<{ id: string }>> {
    return await apiClient.post<{ id: string }>('/clients', clientData);
  }

  async update(id: string, clientData: UpdateClientRequest): Promise<ApiResponse<Client>> {
    return await apiClient.put<Client>(`/clients/${id}`, clientData);
  }

  async updateStatus(id: string, status: 'Active' | 'Inactive' | 'Suspended'): Promise<ApiResponse<Client>> {
    return await apiClient.put<Client>(`/clients/${id}/status`, { status });
  }

  async delete(id: string): Promise<ApiResponse<string>> {
    return await apiClient.delete<string>(`/clients/${id}`);
  }

  async getClientStats(id: string): Promise<ApiResponse<any>> {
    return await apiClient.get<any>(`/clients/${id}/stats`);
  }
}

export const clientService = new ClientService();