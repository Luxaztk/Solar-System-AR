# Solar System AR

Ứng dụng Android dùng AR Foundation để đặt và khám phá mô hình Hệ Mặt Trời trong không gian thực.

## Trạng thái hiện tại

- Đã scaffold Unity project bằng template **3D URP**.
- Đã cấu hình Unity **6000.6.0f1** và Android Build Support.
- Đã khai báo AR Foundation và Google ARCore XR Plugin **6.7.1**.
- Đã cấu hình Android package `com.solarsystemar.app`, IL2CPP và ARM64.
- Đã bật ARCore loader và hoàn tất scene AR Placement: nhận diện mặt phẳng,
  raycast, đặt Mặt Trời bằng anchor và reset.
- Prototype **v0.1** (`versionCode 3`) build thành công và đã cài trên thiết bị
  Android thật. Xem kết quả kiểm tra tại [`docs/week-2-ar-placement-test.md`](docs/week-2-ar-placement-test.md).

## Yêu cầu

- Unity Hub
- Unity Editor `6000.6.0f1`
- Android Build Support gồm SDK, NDK và OpenJDK
- Thiết bị Android hỗ trợ ARCore, bật USB debugging
- Git LFS

## Mở dự án lần đầu

1. Clone repository và chạy `git lfs pull`.
2. Trong Unity Hub, chọn **Add > Add project from disk** và mở thư mục repository.
3. Chờ Unity khôi phục package và import asset.
4. Mở **Edit > Project Settings > XR Plug-in Management > Android** và bật **ARCore**.
5. Chuyển build target sang Android trong **File > Build Profiles**.
6. Kết nối thiết bị ARCore rồi dùng **Build And Run** để xác nhận camera.

> Không commit các thư mục sinh tự động như `Library`, `Temp`, `Logs`, `Obj`, `Build` và `UserSettings`.

## Cấu trúc thư mục

```text
Assets/
├── Art/
│   ├── Materials/
│   ├── Models/
│   └── Textures/
├── Audio/
├── Prefabs/
├── Scenes/
├── Scripts/
│   ├── AR/
│   ├── Core/
│   └── UI/
└── Settings/
Packages/
ProjectSettings/
```

## Lộ trình MVP

1. Thiết lập Unity, Android, repository và camera AR.
2. Nhận diện mặt phẳng, AR Raycast, đặt Mặt Trời, anchor và reset.
3. Khởi tạo hệ hành tinh, chọn hành tinh và hiển thị panel thông tin.
4. Ổn định anchor/trạng thái, sửa lỗi, tối ưu và thử chỉ dẫn hướng.

## Quy ước Git

- Nhánh chính: `main`
- Nhánh tính năng: `feature/<ten-tinh-nang>`
- Commit ngắn gọn, ví dụ: `feat: add AR placement scene`
- Asset nhị phân lớn được quản lý bằng Git LFS.

## Nội Dung nhánh Đức tuần 2

- Hoàn thiện [`ScaleCalculator.cs`](Assets/Scripts/Core/ScaleCalculator.cs): đổi AU sang mét và tính vị trí theo hướng ngang đã chuẩn hóa.
- Tạo scene [`ScaleTest.unity`](Assets/Scenes/ScaleTest.unity) với 8 hành tinh trên cùng trục; báo cáo [`scale-test-report.md`](docs/scale-test-report.md) ghi nhận 24/24 phép đối chiếu `Transform.position` đạt với gốc, hướng và scale khác nhau.
- Hoàn thiện [`LocationServiceController.cs`](Assets/Scripts/Core/LocationServiceController.cs): yêu cầu quyền vị trí và hiển thị trạng thái, lat/lon, accuracy, timeout cùng thông báo lỗi. Kiểm thử GPS trên điện thoại vẫn chờ thiết bị Android.
