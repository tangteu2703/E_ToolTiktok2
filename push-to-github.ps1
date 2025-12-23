# Script để đẩy code lên GitHub
# Chạy script này: .\push-to-github.ps1

$gitPath = "C:\Program Files\Git\cmd\git.exe"

# Kiểm tra xem Git có tồn tại không
if (-not (Test-Path $gitPath)) {
    Write-Host "Không tìm thấy Git tại: $gitPath" -ForegroundColor Red
    Write-Host "Vui lòng cài đặt Git hoặc cập nhật đường dẫn trong script này." -ForegroundColor Yellow
    exit 1
}

# Khởi tạo git repository (nếu chưa có)
if (-not (Test-Path ".git")) {
    Write-Host "Đang khởi tạo git repository..." -ForegroundColor Green
    & $gitPath init
}

# Thêm remote (nếu chưa có)
$remoteUrl = "https://github.com/tangteu2703/E_ToolTiktok2.git"
$remoteExists = & $gitPath remote | Select-String -Pattern "origin"

if (-not $remoteExists) {
    Write-Host "Đang thêm remote origin..." -ForegroundColor Green
    & $gitPath remote add origin $remoteUrl
} else {
    Write-Host "Đang cập nhật remote origin..." -ForegroundColor Green
    & $gitPath remote set-url origin $remoteUrl
}

# Thêm tất cả các file
Write-Host "Đang thêm các file vào staging area..." -ForegroundColor Green
& $gitPath add .

# Commit (nếu có thay đổi)
$status = & $gitPath status --porcelain
if ($status) {
    Write-Host "Đang commit các thay đổi..." -ForegroundColor Green
    & $gitPath commit -m "Initial commit: E_ToolTiktok project"
} else {
    Write-Host "Không có thay đổi để commit." -ForegroundColor Yellow
}

# Đẩy lên GitHub
Write-Host "Đang đẩy code lên GitHub..." -ForegroundColor Green
& $gitPath branch -M main
& $gitPath push -u origin main

Write-Host "Hoàn thành! Code đã được đẩy lên GitHub." -ForegroundColor Green
Write-Host "Repository: $remoteUrl" -ForegroundColor Cyan

