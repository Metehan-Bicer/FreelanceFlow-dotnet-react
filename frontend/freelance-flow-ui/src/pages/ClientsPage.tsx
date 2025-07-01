import React, { useState, useEffect } from 'react';
import { Plus, Search, Edit, Trash2, Mail, Phone, Building } from 'lucide-react';
import { DashboardLayout } from '../components/layout/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Badge } from '../components/ui/Badge';
import { Loading } from '../components/ui/Loading';
import { ClientModal } from '../components/modals/ClientModal';
import { clientService } from '../services/clientService';
import { Client } from '../types';
import { formatCurrency, formatDate } from '../utils';

const ClientsPage: React.FC = () => {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState('');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedClient, setSelectedClient] = useState<Client | null>(null);

  useEffect(() => {
    loadClients();
  }, []);

  const loadClients = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await clientService.getAll();
      if (response.success && response.data) {
        setClients(response.data);
      } else {
        setError(response.error || 'Müşteriler yüklenirken hata oluştu');
      }
    } catch (err) {
      setError('Müşteriler yüklenirken hata oluştu');
      console.error('Error loading clients:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu müşteriyi silmek istediğinizden emin misiniz?')) {
      try {
        const response = await clientService.delete(id);
        if (response.success) {
          setClients(clients.filter(client => client.id !== id));
        } else {
          setError('Müşteri silinirken hata oluştu');
        }
      } catch (err) {
        setError('Müşteri silinirken hata oluştu');
      }
    }
  };

  const handleEdit = (client: Client) => {
    setSelectedClient(client);
    setShowEditModal(true);
  };

  const handleModalSuccess = () => {
    loadClients(); // Reload clients after successful operation
  };

  const handleCloseEditModal = () => {
    setShowEditModal(false);
    setSelectedClient(null);
  };

  const filteredClients = clients.filter(client =>
    client.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    client.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <DashboardLayout title="Müşteriler">
        <div className="flex items-center justify-center h-64">
          <Loading size="lg" text="Müşteriler yükleniyor..." />
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout title="Müşteriler">
      <div className="space-y-6">
        {/* Header Actions */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-secondary-400" size={16} />
            <Input
              type="text"
              placeholder="Müşteri ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          <Button onClick={() => setShowCreateModal(true)}>
            <Plus size={16} className="mr-2" />
            Yeni Müşteri
          </Button>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-md p-4">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Clients Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredClients.map((client) => (
            <Card key={client.id}>
              <CardHeader className="pb-3">
                <div className="flex items-start justify-between">
                  <div className="flex items-center space-x-3">
                    <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
                      <span className="text-primary-700 font-semibold">
                        {client.name.charAt(0).toUpperCase()}
                      </span>
                    </div>
                    <div>
                      <CardTitle className="text-lg">{client.name}</CardTitle>
                      <Badge variant={client.isActive ? 'success' : 'error'} size="sm">
                        {client.isActive ? 'Aktif' : 'Pasif'}
                      </Badge>
                    </div>
                  </div>
                  <div className="flex space-x-1">
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleEdit(client)}
                      title="Müşteriyi düzenle"
                    >
                      <Edit size={16} />
                    </Button>
                    <Button 
                      variant="ghost" 
                      size="sm"
                      onClick={() => handleDelete(client.id)}
                      title="Müşteriyi sil"
                    >
                      <Trash2 size={16} />
                    </Button>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  <div className="flex items-center text-sm text-secondary-600">
                    <Mail size={14} className="mr-2" />
                    {client.email}
                  </div>
                  {client.phone && (
                    <div className="flex items-center text-sm text-secondary-600">
                      <Phone size={14} className="mr-2" />
                      {client.phone}
                    </div>
                  )}
                  {client.address && (
                    <div className="flex items-center text-sm text-secondary-600">
                      <Building size={14} className="mr-2" />
                      {client.address}
                    </div>
                  )}
                  <div className="pt-3 border-t border-secondary-200">
                    <div className="flex justify-between text-sm">
                      <span className="text-secondary-600">Projeler:</span>
                      <span className="font-medium">{client.projectCount || 0}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-secondary-600">Toplam Gelir:</span>
                      <span className="font-medium">{formatCurrency(client.totalRevenue || 0)}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-secondary-600">Kayıt:</span>
                      <span className="font-medium">{formatDate(client.createdAt, 'short')}</span>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {filteredClients.length === 0 && !loading && (
          <div className="text-center py-12">
            <p className="text-secondary-500">
              {searchTerm ? 'Arama kriterlerine uygun müşteri bulunamadı.' : 'Henüz müşteri bulunmuyor.'}
            </p>
            {!searchTerm && (
              <Button className="mt-4" onClick={() => setShowCreateModal(true)}>
                İlk Müşteriyi Ekle
              </Button>
            )}
          </div>
        )}

        {/* Create Client Modal */}
        <ClientModal
          isOpen={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onSuccess={handleModalSuccess}
          mode="create"
        />

        {/* Edit Client Modal */}
        <ClientModal
          isOpen={showEditModal}
          onClose={handleCloseEditModal}
          onSuccess={handleModalSuccess}
          client={selectedClient || undefined}
          mode="edit"
        />
      </div>
    </DashboardLayout>
  );
};

export default ClientsPage;