import React, { useState, useEffect } from 'react';
import { X, Plus, Minus } from 'lucide-react';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { invoiceService } from '../../services/invoiceService';
import { clientService } from '../../services/clientService';
import { projectService } from '../../services/projectService';
import { CreateInvoiceRequest, Client, Project, CreateInvoiceItemRequest } from '../../types';

interface InvoiceModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const InvoiceModal: React.FC<InvoiceModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [clients, setClients] = useState<Client[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [formData, setFormData] = useState<CreateInvoiceRequest>({
    clientId: '',
    projectId: '',
    issueDate: '',
    dueDate: '',
    taxRate: 18,
    notes: '',
    items: [{ description: '', quantity: 1, unitPrice: 0 }]
  });

  useEffect(() => {
    if (isOpen) {
      loadClients();
      // Set default dates
      const today = new Date().toISOString().split('T')[0];
      const dueDate = new Date();
      dueDate.setDate(dueDate.getDate() + 30);
      
      setFormData(prev => ({
        ...prev,
        issueDate: today,
        dueDate: dueDate.toISOString().split('T')[0]
      }));
    }
  }, [isOpen]);

  useEffect(() => {
    if (formData.clientId) {
      loadProjectsForClient(formData.clientId);
    } else {
      setProjects([]);
    }
  }, [formData.clientId]);

  const loadClients = async () => {
    try {
      const response = await clientService.getAll();
      if (response.success && response.data) {
        setClients(response.data);
      }
    } catch (error) {
      console.error('Error loading clients:', error);
    }
  };

  const loadProjectsForClient = async (clientId: string) => {
    try {
      const response = await projectService.getByClientId(clientId);
      if (response.success && response.data) {
        setProjects(response.data);
      }
    } catch (error) {
      console.error('Error loading projects:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await invoiceService.create(formData);
      if (response.success) {
        onSuccess();
        onClose();
        // Reset form
        setFormData({
          clientId: '',
          projectId: '',
          issueDate: '',
          dueDate: '',
          taxRate: 18,
          notes: '',
          items: [{ description: '', quantity: 1, unitPrice: 0 }]
        });
      } else {
        alert(response.error || 'Fatura oluşturulurken hata oluştu');
      }
    } catch (error) {
      alert('Fatura oluşturulurken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'taxRate' ? Number(value) : value
    });
  };

  const handleItemChange = (index: number, field: keyof CreateInvoiceItemRequest, value: string | number) => {
    const newItems = [...formData.items];
    newItems[index] = {
      ...newItems[index],
      [field]: field === 'quantity' || field === 'unitPrice' ? Number(value) : value
    };
    setFormData({ ...formData, items: newItems });
  };

  const addItem = () => {
    setFormData({
      ...formData,
      items: [...formData.items, { description: '', quantity: 1, unitPrice: 0 }]
    });
  };

  const removeItem = (index: number) => {
    if (formData.items.length > 1) {
      const newItems = formData.items.filter((_, i) => i !== index);
      setFormData({ ...formData, items: newItems });
    }
  };

  const calculateSubTotal = () => {
    return formData.items.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);
  };

  const calculateTax = () => {
    return calculateSubTotal() * (formData.taxRate / 100);
  };

  const calculateTotal = () => {
    return calculateSubTotal() + calculateTax();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-4xl max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">Yeni Fatura Oluştur</h2>
          <button
            onClick={onClose}
            className="p-1 hover:bg-gray-100 rounded"
          >
            <X size={20} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-secondary-700 mb-2">
                Müşteri <span className="text-red-500">*</span>
              </label>
              <select
                name="clientId"
                value={formData.clientId}
                onChange={handleChange}
                required
                className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
              >
                <option value="">Müşteri seçin</option>
                {clients.map((client) => (
                  <option key={client.id} value={client.id}>
                    {client.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-secondary-700 mb-2">
                Proje (Opsiyonel)
              </label>
              <select
                name="projectId"
                value={formData.projectId}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
              >
                <option value="">Proje seçin</option>
                {projects.map((project) => (
                  <option key={project.id} value={project.id}>
                    {project.name}
                  </option>
                ))}
              </select>
            </div>

            <Input
              label="Düzenleme Tarihi"
              name="issueDate"
              type="date"
              value={formData.issueDate}
              onChange={handleChange}
              required
            />

            <Input
              label="Vade Tarihi"
              name="dueDate"
              type="date"
              value={formData.dueDate}
              onChange={handleChange}
              required
            />
          </div>

          {/* Invoice Items */}
          <div>
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium">Fatura Kalemleri</h3>
              <Button type="button" variant="outline" size="sm" onClick={addItem}>
                <Plus size={16} className="mr-2" />
                Kalem Ekle
              </Button>
            </div>

            <div className="space-y-4">
              {formData.items.map((item, index) => (
                <div key={index} className="grid grid-cols-1 md:grid-cols-5 gap-4 p-4 border border-secondary-200 rounded-lg">
                  <div className="md:col-span-2">
                    <Input
                      label="Açıklama"
                      value={item.description}
                      onChange={(e) => handleItemChange(index, 'description', e.target.value)}
                      placeholder="Hizmet/ürün açıklaması"
                      required
                    />
                  </div>
                  <div>
                    <Input
                      label="Miktar"
                      type="number"
                      value={item.quantity}
                      onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
                      min="0.01"
                      step="0.01"
                      required
                    />
                  </div>
                  <div>
                    <Input
                      label="Birim Fiyat"
                      type="number"
                      value={item.unitPrice}
                      onChange={(e) => handleItemChange(index, 'unitPrice', e.target.value)}
                      min="0"
                      step="0.01"
                      required
                    />
                  </div>
                  <div className="flex items-end">
                    <div className="flex-1">
                      <label className="block text-sm font-medium text-secondary-700 mb-2">Toplam</label>
                      <div className="px-3 py-2 bg-secondary-50 border border-secondary-200 rounded-md text-sm">
                        {(item.quantity * item.unitPrice).toFixed(2)} ₺
                      </div>
                    </div>
                    {formData.items.length > 1 && (
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => removeItem(index)}
                        className="ml-2"
                      >
                        <Minus size={16} />
                      </Button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Totals */}
          <div className="bg-secondary-50 p-4 rounded-lg">
            <div className="space-y-2">
              <div className="flex justify-between">
                <span>Ara Toplam:</span>
                <span>{calculateSubTotal().toFixed(2)} ₺</span>
              </div>
              <div className="flex justify-between items-center">
                <div className="flex items-center space-x-2">
                  <span>KDV:</span>
                  <Input
                    type="number"
                    name="taxRate"
                    value={formData.taxRate}
                    onChange={handleChange}
                    className="w-20"
                    min="0"
                    max="100"
                    step="1"
                  />
                  <span>%</span>
                </div>
                <span>{calculateTax().toFixed(2)} ₺</span>
              </div>
              <div className="flex justify-between font-semibold text-lg border-t pt-2">
                <span>Genel Toplam:</span>
                <span>{calculateTotal().toFixed(2)} ₺</span>
              </div>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Notlar
            </label>
            <textarea
              name="notes"
              value={formData.notes}
              onChange={handleChange}
              rows={3}
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="Fatura notları..."
            />
          </div>

          <div className="flex space-x-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              className="flex-1"
            >
              İptal
            </Button>
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
            >
              Faturayı Oluştur
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export { InvoiceModal };