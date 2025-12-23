# Hướng dẫn fix lỗi push GitHub

## Vấn đề
Khi push code lên GitHub, bạn gặp lỗi:
```
! [rejected]        main -> main (fetch first)
error: failed to push some refs
```

## Nguyên nhân
Remote repository trên GitHub đã có code (thường là README.md mặc định) mà local repository chưa có.

## Giải pháp

### Cách 1: Pull và merge (Khuyến nghị - An toàn)

Trong Git Bash, chạy các lệnh sau:

```bash
# 1. Pull code từ remote và merge (cho phép merge các history không liên quan)
git pull origin main --allow-unrelated-histories --no-edit

# 2. Nếu có conflict, giải quyết conflict rồi:
git add .
git commit -m "Merge remote and local code"

# 3. Push lên GitHub
git push -u origin main
```

### Cách 2: Force push (Cẩn thận - Sẽ ghi đè code trên remote)

**CHỈ DÙNG NẾU BẠN CHẮC CHẮN MUỐN GHI ĐÈ CODE TRÊN REMOTE:**

```bash
git push -u origin main --force
```

⚠️ **Cảnh báo:** Lệnh này sẽ ghi đè hoàn toàn code trên remote. Chỉ dùng nếu bạn chắc chắn muốn làm vậy.

### Cách 3: Sử dụng script tự động

Chạy script `fix-push-github.sh`:
```bash
chmod +x fix-push-github.sh
./fix-push-github.sh
```

## Lưu ý

- Cách 1 là an toàn nhất vì nó giữ lại cả code local và remote
- Nếu có conflict khi merge, bạn cần giải quyết conflict trước khi push
- Sau khi merge thành công, code của bạn sẽ được kết hợp với README.md trên GitHub

