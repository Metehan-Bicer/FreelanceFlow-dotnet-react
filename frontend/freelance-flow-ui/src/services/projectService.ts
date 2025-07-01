import { apiClient } from './apiClient';
import { 
  Project, 
  CreateProjectRequest, 
  UpdateProjectRequest,
  ApiResponse 
} from '../types';

export class ProjectService {
  async getAll(): Promise<ApiResponse<Project[]>> {
    return await apiClient.get<Project[]>('/projects');
  }

  async getActive(): Promise<ApiResponse<Project[]>> {
    return await apiClient.get<Project[]>('/projects/active');
  }

  async getById(id: string): Promise<ApiResponse<Project>> {
    return await apiClient.get<Project>(`/projects/${id}`);
  }

  async create(projectData: CreateProjectRequest): Promise<ApiResponse<{ id: string }>> {
    return await apiClient.post<{ id: string }>('/projects', projectData);
  }

  async update(id: string, projectData: UpdateProjectRequest): Promise<ApiResponse<Project>> {
    return await apiClient.put<Project>(`/projects/${id}`, projectData);
  }

  async updateActiveStatus(id: string, isActive: boolean): Promise<ApiResponse<string>> {
    return await apiClient.put<string>(`/projects/${id}/active-status`, { isActive });
  }

  async delete(id: string): Promise<ApiResponse<string>> {
    return await apiClient.delete<string>(`/projects/${id}`);
  }

  async getByClientId(clientId: string): Promise<ApiResponse<Project[]>> {
    return await apiClient.get<Project[]>(`/projects/client/${clientId}`);
  }

  async getProjectStats(id: string): Promise<ApiResponse<any>> {
    return await apiClient.get<any>(`/projects/${id}/stats`);
  }
}

export const projectService = new ProjectService();