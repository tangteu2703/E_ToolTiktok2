# Tính Năng Proxy và Chọn Quốc Gia

## 🌍 Tính Năng Mới

### 1. **Chọn Quốc Gia**
- Dropdown để chọn quốc gia khi đăng ký
- Hỗ trợ 20+ quốc gia: Việt Nam, US, UK, Canada, Australia, Japan, Korea, v.v.
- Mỗi quốc gia có:
  - Timezone riêng
  - Language riêng
  - Region code riêng
  - Tên người dùng phù hợp với văn hóa

### 2. **Tạo Tên Theo Quốc Gia**
- **Việt Nam**: Tên Việt Nam (Nguyễn Văn An, Trần Thị Lan, ...)
- **US/UK**: Tên tiếng Anh (James Smith, Mary Johnson, ...)
- **Japan**: Tên Nhật Bản (Tanaka Hiroshi, Sato Yuki, ...)
- **Korea**: Tên Hàn Quốc (Kim Min-jun, Lee Soo-jin, ...)
- Tự động tạo username phù hợp với từng quốc gia

### 3. **Proxy Rotation**
- Mỗi tài khoản tự động dùng IP/proxy khác nhau
- Proxy được lấy theo quốc gia đã chọn
- Hỗ trợ nhiều loại proxy: HTTP, HTTPS, SOCKS5

## 🔧 Cách Hoạt Động

### Proxy Service
1. **ProxyScrape API** (Free):
   - Lấy proxy miễn phí từ ProxyScrape
   - Filter theo quốc gia
   - Tự động test proxy

2. **Free Proxy Lists**:
   - Fallback nếu ProxyScrape không hoạt động
   - Lấy từ nhiều nguồn khác nhau

3. **Test Proxy**:
   - Tự động test proxy trước khi sử dụng
   - Chỉ dùng proxy hoạt động

### Name Generator
- Database tên theo từng quốc gia
- Tự động generate firstName + lastName
- Username được tạo từ tên + số ngẫu nhiên

## 📝 Cách Sử Dụng

### Manual Registration:
1. Chọn quốc gia từ dropdown
2. Nhập thông tin (hoặc để trống để tự động)
3. Check "Sử dụng Proxy" để dùng IP khác nhau
4. Click "Đăng Ký"

### Auto Registration:
1. Chọn quốc gia từ dropdown
2. Nhập số lượng tài khoản
3. Nhập mật khẩu mặc định
4. Click "Bắt Đầu Tạo Tự Động"
5. Mỗi tài khoản sẽ tự động:
   - Tạo email từ inboxes.com
   - Tạo tên theo quốc gia
   - Lấy proxy/IP khác nhau
   - Đăng ký với thông tin phù hợp

## ⚙️ Cấu Hình

### Thêm Quốc Gia Mới:
Sửa file `src/E_ToolTiktok.Core/Models/Country.cs`:
```csharp
new Country { 
    Code = "XX", 
    Name = "Country Name", 
    NameVi = "Tên Tiếng Việt",
    TimeZone = "Continent/City", 
    Language = "xx", 
    Region = "XX" 
}
```

### Thêm Tên Mới:
Sửa file `src/E_ToolTiktok.Core/Services/NameGeneratorService.cs`:
```csharp
_firstNames["XX"] = new List<string> { "Name1", "Name2", ... };
_lastNames["XX"] = new List<string> { "LastName1", "LastName2", ... };
```

## 🔍 Proxy Providers (Có thể tích hợp thêm)

### Free Options:
- **ProxyScrape**: `https://api.proxyscrape.com`
- **Free Proxy List**: Nhiều nguồn khác nhau

### Paid Options (Mạnh hơn):
- **Bright Data** (Luminati): Premium proxy service
- **Smartproxy**: Residential proxies
- **ProxyMesh**: Rotating proxies
- **ScraperAPI**: Proxy + scraping service

### Để tích hợp Proxy Service trả phí:
1. Thêm API key vào `appsettings.json`
2. Cập nhật `ProxyService.cs` để gọi API của họ
3. Parse response và tạo `ProxyInfo` objects

## ⚠️ Lưu Ý

1. **Free Proxies**:
   - Có thể không ổn định
   - Có thể bị chặn bởi TikTok
   - Nên test trước khi dùng

2. **Paid Proxies**:
   - Ổn định hơn
   - Tốc độ nhanh hơn
   - Tỷ lệ thành công cao hơn

3. **Rate Limiting**:
   - TikTok có thể giới hạn số request
   - Nên đợi giữa các lần đăng ký (đã set 3 giây)

4. **IP Rotation**:
   - Mỗi account dùng IP khác nhau
   - Giúp tránh bị phát hiện
   - Tăng tỷ lệ thành công

## 🚀 Cải Tiến Trong Tương Lai

- [ ] Tích hợp Bright Data API
- [ ] Cache proxies để tái sử dụng
- [ ] Auto-retry với proxy khác nếu fail
- [ ] Thống kê proxy success rate
- [ ] Support SOCKS5 proxy
- [ ] Proxy health check tự động

