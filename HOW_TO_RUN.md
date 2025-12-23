# Hướng Dẫn Chạy Dự Án

## 🚀 Cách Chạy 2 Projects Cùng Lúc

### Trong Visual Studio:

1. **Right-click vào Solution** → **Properties**
2. Chọn **"Multiple startup projects"**
3. Set:
   - **E_ToolTiktok.API** → **Start**
   - **E_ToolTiktok.Web** → **Start**
4. Click **OK** và nhấn **F5**

### Hoặc Chạy Từng Project:

#### Bước 1: Chạy API
1. Right-click **E_ToolTiktok.API** → **Set as Startup Project**
2. Nhấn **F5** hoặc **Ctrl+F5**
3. API sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`
4. Kiểm tra: Mở `https://localhost:5001/swagger` để xem API có chạy không

#### Bước 2: Chạy Web (MVC)
1. Right-click **E_ToolTiktok.Web** → **Set as Startup Project**
2. Nhấn **F5** hoặc **Ctrl+F5**
3. Web sẽ chạy tại: `https://localhost:7200` hoặc `http://localhost:5213`

### Chạy Bằng Command Line:

#### Terminal 1 - Chạy API:
```bash
cd src/E_ToolTiktok.API
dotnet run
```

#### Terminal 2 - Chạy Web:
```bash
cd src/E_ToolTiktok.Web
dotnet run
```

## 🔧 Xử Lý Lỗi ERR_CONNECTION_REFUSED

Nếu gặp lỗi `ERR_CONNECTION_REFUSED`, kiểm tra:

1. **API có đang chạy không?**
   - Mở `https://localhost:5001/swagger` hoặc `http://localhost:5000/swagger`
   - Nếu không mở được → API chưa chạy

2. **Port có đúng không?**
   - Kiểm tra trong `src/E_ToolTiktok.API/Properties/launchSettings.json`
   - Mặc định: HTTPS `5001`, HTTP `5000`

3. **Cập nhật API URL trong Web project:**
   - Mở `src/E_ToolTiktok.Web/appsettings.json`
   - Sửa `ApiSettings:BaseUrl` nếu API chạy ở port khác

4. **Kiểm tra HTTPS Certificate:**
   - Nếu dùng HTTPS, có thể cần trust certificate:
   ```bash
   dotnet dev-certs https --trust
   ```

## 📝 Lưu Ý

- **Luôn chạy API trước** khi chạy Web
- Nếu API chạy ở HTTP (port 5000), Web sẽ tự động fallback
- Kiểm tra console trong browser để xem lỗi chi tiết

## ✅ Kiểm Tra Kết Nối

1. Mở browser console (F12)
2. Xem có thông báo "API đã kết nối tại: ..." không
3. Nếu có lỗi, sẽ hiển thị thông báo chi tiết

