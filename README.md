# FreelanceFlow - Freelance İş Takip & Fatura Otomasyon Sistemi

## 🎯 Proje Hakkında
FreelanceFlow, freelancer'lar için müşteri takibi, proje yönetimi, teklif hazırlama ve otomatik fatura oluşturma sistemidir. Clean Architecture prensiplerine uygun olarak geliştirilmiştir.

## 🏗️ Mimari Yapı
Proje, Clean Architecture (Onion Architecture) prensiplerine uygun olarak katmanlı bir yapıda geliştirilmiştir:

```
📁 FreelanceFlow/
├── 📁 src/
│   ├── 📁 FreelanceFlow.API/           # Web API Katmanı
│   ├── 📁 FreelanceFlow.Application/   # Uygulama Katmanı
│   ├── 📁 FreelanceFlow.Domain/        # Domain Katmanı
│   ├── 📁 FreelanceFlow.Infrastructure/# Altyapı Katmanı
│   ├── 📁 FreelanceFlow.Persistence/   # Veri Erişim Katmanı
│   └── 📁 FreelanceFlow.Core/          # Ortak Kütüphane
└── 📁 tests/                           # Test Projeleri
```

## 🛠️ Kullanılan Teknolojiler
- **Backend:** ASP.NET Core 8 Web API
- **Database:** Entity Framework Core + SQL Server
- **Authentication:** JWT Bearer Token
- **Documentation:** Swagger/OpenAPI
- **Mapping:** AutoMapper
- **Validation:** FluentValidation
- **Architecture:** Clean Architecture
- **Patterns:** Repository Pattern, Result Pattern

## ✨ Özellikler
- ✅ Müşteri yönetimi (CRUD işlemleri)
- ✅ Proje takibi ve yönetimi
- ✅ Görev (Task) yönetimi
- ✅ Zaman takibi (Time Tracking)
- ✅ Fatura oluşturma ve yönetimi
- ✅ Ödeme durumu takibi
- ✅ JWT Authentication
- ✅ Swagger API dokümantasyonu
- ✅ Result Pattern ile hata yönetimi
- ✅ AutoMapper ile DTO dönüşümleri
- ✅ Soft Delete desteği

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8 SDK
- SQL Server (LocalDB desteklenir)
- Visual Studio 2022 veya VS Code

### Adımlar
1. **Projeyi klonlayın:**
   ```bash
   git clone [repository-url]
   cd FreelanceFlow
   ```

2. **Bağımlılıkları yükleyin:**
   ```bash
   dotnet restore
   ```

3. **Veritabanı ayarları:**
   - `appsettings.json` dosyasında connection string'i düzenleyin
   - Uygulama ilk çalıştırıldığında otomatik olarak veritabanı oluşturulur

4. **Uygulamayı çalıştırın:**
   ```bash
   cd src/FreelanceFlow.API
   dotnet run
   ```

5. **Swagger UI'ye erişin:**
   - https://localhost:7000 (veya http://localhost:5000)

## 📊 API Endpoints

### Clients (Müşteriler)
- `GET /api/clients` - Tüm müşterileri listele
- `GET /api/clients/{id}` - Müşteri detayı
- `POST /api/clients` - Yeni müşteri oluştur
- `PUT /api/clients/{id}` - Müşteri güncelle
- `DELETE /api/clients/{id}` - Müşteri sil
- `GET /api/clients/search?name={name}` - Müşteri ara
- `GET /api/clients/active` - Aktif müşterileri listele

### Projects (Projeler)
- `GET /api/projects` - Tüm projeleri listele
- `GET /api/projects/{id}` - Proje detayı
- `POST /api/projects` - Yeni proje oluştur
- `PUT /api/projects/{id}` - Proje güncelle
- `DELETE /api/projects/{id}` - Proje sil

### Invoices (Faturalar)
- `GET /api/invoices` - Tüm faturaları listele
- `GET /api/invoices/{id}` - Fatura detayı
- `POST /api/invoices` - Yeni fatura oluştur
- `PUT /api/invoices/{id}/status` - Fatura durumu güncelle

## 🗄️ Veritabanı Şeması
Sistem aşağıdaki temel entity'leri içerir:
- **Clients:** Müşteri bilgileri
- **Projects:** Proje bilgileri
- **ProjectTasks:** Proje görevleri
- **TimeEntries:** Zaman kayıtları
- **Invoices:** Fatura bilgileri
- **InvoiceItems:** Fatura kalemleri

## 🔒 Authentication
Sistem JWT Bearer token authentication kullanır. API endpoint'lerine erişim için geçerli bir token gereklidir.

## 🧪 Test Etme
```bash
# Unit testleri çalıştır
dotnet test tests/FreelanceFlow.UnitTests/

# Integration testleri çalıştır
dotnet test tests/FreelanceFlow.IntegrationTests/
```

## 📝 Örnek Kullanım

### Yeni Müşteri Oluşturma
```json
POST /api/clients
{
  "name": "Acme Yazılım Ltd.",
  "email": "info@acme.com",
  "phone": "+90 212 555 0123",
  "company": "Acme Yazılım Ltd.",
  "address": "İstanbul, Türkiye",
  "taxNumber": "1234567890",
  "notes": "Önemli müşteri"
}
```

### Yeni Proje Oluşturma
```json
POST /api/projects
{
  "name": "E-Ticaret Platformu",
  "description": "Modern e-ticaret sitesi geliştirme",
  "clientId": "guid-here",
  "startDate": "2024-01-01",
  "deadlineDate": "2024-06-01",
  "budget": 75000,
  "priority": 3
}
```

## 🤝 Katkıda Bulunma
1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans
Bu proje MIT lisansı altında lisanslanmıştır.

## 📞 İletişim
Proje hakkında sorularınız için https://www.linkedin.com/in/metehan-bi%C3%A7er-b77a89210/