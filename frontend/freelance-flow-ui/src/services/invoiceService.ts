import { apiClient } from './apiClient';
import { 
  Invoice, 
  CreateInvoiceRequest, 
  UpdateInvoiceRequest,
  ApiResponse 
} from '../types';

export class InvoiceService {
  async getAll(): Promise<ApiResponse<Invoice[]>> {
    return await apiClient.get<Invoice[]>('/invoices');
  }

  async getById(id: string): Promise<ApiResponse<Invoice>> {
    return await apiClient.get<Invoice>(`/invoices/${id}`);
  }

  async create(invoiceData: CreateInvoiceRequest): Promise<ApiResponse<{ id: string }>> {
    return await apiClient.post<{ id: string }>('/invoices', invoiceData);
  }

  async update(id: string, invoiceData: UpdateInvoiceRequest): Promise<ApiResponse<Invoice>> {
    return await apiClient.put<Invoice>(`/invoices/${id}`, invoiceData);
  }

  async delete(id: string): Promise<ApiResponse<string>> {
    return await apiClient.delete<string>(`/invoices/${id}`);
  }

  async markAsPaid(id: string): Promise<ApiResponse<string>> {
    return await apiClient.put<string>(`/invoices/${id}/payment-status`, { 
      paymentStatus: 2, // Paid = 2 (backend enum değeri)
      paidAt: new Date().toISOString()
    });
  }

  async updatePaymentStatus(id: string, status: 'Pending' | 'Paid' | 'PartiallyPaid' | 'Overdue' | 'Cancelled', paidAt?: string): Promise<ApiResponse<string>> {
    // String değerleri backend enum integer değerlerine çevir
    const statusMap = {
      'Pending': 1,
      'Paid': 2,
      'PartiallyPaid': 3,
      'Overdue': 4,
      'Cancelled': 5
    };

    return await apiClient.put<string>(`/invoices/${id}/payment-status`, { 
      paymentStatus: statusMap[status],
      paidAt: paidAt || (status === 'Paid' ? new Date().toISOString() : null)
    });
  }

  async sendByEmail(id: string): Promise<ApiResponse<string>> {
    return await apiClient.post<string>(`/invoices/${id}/send-email`, {});
  }

  async downloadPdf(id: string): Promise<ApiResponse<Blob>> {
    try {
      // Base URL zaten /api içeriyor, tekrar eklemeye gerek yok
      const baseUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000';
      const apiUrl = baseUrl.endsWith('/api') ? baseUrl : `${baseUrl}/api`;
      
      const response = await fetch(`${apiUrl}/invoices/${id}/pdf`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
        },
      });

      if (!response.ok) {
        return { success: false, error: 'PDF indirilemedi' };
      }

      const blob = await response.blob();
      return { success: true, data: blob };
    } catch (error) {
      return { success: false, error: 'PDF indirilemedi' };
    }
  }

  async getByClientId(clientId: string): Promise<ApiResponse<Invoice[]>> {
    return await apiClient.get<Invoice[]>(`/invoices/client/${clientId}`);
  }

  async getByProjectId(projectId: string): Promise<ApiResponse<Invoice[]>> {
    return await apiClient.get<Invoice[]>(`/invoices/project/${projectId}`);
  }
}

export const invoiceService = new InvoiceService();