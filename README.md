# E_ToolTiktok - Tool Tự Đăng Ký Tài Khoản TikTok

Dự án .NET Core để tự động đăng ký tài khoản TikTok, có thể build và chạy trên Web hoặc Mobile (thông qua API).

## 🚀 Tính năng

- ✅ Đăng ký tài khoản TikTok tự động qua API (không cần Selenium)
- ✅ Tự động tạo email ngẫu nhiên từ [inboxes.com](https://inboxes.com/)
- ✅ Tự động lấy mã xác nhận từ inboxes.com
- ✅ Tự động xác thực email sau khi đăng ký
- ✅ Hỗ trợ đăng ký bằng Email
- ✅ Xác thực email và số điện thoại
- ✅ RESTful API để tích hợp với Web/Mobile
- ✅ Swagger UI để test API
- ✅ CORS enabled cho mobile clients

## 📋 Yêu cầu

- .NET 8.0 SDK hoặc cao hơn
- Kết nối Internet (để gọi TikTok API và inboxes.com API)

## 🛠️ Cài đặt

### 1. Clone repository

```bash
git clone <repository-url>
cd E_ToolTiktok
```

### 2. Restore packages

```bash
dotnet restore
```

### 3. Build solution

```bash
dotnet build
```

## 🏃 Chạy ứng dụng

### Chạy Web API

```bash
cd src/E_ToolTiktok.API
dotnet run
```

API sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

### Truy cập Swagger UI

Mở trình duyệt và truy cập: `https://localhost:5001/swagger`

## 📱 Sử dụng API cho Mobile/Web

### 1. Đăng ký tài khoản (Tự động tạo email từ inboxes.com)

**Endpoint:** `POST /api/tiktokregistration/register`

**Request Body (Email có thể để trống để tự động tạo):**
```json
{
  "email": "",
  "password": "YourPassword123!",
  "username": "your_username",
  "birthDate": "2000-01-01T00:00:00"
}
```

**Hoặc với email cụ thể:**
```json
{
  "email": "example@email.com",
  "password": "YourPassword123!",
  "username": "your_username",
  "birthDate": "2000-01-01T00:00:00"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đăng ký và xác thực email thành công",
  "account": {
    "username": "your_username",
    "email": "random123@vomoto.com",
    "createdAt": "2024-01-01T00:00:00",
    "isVerified": true
  }
}
```

**Lưu ý:** Nếu không cung cấp email, hệ thống sẽ tự động tạo email ngẫu nhiên từ inboxes.com và tự động lấy mã xác nhận để verify.

### 2. Tạo email ngẫu nhiên từ inboxes.com

**Endpoint:** `POST /api/tiktokregistration/create-email`

**Response:**
```json
{
  "email": "random123@vomoto.com",
  "inboxId": "random123@vomoto.com",
  "createdAt": "2024-01-01T00:00:00"
}
```

### 3. Lấy danh sách email từ inbox

**Endpoint:** `GET /api/tiktokregistration/inbox/{email}/messages`

**Response:**
```json
[
  {
    "id": "msg123",
    "from": "noreply@tiktok.com",
    "subject": "Verify your email",
    "preview": "Your verification code is 123456",
    "received": "2024-01-01T00:00:00"
  }
]
```

### 4. Lấy mã xác nhận tự động

**Endpoint:** `GET /api/tiktokregistration/inbox/{email}/verification-code?timeoutSeconds=60`

**Response:**
```json
{
  "success": true,
  "verificationCode": "123456"
}
```

### 5. Xác thực Email (Tự động lấy mã nếu không cung cấp)

**Endpoint:** `POST /api/tiktokregistration/verify-email`

**Request Body (có thể để trống verificationCode để tự động lấy):**
```json
{
  "email": "example@email.com",
  "verificationCode": ""
}
```

**Hoặc với mã cụ thể:**
```json
{
  "email": "example@email.com",
  "verificationCode": "123456"
}
```

### 6. Xác thực Số điện thoại

**Endpoint:** `POST /api/tiktokregistration/verify-phone`

**Request Body:**
```json
{
  "phoneNumber": "+84123456789",
  "verificationCode": "123456"
}
```

### 7. Health Check

**Endpoint:** `GET /api/tiktokregistration/health`

## 📦 Build cho Production

### Build Release

```bash
dotnet build -c Release
```

### Publish cho Web

```bash
cd src/E_ToolTiktok.API
dotnet publish -c Release -o ./publish
```

### Publish cho Linux

```bash
dotnet publish -c Release -r linux-x64 -o ./publish
```

### Publish cho Windows

```bash
dotnet publish -c Release -r win-x64 -o ./publish
```

## 🔧 Cấu hình

Chỉnh sửa file `appsettings.json` để cấu hình:

```json
{
  "TiktokSettings": {
    "DefaultTimeout": 30
  }
}
```

## 🔗 Tích hợp với Inboxes.com

Tool tự động tích hợp với [inboxes.com](https://inboxes.com/) để:
- Tạo email tạm thời ngẫu nhiên
- Lấy danh sách email từ inbox
- Tự động parse mã xác nhận từ email TikTok
- Tự động xác thực email sau khi đăng ký

## 📱 Tích hợp với Mobile App

### Android (Kotlin/Java)

```kotlin
val client = OkHttpClient()
val json = JSONObject()
json.put("email", "example@email.com")
json.put("password", "YourPassword123!")
json.put("username", "your_username")

val requestBody = json.toString().toRequestBody("application/json".toMediaType())
val request = Request.Builder()
    .url("https://your-api-url/api/tiktokregistration/register")
    .post(requestBody)
    .build()

val response = client.newCall(request).execute()
```

### iOS (Swift)

```swift
let url = URL(string: "https://your-api-url/api/tiktokregistration/register")!
var request = URLRequest(url: url)
request.httpMethod = "POST"
request.setValue("application/json", forHTTPHeaderField: "Content-Type")

let body: [String: Any] = [
    "email": "example@email.com",
    "password": "YourPassword123!",
    "username": "your_username"
]

request.httpBody = try? JSONSerialization.data(withJSONObject: body)

URLSession.shared.dataTask(with: request) { data, response, error in
    // Handle response
}.resume()
```

## ⚠️ Lưu ý

- Tool này chỉ dùng cho mục đích học tập và nghiên cứu
- Việc tự động đăng ký tài khoản có thể vi phạm Terms of Service của TikTok
- Sử dụng có trách nhiệm và tuân thủ pháp luật
- TikTok có thể thay đổi giao diện và cơ chế đăng ký, cần cập nhật code tương ứng

## 📄 License

MIT License

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Vui lòng tạo Pull Request.

