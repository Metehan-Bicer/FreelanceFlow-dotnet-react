import React, { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { clientService } from '../../services/clientService';
import { CreateClientRequest, Client } from '../../types';

interface ClientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  client?: Client; // Edit mode için
  mode?: 'create' | 'edit';
}

const ClientModal: React.FC<ClientModalProps> = ({ 
  isOpen, 
  onClose, 
  onSuccess, 
  client, 
  mode = 'create' 
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [formData, setFormData] = useState<CreateClientRequest>({
    name: '',
    email: '',
    phone: '',
    company: '',
    address: '',
    taxNumber: '',
    notes: ''
  });

  // Edit mode için form data'yı doldur
  useEffect(() => {
    if (client && mode === 'edit') {
      setFormData({
        name: client.name || '',
        email: client.email || '',
        phone: client.phone || '',
        company: client.company || '',
        address: client.address || '',
        taxNumber: client.taxNumber || '',
        notes: client.notes || ''
      });
    }
  }, [client, mode]);

  const resetForm = () => {
    setFormData({
      name: '',
      email: '',
      phone: '',
      company: '',
      address: '',
      taxNumber: '',
      notes: ''
    });
    setError('');
    setSuccess('');
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      console.log(`Müşteri ${mode === 'edit' ? 'güncelleme' : 'ekleme'} isteği gönderiliyor:`, formData);
      
      let response;
      if (mode === 'edit' && client) {
        response = await clientService.update(client.id, formData);
      } else {
        response = await clientService.create(formData);
      }
      
      console.log('Backend response:', response);
      
      if (response.success) {
        console.log(`Müşteri başarıyla ${mode === 'edit' ? 'güncellendi' : 'eklendi'}:`, response.data);
        setSuccess(`Müşteri başarıyla ${mode === 'edit' ? 'güncellendi' : 'eklendi'}!`);
        
        // Kısa bir delay ile kullanıcıya success mesajını göster
        setTimeout(() => {
          onSuccess(); // Bu, parent component'te loadClients() fonksiyonunu çağırır
          handleClose();
        }, 1000);
      } else {
        console.error('Backend hatası:', response.error);
        setError(response.error || `Müşteri ${mode === 'edit' ? 'güncellenirken' : 'oluşturulurken'} bir hata oluştu`);
      }
    } catch (error) {
      console.error('Network/Client hatası:', error);
      setError('Bağlantı hatası. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
    // Input değiştiğinde hata mesajını temizle
    if (error) setError('');
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">
            {mode === 'edit' ? 'Müşteri Düzenle' : 'Yeni Müşteri Ekle'}
          </h2>
          <button
            onClick={handleClose}
            className="p-1 hover:bg-gray-100 rounded"
            disabled={loading}
          >
            <X size={20} />
          </button>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Success Message */}
        {success && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md">
            <p className="text-sm text-green-600">{success}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Müşteri Adı"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
            placeholder="Müşteri adını girin"
            disabled={loading}
          />

          <Input
            label="E-posta"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            required
            placeholder="E-posta adresini girin"
            disabled={loading}
          />

          <Input
            label="Telefon"
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            placeholder="Telefon numarasını girin"
            disabled={loading}
          />

          <Input
            label="Şirket"
            name="company"
            value={formData.company}
            onChange={handleChange}
            placeholder="Şirket adını girin"
            disabled={loading}
          />

          <Input
            label="Adres"
            name="address"
            value={formData.address}
            onChange={handleChange}
            placeholder="Adres bilgisini girin"
            disabled={loading}
          />

          <Input
            label="Vergi Numarası"
            name="taxNumber"
            value={formData.taxNumber}
            onChange={handleChange}
            placeholder="Vergi numarasını girin"
            disabled={loading}
          />

          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Notlar
            </label>
            <textarea
              name="notes"
              value={formData.notes}
              onChange={handleChange}
              rows={3}
              className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:bg-gray-50 disabled:cursor-not-allowed"
              placeholder="Ek notlar..."
              disabled={loading}
            />
          </div>

          <div className="flex space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              className="flex-1"
              disabled={loading}
            >
              İptal
            </Button>
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
              disabled={loading}
            >
              {loading ? 
                (mode === 'edit' ? 'Güncelleniyor...' : 'Kaydediliyor...') : 
                (mode === 'edit' ? 'Güncelle' : 'Kaydet')
              }
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export { ClientModal };