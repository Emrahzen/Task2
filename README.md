# Task2 - E-Ticaret Projesi

Bu proje, .NET Core Web API backend ve Next.js frontend kullanarak geliştirilmiş bir e-ticaret uygulamasıdır.

## Özellikler

### Backend (.NET Core Web API)
- **MediatR Pattern** ile CQRS implementasyonu
- **Entity Framework Core** ile veritabanı yönetimi
- **JWT Authentication** ile güvenli API erişimi
- **Redis Cache** ile performans optimizasyonu
- **AutoMapper** ile DTO mapping
- **FluentValidation** ile veri doğrulama

### Frontend (Next.js)
- **TypeScript** ile tip güvenliği
- **Tailwind CSS** ile modern UI tasarımı
- **Redux Toolkit** ile state management
- **Internationalization (i18n)** desteği (Türkçe/İngilizce)
- **Responsive design** ile mobil uyumluluk

## Teknolojiler

### Backend
- .NET 8.0
- Entity Framework Core
- MediatR
- AutoMapper
- FluentValidation
- Redis
- JWT

### Frontend
- Next.js 14
- TypeScript
- Tailwind CSS
- Redux Toolkit
- React Hook Form



### Backend Kurulumu

1. **Veritabanı bağlantısı:**
   ```bash
   cd Task2.API
   # appsettings.json dosyasında connection string'i güncelleyin
   dotnet ef database update
   dotnet run
   ```

2. **Redis kurulumu:**
   - Redis server'ı çalıştırın
   - appsettings.json'da Redis connection string'i güncelleyin

### Frontend Kurulumu

1. **Bağımlılıkları yükleyin:**
   ```bash
   cd frontend
   npm install
   ```

2. **Environment variables:**
   ```bash
   # .env.local dosyası oluşturun
   NEXT_PUBLIC_API_BASE_URL=https://localhost:7040
   ```

3. **Uygulamayı çalıştırın:**
   ```bash
   npm run dev
   ```

### Authentication
- `POST /api/auth/login` - Kullanıcı girişi
- `POST /api/auth/register` - Kullanıcı kaydı

### Products
- `GET /api/product` - Tüm ürünleri listele
- `GET /api/product/{id}` - Ürün detayı
- `POST /api/product` - Yeni ürün ekle (Auth gerekli)
- `PUT /api/product/{id}` - Ürün güncelle (Auth gerekli)
- `DELETE /api/product/{id}` - Ürün sil (Auth gerekli)
- `GET /api/product/category/{category}` - Kategoriye göre ürünler
- `GET /api/product/search` - Ürün arama

## 📱 Kullanım

### Ürün Ekleme
1. Frontend'de `/products/create` sayfasına gidin
2. Gerekli alanları doldurun (Ürün Adı, Fiyat, Stok Miktarı zorunlu)
3. "Ürün Ekle" butonuna tıklayın
4. Ürün API'ye gönderilir ve veritabanına kaydedilir

### Ürün Listeleme
1. `/products` sayfasında tüm ürünler görüntülenir
2. Filtreleme ve arama özellikleri mevcuttur
3. Ürün detayı için ürün kartına tıklayın

### Sepet İşlemleri
1. Ürün detay sayfasında "Add to cart" butonuna tıklayın
2. Ürün Redux store'a eklenir
3. `/cart` sayfasında sepet içeriği görüntülenir

## 🔧 Geliştirme

### Yeni Özellik Ekleme
1. Backend'de gerekli DTO, Command/Query ve Handler'ları oluşturun
2. Frontend'de ilgili component ve sayfaları ekleyin
3. API endpoint'lerini test edin

### Veritabanı Değişiklikleri
1. Entity'lerde değişiklik yapın
2. Migration oluşturun: `dotnet ef migrations add MigrationName`
3. Veritabanını güncelleyin: `dotnet ef database update`

## Notlar

- JWT token'lar localStorage'da saklanır
- Redis cache 15-30 dakika TTL ile çalışır
- Soft delete kullanılarak veriler fiziksel olarak silinmez
- Tüm API endpoint'leri (auth hariç) JWT token gerektirir

## Sorun Giderme

### Backend Hataları
- Veritabanı bağlantısını kontrol edin
- Redis server'ın çalıştığından emin olun
- JWT secret key'in doğru olduğunu kontrol edin

### Frontend Hataları
- API base URL'in doğru olduğunu kontrol edin
- JWT token'ın geçerli olduğunu kontrol edin
- Browser console'da hata mesajlarını inceleyin
