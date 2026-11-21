# Solar System AR - Checklist

Cập nhật gần nhất: **21/11/2026**

## Quy ước trạng thái

- [ ] **Chưa cập nhật**: chưa có kết quả được ghi nhận.
- [x] **Chờ nghiệm thu**: đã có sản phẩm bàn giao hoặc kết quả chạy thử, đang chờ xác nhận hoàn tất.
- [x] **Hoàn thành**: đã được nghiệm thu.

## Tuần 1

| Xong | Nhiệm vụ | Thành viên | Trạng thái | Hạn | Ưu tiên | Sản phẩm bàn giao |
|---|---|---|---|---|---|---|
| [ ] | Tập hợp dữ liệu AU và chọn scale | Lê Anh Đức | Chưa cập nhật | 13/11/2026 | MVP | Bảng AU, đề xuất chiều dài đường |
| [ ] | Tìm hiểu tọa độ Unity và GPS | Lê Anh Đức | Chưa cập nhật | 13/11/2026 | MVP | Scene thử + ghi chú GPS |
| [ ] | Kiểm tra Điện thoại và Địa Điểm | Lê Anh Đức | Chưa cập nhật | 13/11/2026 | MVP | Checklist thiết bị + test plan |
| [x] | Phân tích yêu cầu và user flow | Trần Huy Hoàng | Chờ nghiệm thu | 13/11/2026 | MVP | `docs/hoang/requirements-user-flow.md` |
| [x] | Thu thập thông tin 9 thiên thể | Trần Huy Hoàng | Chờ nghiệm thu | 13/11/2026 | MVP | `docs/hoang/planet-data.md` + PlanetData assets |
| [x] | Học Sphere, Material và tìm asset mẫu | Trần Huy Hoàng | Chờ nghiệm thu | 13/11/2026 | MVP | 9 primitive prefabs, không dùng asset ngoài |
| [x] | Thiết lập Unity và Android | Chữ Văn Lộc | Chờ nghiệm thu | 13/11/2026 | MVP | Unity project chạy Android |
| [x] | Tạo repo và cấu trúc project | Chữ Văn Lộc | Chờ nghiệm thu | 13/11/2026 | MVP | Git repository + README |
| [x] | Cài AR Foundation và chạy thử camera | Chữ Văn Lộc | Chờ nghiệm thu | 13/11/2026 | MVP | AR sample APK |

## Tuần 2

| Xong | Nhiệm vụ | Thành viên | Trạng thái | Hạn | Ưu tiên | Sản phẩm bàn giao |
|---|---|---|---|---|---|---|
| [ ] | Viết ScaleCalculator | Lê Anh Đức | Chưa cập nhật | 20/11/2026 | MVP | `ScaleCalculator.cs` |
| [ ] | Tạo test scene khoảng cách | Lê Anh Đức | Chưa cập nhật | 20/11/2026 | MVP | ScaleTestScene + kết quả |
| [ ] | Thử đọc GPS bằng LocationService | Lê Anh Đức | Chưa cập nhật | 20/11/2026 | MVP | `LocationServiceController.cs` |
| [x] | Tạo bộ 9 prefab cơ bản | Trần Huy Hoàng | Chờ nghiệm thu | 20/11/2026 | MVP | `Assets/Prefabs/Planets/` |
| [x] | Chuẩn hóa dữ liệu hành tinh | Trần Huy Hoàng | Chờ nghiệm thu | 20/11/2026 | MVP | `PlanetData.cs` + 9 ScriptableObjects |
| [x] | Làm bảng thông tin mẫu | Trần Huy Hoàng | Chờ nghiệm thu | 20/11/2026 | MVP | Planet Info Panel trong scene demo |
| [x] | Nhận diện mặt phẳng và AR Raycast | Chữ Văn Lộc | Chờ nghiệm thu | 20/11/2026 | MVP | Scene AR Placement |
| [ ] | Đặt Mặt Trời, Anchor và reset | Chữ Văn Lộc | Chưa cập nhật | 20/11/2026 | MVP | `ARPlacementManager.cs` |
| [ ] | Build prototype v0.1 | Chữ Văn Lộc | Chưa cập nhật | 20/11/2026 | MVP | APK v0.1 |

## Tuần 3

| Xong | Nhiệm vụ | Thành viên | Trạng thái | Hạn | Ưu tiên | Sản phẩm bàn giao |
|---|---|---|---|---|---|---|
| [ ] | Kết nối scale với hệ tọa độ AR | Lê Anh Đức | Chưa cập nhật | 27/11/2026 | MVP | Scale module tích hợp |
| [ ] | Tính khoảng cách từ người dùng | Lê Anh Đức | Chưa cập nhật | 27/11/2026 | MVP | `DistanceTracker.cs` + GPS Status |
| [ ] | QA lần đầu và bug log | Lê Anh Đức | Chưa cập nhật | 27/11/2026 | MVP | Tracking Test Log + bug list |
| [x] | Làm Start Menu và hướng dẫn | Trần Huy Hoàng | Chờ nghiệm thu | 27/11/2026 | MVP | Main Menu + Tutorial Panel |
| [x] | Bảng thông tin động và nhãn 3D | Trần Huy Hoàng | Chờ nghiệm thu | 27/11/2026 | MVP | `PlanetInfoPanel.cs` + 9 labels |
| [x] | Rà soát prefab và nội dung | Trần Huy Hoàng | Chờ nghiệm thu | 27/11/2026 | MVP | `docs/hoang/asset-content-audit.md` |
| [ ] | Khởi tạo hệ hành tinh từ Mặt Trời | Chữ Văn Lộc | Chưa cập nhật | 27/11/2026 | MVP | `SolarSystemManager.cs` |
| [ ] | Tích hợp chọn hành tinh và panel | Chữ Văn Lộc | Chưa cập nhật | 27/11/2026 | MVP | `PlanetInteraction.cs` |
| [ ] | Build bản tích hợp v0.2 | Chữ Văn Lộc | Chưa cập nhật | 27/11/2026 | MVP | APK v0.2 + log |

## Tuần 4

| Xong | Nhiệm vụ | Thành viên | Trạng thái | Hạn | Ưu tiên | Sản phẩm bàn giao |
|---|---|---|---|---|---|---|
| [ ] | Khảo sát và kiểm thử đường đi ngoài trời | Lê Anh Đức | Chưa cập nhật | 04/12/2026 | MVP | Outdoor Test Report |
| [ ] | Đánh giá GPS và Geospatial | Lê Anh Đức | Chưa cập nhật | 04/12/2026 | MVP | Quyết định định vị |
| [ ] | Regression và báo cáo lỗi cuối tuần | Lê Anh Đức | Chưa cập nhật | 04/12/2026 | MVP | Test Case Report + backlog fix |
| [x] | Tinh chỉnh giao diện và mô hình | Trần Huy Hoàng | Chờ nghiệm thu | 04/12/2026 | MVP | UI + prefab cập nhật, ảnh preview |
| [ ] | Thử nghiệm người dùng, cập nhật Figma | Trần Huy Hoàng | Đang làm | 04/12/2026 | MVP | Heuristic review + screenshot; chờ test người thật |
| [x] | Chuẩn bị quiz và tiến độ khám phá | Trần Huy Hoàng | Chờ nghiệm thu | 04/12/2026 | Bổ sung | Quiz dataset + Discovery UI |
| [ ] | Ổn định anchor và trạng thái AR | Chữ Văn Lộc | Chưa cập nhật | 04/12/2026 | MVP | `ApplicationManager.cs` |
| [ ] | Sửa lỗi, tối ưu và tích hợp v0.3 | Chữ Văn Lộc | Chưa cập nhật | 04/12/2026 | MVP | APK v0.3 |
| [ ] | Thử mũi tên điều hướng | Chữ Văn Lộc | Chưa cập nhật | 04/12/2026 | Bổ sung | Arrow Indicator thử nghiệm |

## Kết quả đã xác minh trên thiết bị

- [x] APK Android build và cài đặt thành công.
- [x] AR Foundation và Google ARCore khởi tạo thành công.
- [x] Camera sau hiển thị đúng bằng OpenGLES3.
- [x] ARCore nhận diện và hiển thị mặt phẳng.
- [ ] Chưa kiểm tra thao tác raycast đặt Mặt Trời, tạo anchor và reset.
