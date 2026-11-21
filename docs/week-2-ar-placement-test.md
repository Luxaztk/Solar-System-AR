# Week 2 - AR Placement Test

Ngày kiểm tra: **21/11/2026**

## Phạm vi

- Scene `ARPlacement`: nhận diện mặt phẳng ngang/dọc và AR raycast.
- Đặt một Mặt Trời vào `ARAnchor` và reset để đặt lại.
- Build/cài prototype Android v0.1 trên thiết bị thật.

## Lỗi đã xác nhận

Logcat của APK cũ (`versionCode 1`) lặp lại lỗi sau mỗi lần đặt:

```text
Can't add component because class 'SphereCollider' doesn't exist!
UnityEngine.GameObject:CreatePrimitive(PrimitiveType)
SolarSystemAR.AR.ARPlacementManager:CreateSunInstance(Transform)
```

Ngay sau lỗi, log vẫn có `[ARPlacement] Sun placed ... with anchor ...` và
`[ARPlacement] Placement reset.`, nên raycast, anchor và reset hoạt động; lỗi nằm
ở placeholder được tạo bằng `GameObject.CreatePrimitive` trong Android player.

Nguyên nhân là IL2CPP engine-code stripping không thấy tham chiếu runtime cụ thể
đến `SphereCollider` và loại type này khỏi player.

## Bản sửa

- Dùng tham chiếu runtime cụ thể `GetComponent<SphereCollider>()` để linker giữ
  type mà `GameObject.CreatePrimitive` cần.
- Tạo và gán sẵn `SunPlaceholder.prefab` không có collider cho scene; nhánh tạo
  primitive chỉ còn là fallback.
- Cố định application ID khi build là `com.solarsystemar.app`.

## Kết quả kiểm tra

- Unity 6000.6.0f1 batch build: **Success**.
- APK: `Solar-System-AR-v0.1.apk`, `versionName 0.1.0`, `versionCode 3`.
- SHA-256: `52B7B325EE327E78228F944DFFA5730559E8FB0A8BE720FD4D60C7113D83D397`.
- Cài đè thành công lên Xiaomi M2012K11C, Android 14.
- IL2CPP output có đăng ký `SphereCollider` trong `UnityClassRegistration.cpp`.
- Phiên khởi động APK mới không còn log lỗi `SphereCollider`.

## Nghiệm thu trên thiết bị

Người dùng đã thao tác trực tiếp trên thiết bị và xác nhận nhìn thấy sphere.
Logcat của đúng tiến trình APK `versionCode 3` ghi nhận:

- `13:27:49`: đặt Mặt Trời và tạo anchor thành công.
- `13:28:45`: reset thành công; `13:28:46`: đặt lại thành công với anchor mới.
- `13:29:29`: reset và đặt lại lần nữa thành công với anchor mới.
- Không có `SphereCollider`, `CreatePrimitive` hoặc `Anchor creation failed`
  trong toàn bộ các lần đặt/reset trên.

Kết luận: regression `SphereCollider` đã được sửa và ba hạng mục AR Placement
thuộc Week 2 đã nghiệm thu **Hoàn thành**.
