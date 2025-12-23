#!/bin/bash
# Script để fix lỗi push GitHub khi remote đã có code

echo "Đang pull code từ remote và merge..."
git pull origin main --allow-unrelated-histories --no-edit

echo "Đang push code lên GitHub..."
git push -u origin main

echo "Hoàn thành!"

