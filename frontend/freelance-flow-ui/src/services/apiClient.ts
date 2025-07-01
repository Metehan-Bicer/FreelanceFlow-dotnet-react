import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { ApiResponse } from '../types';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5000/api',
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor to add auth token
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor to handle errors
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  async get<T>(url: string, config?: any, responseType?: string): Promise<ApiResponse<T>> {
    try {
      const requestConfig = {
        ...config,
        ...(responseType && { responseType }),
      };
      const response: AxiosResponse<any> = await this.client.get(url, requestConfig);
      
      console.log('ApiClient GET response:', response.data); // Debug log
      
      // Backend zaten ApiResponse formatında döndürüyor
      if (response.data && typeof response.data === 'object' && 'success' in response.data) {
        return response.data as ApiResponse<T>;
      }
      
      // Eski format için fallback
      return {
        success: true,
        data: response.data,
      };
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  async post<T>(url: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<any> = await this.client.post(url, data);
      
      console.log('ApiClient POST response:', response.data); // Debug log
      
      // Backend zaten ApiResponse formatında döndürüyor
      if (response.data && typeof response.data === 'object' && 'success' in response.data) {
        return response.data as ApiResponse<T>;
      }
      
      // Eski format için fallback
      return {
        success: true,
        data: response.data,
      };
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  async put<T>(url: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<any> = await this.client.put(url, data);
      
      console.log('ApiClient PUT response:', response.data); // Debug log
      
      // Backend zaten ApiResponse formatında döndürüyor
      if (response.data && typeof response.data === 'object' && 'success' in response.data) {
        return response.data as ApiResponse<T>;
      }
      
      // Eski format için fallback
      return {
        success: true,
        data: response.data,
      };
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  async delete<T>(url: string): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<any> = await this.client.delete(url);
      
      console.log('ApiClient DELETE response:', response.data); // Debug log
      
      // Backend zaten ApiResponse formatında döndürüyor
      if (response.data && typeof response.data === 'object' && 'success' in response.data) {
        return response.data as ApiResponse<T>;
      }
      
      // Eski format için fallback
      return {
        success: true,
        data: response.data,
      };
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  private handleError(error: any): ApiResponse<any> {
    console.error('API Error:', error);
    
    // Backend'den gelen hata mesajlarını kontrol et
    let errorMessage = 'Bir hata oluştu';
    
    if (error.response?.data) {
      // Backend validation errors (array format)
      if (Array.isArray(error.response.data)) {
        errorMessage = error.response.data.join(', ');
      }
      // Backend string errors
      else if (typeof error.response.data === 'string') {
        errorMessage = error.response.data;
      }
      // Backend object errors
      else if (error.response.data.error) {
        errorMessage = error.response.data.error;
      }
      else if (error.response.data.message) {
        errorMessage = error.response.data.message;
      }
      // Model validation errors
      else if (error.response.data.errors) {
        const errors = error.response.data.errors;
        errorMessage = Object.values(errors).flat().join(', ');
      }
    }
    // Network errors
    else if (error.message) {
      errorMessage = error.message;
    }
    
    return {
      success: false,
      error: errorMessage,
    };
  }
}

export const apiClient = new ApiClient();