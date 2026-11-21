# Báo cáo kiểm thử Location Service (W2-D3)

Ngày ghi nhận: 2026-11-21

Unity project: `6000.6.0f1`
Người phụ trách module: Lê Anh Đức

## Kết quả triển khai

- `LocationServiceController` khởi tạo `Input.location` và trên Android yêu cầu quyền vị trí khi dịch vụ bắt đầu.
- Có timeout khởi tạo cấu hình được (mặc định 20 giây), trạng thái `Initializing` / `Running` / `Failed` / `Stopped`, trạng thái dịch vụ bật/tắt, lat/lon, horizontal accuracy và timestamp.
- Panel chẩn đoán có nút yêu cầu/thử lại, thông báo khi quyền chưa được cấp, Location tắt, không lấy được fix hoặc hết thời gian chờ.
- Component chỉ đọc và hiển thị dữ liệu chẩn đoán; không chuyển GPS thành vị trí hành tinh.

## Trạng thái xác minh

| Kiểm tra | Kết quả |
| --- | --- |
| Rà soát mã nguồn và luồng trạng thái | Đã thực hiện |
| Biên dịch trong Unity Editor | Chưa chạy: Unity CLI không tìm thấy Editor instance hoặc Editor đã cài cho project |
| Build APK | Chưa chạy: không có Android SDK/ADB trong môi trường |
| Cấp/từ chối quyền và đọc GPS trên điện thoại | Chưa chạy: không có thiết bị Android kết nối |

Do đó tiêu chí chạy trên điện thoại chưa được xác nhận. Không có APK hoặc ảnh/video kiểm thử thiết bị để đính kèm.

## Các bước kiểm thử thiết bị còn lại

1. Gắn `LocationServiceController` vào một GameObject trong scene chạy trên Android.
2. Cài APK, cấp quyền vị trí khi được hỏi và ghi lại lat/lon, accuracy, timestamp cùng trạng thái trên panel.
3. Xóa quyền rồi chạy lại để xác nhận thông báo từ chối; tắt Location/GPS để xác nhận trạng thái tắt.
4. Thử không cấp quyền/không có fix quá thời gian timeout và xác nhận thông báo timeout hoặc lỗi.
5. Đối chiếu Transform hành tinh trước/sau khi nhận GPS; vị trí hành tinh không được thay đổi do GPS.
