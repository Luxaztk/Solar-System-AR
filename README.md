# Solar System AR

Ứng dụng Android dùng AR Foundation để đặt và khám phá mô hình Hệ Mặt Trời trong không gian thực.

## Trạng thái hiện tại

- Đã scaffold Unity project bằng template **3D URP**.
- Đã cấu hình Unity **6000.6.0f1** và Android Build Support.
- Đã khai báo AR Foundation và Google ARCore XR Plugin **6.7.1**.
- Đã cấu hình Android package `com.solarsystemar.app`, IL2CPP và ARM64.
- Package restore đã hoàn tất và project mở không có lỗi compile. Chưa bật ARCore loader hoặc xác nhận camera AR trên thiết bị thật.

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

## Nội dung nhánh Hoang (tuần 1–4)

- Scene nghiệm thu: `Assets/Scenes/HoangFeatureDemo.unity`.
- Dữ liệu chuẩn hóa: `Assets/Data/Planets/` và `Assets/Data/quiz.vi.json`.
- Bộ 9 prefab: `Assets/Prefabs/Planets/`.
- UI: Start Menu, Tutorial, Planet Info Panel, nhãn 3D, Discovery Progress và Quiz.
- Tài liệu yêu cầu, user flow, dữ liệu, asset audit và usability review: `docs/hoang/`.

Mở scene demo và bấm Play để thử menu; sau khi bỏ qua hướng dẫn, chạm vào một hành tinh để mở panel thông tin. Có thể tái tạo toàn bộ asset bằng menu **Solar System AR > Generate Hoang Week 1-4 Content**.

## Quy ước Git

- Nhánh chính: `main`
- Nhánh tính năng: `feature/<ten-tinh-nang>`
- Commit ngắn gọn, ví dụ: `feat: add AR placement scene`
- Asset nhị phân lớn được quản lý bằng Git LFS.
