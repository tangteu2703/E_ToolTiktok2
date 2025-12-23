# Hướng Dẫn Debug TikTok API

## 🔍 Vấn Đề: CSRF Token = null và API_CONNECTION_FAILED

### Các Cải Tiến Đã Thực Hiện:

1. **Cải thiện cách lấy CSRF Token:**
   - Sử dụng CookieContainer để maintain session
   - Thử nhiều patterns khác nhau để tìm token trong HTML
   - Tìm trong JSON data của page
   - Lấy từ cookies
   - Fallback: Generate token nếu không tìm thấy

2. **Cải thiện API Endpoints:**
   - Thử nhiều endpoints khác nhau
   - Thử nhiều format payload khác nhau
   - Thêm logging chi tiết

3. **Cải thiện Error Handling:**
   - Thêm thông tin debug chi tiết trong response
   - Log tất cả errors
   - Hiển thị endpoints đã thử

## 🛠️ Cách Debug:

### 1. Kiểm tra Logs trong Visual Studio:
- Mở **Output** window (View → Output)
- Chọn "Debug" từ dropdown
- Xem các thông báo:
  - `TikTok API - Endpoint: ...`
  - `TikTok API - Status: ...`
  - `TikTok API - Response: ...`

### 2. Kiểm tra Response trong API:
- Mở Swagger: `http://localhost:5000/swagger`
- Test endpoint `/api/tiktokregistration/register`
- Xem response có `AdditionalData` với thông tin debug

### 3. Kiểm tra Browser Console:
- Mở F12 → Console
- Xem có lỗi CORS hoặc network không

## 📝 Lưu Ý:

- TikTok có thể đã thay đổi API endpoints
- Có thể cần thêm headers hoặc cookies khác
- Có thể cần giải quyết CAPTCHA
- API có thể yêu cầu rate limiting

## 🔧 Nếu Vẫn Lỗi:

1. **Kiểm tra TikTok có thay đổi API không:**
   - Mở `https://www.tiktok.com/signup` trong browser
   - Mở DevTools → Network tab
   - Thử đăng ký thủ công và xem API call nào được gọi
   - Copy endpoint và payload để cập nhật vào code

2. **Kiểm tra CSRF Token:**
   - Xem trong browser cookies khi vào TikTok
   - Tìm cookie có tên chứa "csrf" hoặc "token"

3. **Cập nhật User-Agent:**
   - Có thể cần User-Agent mới hơn
   - Copy từ browser hiện tại

## 💡 Gợi Ý:

Nếu TikTok API không hoạt động, có thể cần:
- Sử dụng Selenium để tự động hóa browser (đã có code cũ)
- Hoặc tìm API mới từ TikTok
- Hoặc sử dụng TikTok Official API nếu có

