import React, { useState, useEffect } from 'react';
import { Plus, Search, Edit, Trash2, FileText, Send, Download, CheckCircle } from 'lucide-react';
import { DashboardLayout } from '../components/layout/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Badge } from '../components/ui/Badge';
import { Loading } from '../components/ui/Loading';
import { InvoiceModal } from '../components/modals/InvoiceModal';
import { invoiceService } from '../services/invoiceService';
import { Invoice, InvoiceStatus, PaymentStatus } from '../types';
import { formatCurrency, formatDate } from '../utils';

const InvoicesPage: React.FC = () => {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState('');
  const [filterStatus, setFilterStatus] = useState<PaymentStatus | 'All'>('All');
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    loadInvoices();
  }, []);

  const loadInvoices = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await invoiceService.getAll();
      if (response.success && response.data) {
        setInvoices(response.data);
      } else {
        setError(response.error || 'Faturalar yüklenirken hata oluştu');
      }
    } catch (err) {
      setError('Faturalar yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu faturayı silmek istediğinizden emin misiniz?')) {
      try {
        const response = await invoiceService.delete(id);
        if (response.success) {
          setInvoices(invoices.filter(invoice => invoice.id !== id));
        } else {
          setError('Fatura silinirken hata oluştu');
        }
      } catch (err) {
        setError('Fatura silinirken hata oluştu');
      }
    }
  };

  const handleMarkAsPaid = async (id: string) => {
    try {
      const response = await invoiceService.markAsPaid(id);
      if (response.success) {
        loadInvoices(); // Reload invoices
      } else {
        setError('Fatura durumu güncellenirken hata oluştu');
      }
    } catch (err) {
      setError('Fatura durumu güncellenirken hata oluştu');
    }
  };

  const handleSendEmail = async (id: string) => {
    try {
      const response = await invoiceService.sendByEmail(id);
      if (response.success) {
        alert('Fatura başarıyla gönderildi');
        loadInvoices(); // Reload invoices
      } else {
        setError('Fatura gönderilirken hata oluştu');
      }
    } catch (err) {
      setError('Fatura gönderilirken hata oluştu');
    }
  };

  const handleDownloadPdf = async (id: string, invoiceNumber: string) => {
    try {
      const response = await invoiceService.downloadPdf(id);
      if (response.success && response.data) {
        // Create blob and download
        const blob = new Blob([response.data], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Fatura_${invoiceNumber}.pdf`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      } else {
        setError('PDF indirilirken hata oluştu');
      }
    } catch (err) {
      setError('PDF indirilirken hata oluştu');
    }
  };

  const handleCreateSuccess = () => {
    loadInvoices(); // Reload invoices after successful creation
  };

  const getPaymentStatusBadgeVariant = (status: PaymentStatus) => {
    switch (status) {
      case 'Paid':
        return 'success';
      case 'Pending':
        return 'warning';
      case 'Overdue':
        return 'error';
      default:
        return 'default';
    }
  };

  const getPaymentStatusText = (status: PaymentStatus) => {
    switch (status) {
      case 'Paid':
        return 'Ödendi';
      case 'Pending':
        return 'Beklemede';
      case 'Overdue':
        return 'Gecikmiş';
      case 'PartiallyPaid':
        return 'Kısmi Ödendi';
      default:
        return status;
    }
  };

  const getInvoiceStatusText = (status: InvoiceStatus) => {
    switch (status) {
      case 'Draft':
        return 'Taslak';
      case 'Sent':
        return 'Gönderildi';
      case 'Paid':
        return 'Ödendi';
      case 'Overdue':
        return 'Gecikmiş';
      case 'Cancelled':
        return 'İptal Edildi';
      default:
        return status;
    }
  };

  let filteredInvoices = invoices.filter(invoice =>
    invoice.invoiceNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    invoice.clientName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (filterStatus !== 'All') {
    filteredInvoices = filteredInvoices.filter(invoice => invoice.paymentStatus === filterStatus);
  }

  if (loading) {
    return (
      <DashboardLayout title="Faturalar">
        <div className="flex items-center justify-center h-64">
          <Loading size="lg" text="Faturalar yükleniyor..." />
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout title="Faturalar">
      <div className="space-y-6">
        {/* Header Actions */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div className="flex flex-col sm:flex-row gap-4 flex-1">
            <div className="relative flex-1 max-w-md">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-secondary-400" size={16} />
              <Input
                type="text"
                placeholder="Fatura ara..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value as PaymentStatus | 'All')}
              className="px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              <option value="All">Tüm Durumlar</option>
              <option value="Pending">Beklemede</option>
              <option value="Paid">Ödendi</option>
              <option value="Overdue">Gecikmiş</option>
              <option value="PartiallyPaid">Kısmi Ödendi</option>
            </select>
          </div>
          <Button onClick={() => setShowCreateModal(true)}>
            <Plus size={16} className="mr-2" />
            Yeni Fatura
          </Button>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-md p-4">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Invoices Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {filteredInvoices.map((invoice) => (
            <Card key={invoice.id}>
              <CardHeader className="pb-3">
                <div className="flex items-start justify-between">
                  <div>
                    <CardTitle className="text-lg mb-2">
                      <div className="flex items-center space-x-2">
                        <FileText size={20} />
                        <span>{invoice.invoiceNumber}</span>
                      </div>
                    </CardTitle>
                    <div className="flex space-x-2">
                      <Badge variant={getPaymentStatusBadgeVariant(invoice.paymentStatus)} size="sm">
                        {getPaymentStatusText(invoice.paymentStatus)}
                      </Badge>
                      <Badge variant="info" size="sm">
                        {getInvoiceStatusText(invoice.status)}
                      </Badge>
                    </div>
                  </div>
                  <div className="flex space-x-1">
                    <Button variant="ghost" size="sm">
                      <Edit size={16} />
                    </Button>
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleDelete(invoice.id)}
                    >
                      <Trash2 size={16} />
                    </Button>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  <div className="text-sm text-secondary-600">
                    <strong>Müşteri:</strong> {invoice.clientName}
                  </div>
                  
                  {invoice.projectName && (
                    <div className="text-sm text-secondary-600">
                      <strong>Proje:</strong> {invoice.projectName}
                    </div>
                  )}
                  
                  <div className="text-sm text-secondary-600">
                    <strong>Düzenleme:</strong> {formatDate(invoice.issueDate, 'short')}
                  </div>
                  
                  <div className="text-sm text-secondary-600">
                    <strong>Vade:</strong> {formatDate(invoice.dueDate, 'short')}
                  </div>

                  <div className="pt-3 border-t border-secondary-200">
                    <div className="text-2xl font-bold text-primary-600">
                      {formatCurrency(invoice.totalAmount)}
                    </div>
                    <div className="text-sm text-secondary-600">
                      KDV: {formatCurrency(invoice.taxAmount)}
                    </div>
                  </div>

                  {/* Actions */}
                  <div className="pt-3 border-t border-secondary-200 space-y-2">
                    <div className="flex space-x-2">
                      <Button 
                        size="sm" 
                        variant="outline" 
                        className="flex-1"
                        onClick={() => handleDownloadPdf(invoice.id, invoice.invoiceNumber)}
                      >
                        <Download size={14} className="mr-1" />
                        PDF
                      </Button>
                      <Button 
                        size="sm" 
                        variant="outline" 
                        className="flex-1"
                        onClick={() => handleSendEmail(invoice.id)}
                      >
                        <Send size={14} className="mr-1" />
                        Gönder
                      </Button>
                    </div>
                    
                    {invoice.paymentStatus !== 'Paid' && (
                      <Button 
                        size="sm" 
                        variant="success" 
                        className="w-full"
                        onClick={() => handleMarkAsPaid(invoice.id)}
                      >
                        <CheckCircle size={14} className="mr-1" />
                        Ödendi Olarak İşaretle
                      </Button>
                    )}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {filteredInvoices.length === 0 && !loading && (
          <div className="text-center py-12">
            <p className="text-secondary-500">
              {searchTerm || filterStatus !== 'All' 
                ? 'Arama kriterlerine uygun fatura bulunamadı.' 
                : 'Henüz fatura bulunmuyor.'
              }
            </p>
            {!searchTerm && filterStatus === 'All' && (
              <Button className="mt-4" onClick={() => setShowCreateModal(true)}>
                İlk Faturayı Oluştur
              </Button>
            )}
          </div>
        )}

        {/* Invoice Modal */}
        <InvoiceModal
          isOpen={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onSuccess={handleCreateSuccess}
        />
      </div>
    </DashboardLayout>
  );
};

export default InvoicesPage;