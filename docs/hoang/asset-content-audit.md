# Rà soát prefab và nội dung

- [x] Đủ 9 thiên thể, tên tiếng Việt thống nhất.
- [x] Dữ liệu có nguồn NASA và nêu rõ trường có thể thay đổi.
- [x] Prefab dùng primitive/material nội bộ, không có asset bên thứ ba.
- [x] Tên asset ASCII, nhất quán với ID dữ liệu.
- [x] Nhãn 3D lấy tên trực tiếp từ `PlanetData`, tránh lệch nội dung.
- [x] Material dùng URP/Lit và bật emission riêng cho Mặt Trời.
- [x] Sao Thổ có vành đai mô phỏng bằng cylinder dẹt.
- [x] UI có nền tương phản cao và vùng chạm lớn.
- [ ] Kiểm tra độ sáng, kích thước chữ và thao tác trên thiết bị ARCore ngoài trời.
- [ ] Xác nhận số vệ tinh ngay trước bản phát hành vì dữ liệu có thể thay đổi.

## Cách tái tạo asset

Mở Unity và chọn `Solar System AR > Generate Hoang Week 1-4 Content`. Công cụ tạo lại `PlanetData`, material, 9 prefab và scene demo theo cách idempotent.
