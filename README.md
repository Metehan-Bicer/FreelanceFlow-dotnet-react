# FreelanceFlow - Freelance İş Takip & Fatura Otomasyon Sistemi

<div align="center">
  <h3>🚀 Modern Freelance İş Yönetim Platformu</h3>
  <p>Clean Architecture ile geliştirilmiş, tam stack web uygulaması</p>
  
  ![.NET](https://img.shields.io/badge/.NET-8.0-purple)
  ![React](https://img.shields.io/badge/React-18-blue)
  ![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue)
  ![TailwindCSS](https://img.shields.io/badge/TailwindCSS-3.0-cyan)
  ![SQLite](https://img.shields.io/badge/SQLite-Database-green)
  ![License](https://img.shields.io/badge/License-MIT-yellow)
</div>

## 🎯 Proje Hakkında

FreelanceFlow, modern freelancer'lar için geliştirilmiş kapsamlı bir iş takip ve fatura otomasyon sistemidir. Müşteri ilişkileri yönetiminden proje takibine, zaman yönetiminden otomatik fatura oluşturmaya kadar freelance iş süreçlerinin tümünü dijitalleştiren profesyonel bir platformdur.

### ✨ Temel Özellikler
- 👥 **Müşteri Yönetimi** - Kapsamlı müşteri profilleri ve iletişim takibi
- 📋 **Proje Takibi** - Detaylı proje planlama ve durum yönetimi
- ⏱️ **Zaman Takibi** - Proje bazlı çalışma süresi kayıtları
- 🧾 **Fatura Otomasyonu** - Otomatik PDF fatura oluşturma ve gönderme
- 💰 **Ödeme Takibi** - Gelir analizi ve ödeme durumu yönetimi
- 📊 **Dashboard & Raporlama** - Görsel analitik ve performans metrikleri
- 🔐 **Güvenlik** - JWT tabanlı kimlik doğrulama sistemi

## 🏗️ Sistem Mimarisi

Proje, **Clean Architecture (Onion Architecture)** prensiplerine uygun olarak katmanlı bir yapıda geliştirilmiştir:

```
📁 FreelanceFlow/
├── 📁 frontend/                        # React Frontend Uygulaması
│   └── 📁 freelance-flow-ui/
│       ├── 📁 src/
│       │   ├── 📁 components/          # Yeniden kullanılabilir UI bileşenleri
│       │   ├── 📁 pages/               # Sayfa bileşenleri
│       │   ├── 📁 services/            # API servisleri
│       │   ├── 📁 types/               # TypeScript tip tanımları
│       │   ├── 📁 contexts/            # React Context API
│       │   └── 📁 hooks/               # Custom React hooks
│       ├── package.json
│       └── tailwind.config.js
├── 📁 src/                             # Backend API Uygulaması
│   ├── 📁 FreelanceFlow.API/           # 🌐 Web API Katmanı (Controllers, Middleware)
│   ├── 📁 FreelanceFlow.Application/   # 🔧 Uygulama Katmanı (Services, DTOs, Mappings)
│   ├── 📁 FreelanceFlow.Domain/        # 🏛️ Domain Katmanı (Entities, Enums, Interfaces)
│   ├── 📁 FreelanceFlow.Infrastructure/# 🔌 Altyapı Katmanı (External Services, PDF, Email)
│   ├── 📁 FreelanceFlow.Persistence/   # 📊 Veri Erişim Katmanı (EF Core, DbContext)
│   └── 📁 FreelanceFlow.Core/          # 📚 Ortak Kütüphane (Common, Constants)
└── 📁 tests/                           # 🧪 Test Projeleri
    ├── 📁 FreelanceFlow.UnitTests/
    └── 📁 FreelanceFlow.IntegrationTests/
```

## 🛠️ Teknoloji Stack'i

### 🔧 Backend (.NET 8)
| Teknoloji | Sürüm | Açıklama |
|-----------|-------|----------|
| **ASP.NET Core** | 8.0 | Web API framework |
| **Entity Framework Core** | 8.0 | ORM ve veri erişimi |
| **SQLite** | - | Geliştirme veritabanı |
| **AutoMapper** | 12.0 | DTO mapping |
| **FluentValidation** | 11.3 | Model validasyon |
| **JWT Bearer** | 8.0 | Authentication |
| **Swagger/OpenAPI** | 6.4 | API dokümantasyonu |
| **QuestPDF** | 2025.5 | PDF oluşturma |
| **MailKit** | 4.13 | Email gönderimi |
| **Serilog** | 8.0 | Loglama |

### 🎨 Frontend (React 18)
| Teknoloji | Sürüm | Açıklama |
|-----------|-------|----------|
| **React** | 18.x | UI kütüphanesi |
| **TypeScript** | 5.x | Tip güvenliği |
| **TailwindCSS** | 3.x | Utility-first CSS framework |
| **Axios** | - | HTTP client |
| **React Router** | 6.x | SPA routing |
| **React Hook Form** | - | Form yönetimi |
| **Context API** | - | State management |
| **React Query** | - | Server state management |

### 🏛️ Architectural Patterns
- **Clean Architecture** - Katmanlı mimari
- **Repository Pattern** - Veri erişim abstraction'ı
- **Result Pattern** - Hata yönetimi
- **CQRS** - Command Query Responsibility Segregation
- **Dependency Injection** - IoC container
- **Unit of Work** - Transaction yönetimi

## ✨ Özellik Detayları

### 👥 Müşteri Yönetimi
- ✅ CRUD işlemleri (Oluştur, Oku, Güncelle, Sil)
- ✅ Aktif/Pasif durum yönetimi
- ✅ İletişim bilgileri takibi
- ✅ Müşteri arama ve filtreleme
- ✅ Müşteri bazlı proje geçmişi

### 📋 Proje Yönetimi
- ✅ Proje durumu takibi (Planning, InProgress, OnHold, Completed, Cancelled)
- ✅ Bütçe planlama ve maliyet takibi
- ✅ Deadline yönetimi
- ✅ Öncelik seviyesi belirleme
- ✅ İlerleme yüzdesi takibi
- ✅ Proje notları ve dokümantasyonu

### 🧾 Fatura Sistemi
- ✅ Otomatik fatura numaralama
- ✅ Professional PDF fatura oluşturma
- ✅ KDV hesaplama
- ✅ Ödeme durumu takibi (Pending, Paid, Overdue, Cancelled)
- ✅ Email ile fatura gönderimi
- ✅ Fatura geçmişi ve raporlama

### 📊 Dashboard & Analytics
- ✅ Gelir analizi ve grafikleri
- ✅ Proje durumu istatistikleri
- ✅ Müşteri aktivite takibi
- ✅ Aylık gelir trendi
- ✅ Bekleyen işler özeti

### 🔐 Güvenlik Özellikleri
- ✅ JWT Bearer Token authentication
- ✅ Password hashing (BCrypt)
- ✅ API rate limiting
- ✅ CORS policy yönetimi
- ✅ Input validation
- ✅ Soft delete (veri güvenliği)

## 🚀 Kurulum ve Çalıştırma

### 📋 Gereksinimler
- **Backend:** .NET 8 SDK
- **Frontend:** Node.js 18+ ve npm/yarn
- **Database:** SQLite (varsayılan) veya SQL Server
- **IDE:** Visual Studio 2022, VS Code

### ⚡ Hızlı Başlangıç

#### 1. 📥 Projeyi İndirin
```bash
git clone https://github.com/username/FreelanceFlow.git
cd FreelanceFlow
```

#### 2. 🔧 Backend Kurulumu
```bash
# Bağımlılıkları yükle
dotnet restore

# Veritabanını oluştur
cd src/FreelanceFlow.API
dotnet ef database update

# API'yi başlat
dotnet run
```

Backend şu adreste çalışacak: `https://localhost:7000,http://localhost:5000`

#### 3. 🎨 Frontend Kurulumu
```bash
# Frontend dizinine geç
cd frontend/freelance-flow-ui

# Bağımlılıkları yükle
npm install

# Development server'ı başlat
npm start
```

Frontend şu adreste çalışacak: `http://localhost:3000`

#### 4. 📖 API Dokümantasyonu
Swagger UI: `https://localhost:7000/swagger,http://localhost:5000/swagger`

## 📊 API Endpoints

### 🔐 Authentication
```
POST /api/auth/register     # Kullanıcı kaydı
POST /api/auth/login        # Kullanıcı girişi
POST /api/auth/refresh      # Token yenileme
```

### 👥 Clients (Müşteriler)
```
GET    /api/clients              # Tüm müşterileri listele
GET    /api/clients/active       # Aktif müşterileri listele
GET    /api/clients/{id}         # Müşteri detayı
POST   /api/clients              # Yeni müşteri oluştur
PUT    /api/clients/{id}         # Müşteri güncelle
PUT    /api/clients/{id}/status  # Müşteri durumu güncelle
DELETE /api/clients/{id}         # Müşteri sil
GET    /api/clients/search?name={name} # Müşteri ara
```

### 📋 Projects (Projeler)
```
GET    /api/projects                    # Tüm projeleri listele
GET    /api/projects/active             # Aktif projeleri listele
GET    /api/projects/{id}               # Proje detayı
POST   /api/projects                    # Yeni proje oluştur
PUT    /api/projects/{id}               # Proje güncelle
PUT    /api/projects/{id}/status        # Proje durumu güncelle
PUT    /api/projects/{id}/active-status # Proje aktif durumu güncelle
DELETE /api/projects/{id}               # Proje sil
GET    /api/projects/client/{clientId}  # Müşteri projeleri
```

### 🧾 Invoices (Faturalar)
```
GET    /api/invoices                    # Tüm faturaları listele
GET    /api/invoices/{id}               # Fatura detayı
POST   /api/invoices                    # Yeni fatura oluştur
PUT    /api/invoices/{id}               # Fatura güncelle
PUT    /api/invoices/{id}/status        # Fatura durumu güncelle
PUT    /api/invoices/{id}/payment-status # Ödeme durumu güncelle
DELETE /api/invoices/{id}               # Fatura sil
GET    /api/invoices/{id}/pdf           # PDF fatura indir
POST   /api/invoices/{id}/send-email    # Email ile fatura gönder
POST   /api/invoices/{id}/mark-paid     # Ödendi olarak işaretle
GET    /api/invoices/overdue            # Vadesi geçen faturalar
GET    /api/invoices/pending            # Bekleyen faturalar
```

### 📊 Dashboard
```
GET /api/dashboard/stats              # Genel istatistikler
GET /api/dashboard/recent-activities  # Son aktiviteler
GET /api/dashboard/monthly-revenue    # Aylık gelir grafik verisi
```

## 🗄️ Veritabanı Şeması

### 📋 Temel Entity'ler
| Entity | Açıklama | İlişkiler |
|--------|----------|-----------|
| **Users** | Sistem kullanıcıları | 1:N → Clients, Projects, Invoices |
| **Clients** | Müşteri bilgileri | 1:N → Projects, Invoices |
| **Projects** | Proje detayları | N:1 → Client, 1:N → Tasks, TimeEntries |
| **ProjectTasks** | Proje görevleri | N:1 → Project, 1:N → TimeEntries |
| **TimeEntries** | Zaman kayıtları | N:1 → Project, Task |
| **Invoices** | Fatura bilgileri | N:1 → Client, Project, 1:N → InvoiceItems |
| **InvoiceItems** | Fatura kalemleri | N:1 → Invoice |

### 🔗 İlişki Diyagramı
```
Users (1) ──→ (N) Clients (1) ──→ (N) Projects (1) ──→ (N) Tasks
   │                │                    │                  │
   │                │                    │                  │
   │                └─→ (N) Invoices (N) ←┘                 │
   │                            │                           │
   │                            └→ (N) InvoiceItems         │
   │                                                        │
   └─→ (N) TimeEntries ←─ (N) ──────────────────────────────┘
```

## 🧪 Test Stratejisi

### Unit Tests
```bash
# Tüm unit testleri çalıştır
dotnet test tests/FreelanceFlow.UnitTests/

# Coverage raporu oluştur
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests
```bash
# API integration testleri
dotnet test tests/FreelanceFlow.IntegrationTests/
```

### Frontend Tests
```bash
cd frontend/freelance-flow-ui

# Unit testleri çalıştır
npm test

# Coverage raporu
npm run test:coverage
```

## 📝 Örnek Kullanım Senaryoları

### 🎯 Yeni Müşteri ve Proje Oluşturma
```javascript
// 1. Yeni müşteri oluştur
const client = await clientService.create({
  name: "Test Yazılım Ltd.",
  email: "info@Test.com",
  phone: "+90 212 555 0123",
  company: "Test Yazılım Ltd.",
  address: "İstanbul, Türkiye",
  taxNumber: "1234567890"
});

// 2. Müşteri için proje oluştur
const project = await projectService.create({
  name: "E-Ticaret Platformu",
  description: "Modern React/Node.js e-ticaret sitesi",
  clientId: client.id,
  startDate: "2024-01-01",
  deadlineDate: "2024-06-01",
  budget: 75000,
  priority: 3 // High
});

// 3. Proje için fatura oluştur
const invoice = await invoiceService.create({
  clientId: client.id,
  projectId: project.id,
  items: [
    {
      description: "Frontend Geliştirme",
      quantity: 40,
      unitPrice: 1000
    },
    {
      description: "Backend API Geliştirme",
      quantity: 30,
      unitPrice: 1200
    }
  ],
  taxRate: 18
});
```

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow
```yaml
name: FreelanceFlow CI/CD

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  backend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Test
        run: dotnet test

  frontend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: 18
      - name: Test
        run: |
          cd frontend/freelance-flow-ui
          npm ci
          npm test
```

## 📈 Performans Optimizasyonları

- **Backend:** Response caching, async/await pattern, Entity Framework query optimization
- **Frontend:** React.memo, useMemo, useCallback, code splitting, lazy loading
- **Database:** Indexing, query optimization, connection pooling

## 🤝 Katkıda Bulunma

### 📋 Katkı Süreci
1. 🍴 Fork yapın
2. 🌟 Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. ✅ Testlerinizi yazın
4. 💾 Commit yapın (`git commit -m 'feat: Add amazing feature'`)
5. 📤 Push yapın (`git push origin feature/amazing-feature`)
6. 🔄 Pull Request oluşturun

### 📏 Kod Standartları
- **Backend:** C# Coding Conventions, SOLID principles
- **Frontend:** ESLint + Prettier configuration
- **Git:** Conventional Commits standardı

## 🔮 Roadmap

### 🎯 Yakın Gelecek (Q3 2025)
- [ ] 📱 React Native mobil uygulama
- [ ] 📧 Gelişmiş email template sistemi
- [ ] 🌍 Çoklu dil desteği

### 🚀 Uzun Vadeli (Q4 2025)
- [ ] ☁️ Cloud deployment (Azure/AWS)
- [ ] 🤖 AI-powered project estimation
- [ ] 📊 Advanced analytics dashboard
- [ ] 🔗 Third-party integrations (Stripe, PayPal)

## 📊 Proje İstatistikleri

```
📂 Total Files: 200+
📝 Lines of Code: 15,000+
🧪 Test Coverage: 85%+
🚀 Build Time: <1 minutes
📦 Bundle Size: <500KB (gzipped)
```

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim & Destek

<div align="center">

### 👨‍💻 Geliştirici: Metehan Biçer

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/metehan-bi%C3%A7er-b77a89210/)
[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Metehan-Bicer)

### 💬 Destek
- 🐛 **Bug Reports:** GitHub Issues
- 💡 **Feature Requests:** GitHub Discussions
- 📧 **Email:** metehanbicer006@gmail.com

### ⭐ Projeyi Beğendiyseniz
Eğer bu proje işinize yaradıysa, GitHub'da ⭐ vermeyi unutmayın!

</div>

---

<div align="center">
  <p><strong>FreelanceFlow ile freelance işlerinizi profesyonel seviyeye taşıyın! 🚀</strong></p>
  <p><em>Made with ❤️ by Metehan Biçer</em></p>
</div>