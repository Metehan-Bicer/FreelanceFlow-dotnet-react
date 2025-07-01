import React, { useState } from 'react';
import { User, Lock, Bell, Globe, Save } from 'lucide-react';
import { DashboardLayout } from '../components/layout/DashboardLayout';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { useAuth } from '../hooks/useAuth';

const SettingsPage: React.FC = () => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState('profile');
  const [saving, setSaving] = useState(false);

  const tabs = [
    { id: 'profile', label: 'Profil', icon: User },
    { id: 'security', label: 'Güvenlik', icon: Lock },
    { id: 'notifications', label: 'Bildirimler', icon: Bell },
    { id: 'general', label: 'Genel', icon: Globe },
  ];

  const handleSave = async () => {
    setSaving(true);
    // Simulate save operation
    setTimeout(() => {
      setSaving(false);
      alert('Ayarlar başarıyla kaydedildi!');
    }, 1000);
  };

  const renderProfileTab = () => (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-medium text-secondary-900 mb-4">Profil Bilgileri</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            label="Kullanıcı Adı"
            defaultValue={user?.username || ''}
            placeholder="Kullanıcı adınız"
          />
          <Input
            label="E-posta"
            type="email"
            defaultValue={user?.email || ''}
            placeholder="E-posta adresiniz"
          />
          <Input
            label="Ad"
            placeholder="Adınız"
          />
          <Input
            label="Soyad"
            placeholder="Soyadınız"
          />
          <Input
            label="Telefon"
            placeholder="Telefon numaranız"
          />
          <Input
            label="Şirket"
            placeholder="Şirket adınız"
          />
        </div>
      </div>
      
      <div>
        <h3 className="text-lg font-medium text-secondary-900 mb-4">Adres Bilgileri</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="md:col-span-2">
            <Input
              label="Adres"
              placeholder="Tam adresiniz"
            />
          </div>
          <Input
            label="Şehir"
            placeholder="Şehir"
          />
          <Input
            label="Ülke"
            placeholder="Ülke"
          />
        </div>
      </div>
    </div>
  );

  const renderSecurityTab = () => (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-medium text-secondary-900 mb-4">Şifre Değiştir</h3>
        <div className="max-w-md space-y-4">
          <Input
            label="Mevcut Şifre"
            type="password"
            placeholder="Mevcut şifreniz"
          />
          <Input
            label="Yeni Şifre"
            type="password"
            placeholder="Yeni şifreniz"
          />
          <Input
            label="Yeni Şifre (Tekrar)"
            type="password"
            placeholder="Yeni şifrenizi tekrar girin"
          />
        </div>
      </div>
    </div>
  );

  const renderNotificationsTab = () => (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-medium text-secondary-900 mb-4">Bildirim Tercihleri</h3>
        <div className="space-y-4">
          <div className="flex items-center justify-between p-4 border border-secondary-200 rounded-lg">
            <div>
              <p className="font-medium text-secondary-900">E-posta Bildirimleri</p>
              <p className="text-sm text-secondary-600">Yeni faturalar ve ödemeler hakkında e-posta alın</p>
            </div>
            <input type="checkbox" className="w-4 h-4 text-primary-600 rounded" defaultChecked />
          </div>
          
          <div className="flex items-center justify-between p-4 border border-secondary-200 rounded-lg">
            <div>
              <p className="font-medium text-secondary-900">Proje Bildirimleri</p>
              <p className="text-sm text-secondary-600">Proje durumu değişiklikleri hakkında bildirim alın</p>
            </div>
            <input type="checkbox" className="w-4 h-4 text-primary-600 rounded" defaultChecked />
          </div>
          
          <div className="flex items-center justify-between p-4 border border-secondary-200 rounded-lg">
            <div>
              <p className="font-medium text-secondary-900">Ödeme Hatırlatmaları</p>
              <p className="text-sm text-secondary-600">Vadesi yaklaşan ödemeler için hatırlatma alın</p>
            </div>
            <input type="checkbox" className="w-4 h-4 text-primary-600 rounded" />
          </div>
        </div>
      </div>
    </div>
  );

  const renderGeneralTab = () => (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-medium text-secondary-900 mb-4">Genel Ayarlar</h3>
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Dil
            </label>
            <select className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500">
              <option value="tr">Türkçe</option>
              <option value="en">English</option>
            </select>
          </div>
          
          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Para Birimi
            </label>
            <select className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500">
              <option value="TRY">Türk Lirası (₺)</option>
              <option value="USD">US Dollar ($)</option>
              <option value="EUR">Euro (€)</option>
            </select>
          </div>
          
          <div>
            <label className="block text-sm font-medium text-secondary-700 mb-2">
              Saat Dilimi
            </label>
            <select className="w-full px-3 py-2 border border-secondary-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500">
              <option value="Europe/Istanbul">Istanbul (GMT+3)</option>
              <option value="UTC">UTC (GMT+0)</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  );

  const renderTabContent = () => {
    switch (activeTab) {
      case 'profile':
        return renderProfileTab();
      case 'security':
        return renderSecurityTab();
      case 'notifications':
        return renderNotificationsTab();
      case 'general':
        return renderGeneralTab();
      default:
        return renderProfileTab();
    }
  };

  return (
    <DashboardLayout title="Ayarlar">
      <div className="flex gap-6">
        {/* Sidebar */}
        <div className="w-64 flex-shrink-0">
          <Card>
            <CardContent className="p-4">
              <nav className="space-y-1">
                {tabs.map((tab) => {
                  const Icon = tab.icon;
                  return (
                    <button
                      key={tab.id}
                      onClick={() => setActiveTab(tab.id)}
                      className={`w-full flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                        activeTab === tab.id
                          ? 'bg-primary-100 text-primary-700'
                          : 'text-secondary-600 hover:bg-secondary-100 hover:text-secondary-900'
                      }`}
                    >
                      <Icon size={16} className="mr-3" />
                      {tab.label}
                    </button>
                  );
                })}
              </nav>
            </CardContent>
          </Card>
        </div>

        {/* Content */}
        <div className="flex-1">
          <Card>
            <CardHeader>
              <CardTitle>
                {tabs.find(tab => tab.id === activeTab)?.label} Ayarları
              </CardTitle>
            </CardHeader>
            <CardContent>
              {renderTabContent()}
              
              <div className="flex justify-end pt-6 border-t border-secondary-200 mt-6">
                <Button onClick={handleSave} loading={saving}>
                  <Save size={16} className="mr-2" />
                  Kaydet
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </DashboardLayout>
  );
};

export default SettingsPage;