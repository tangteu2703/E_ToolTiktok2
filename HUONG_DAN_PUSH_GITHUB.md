# Hướng dẫn đẩy code lên GitHub

## Cách 1: Sử dụng script tự động (Khuyến nghị)

1. Mở PowerShell trong thư mục dự án: `D:\8.ProjectGit\E_ToolTiktok`
2. Chạy lệnh:
   ```powershell
   .\push-to-github.ps1
   ```

## Cách 2: Chạy thủ công từng lệnh

Mở PowerShell hoặc Git Bash trong thư mục dự án và chạy các lệnh sau:

```bash
# 1. Khởi tạo git repository
git init

# 2. Thêm remote GitHub
git remote add origin https://github.com/tangteu2703/E_ToolTiktok2.git

# 3. Thêm tất cả các file
git add .

# 4. Commit
git commit -m "Initial commit: E_ToolTiktok project"

# 5. Đặt branch chính là main
git branch -M main

# 6. Đẩy lên GitHub
git push -u origin main
```

## Lưu ý

- Nếu bạn chưa cấu hình Git lần đầu, bạn cần chạy:
  ```bash
  git config --global user.name "Your Name"
  git config --global user.email "your.email@example.com"
  ```

- Nếu gặp lỗi authentication, bạn có thể cần:
  - Sử dụng Personal Access Token thay vì password
  - Hoặc cấu hình SSH key

- File `.gitignore` đã được tạo để loại trừ các file không cần thiết (bin, obj, etc.)

