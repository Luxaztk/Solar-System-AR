# Bảng dữ liệu 9 thiên thể

Dữ liệu số dùng cho nội dung giáo dục và UI; giá trị được làm tròn phù hợp màn hình di động. AU là khoảng cách quỹ đạo trung bình, không phải khoảng cách tức thời.

| ID | Tên hiển thị | AU | Bán kính (km) | Chu kỳ quỹ đạo (ngày) | Tự quay (giờ) | Vệ tinh* | Vành đai |
|---|---|---:|---:|---:|---:|---:|---|
| sun | Mặt Trời | 0 | 695700 | 0 | 609.1 | 0 | Không |
| mercury | Sao Thủy | 0.387 | 2439.7 | 88.0 | 1407.6 | 0 | Không |
| venus | Sao Kim | 0.723 | 6051.8 | 224.7 | 5832.5 | 0 | Không |
| earth | Trái Đất | 1.000 | 6371.0 | 365.26 | 23.93 | 1 | Không |
| mars | Sao Hỏa | 1.524 | 3389.5 | 687.0 | 24.62 | 2 | Không |
| jupiter | Sao Mộc | 5.203 | 69911 | 4332.6 | 9.93 | 95 | Có |
| saturn | Sao Thổ | 9.537 | 58232 | 10759.2 | 10.7 | 146 | Có |
| uranus | Sao Thiên Vương | 19.191 | 25362 | 30688.5 | 17.24 | 27 | Có |
| neptune | Sao Hải Vương | 30.069 | 24622 | 60182 | 16.11 | 14 | Có |

\* Số vệ tinh là dữ liệu có thể thay đổi khi có phát hiện mới; nên rà soát lại trước bản phát hành.

## Nguồn và giấy phép

- NASA NSSDC, Planetary Fact Sheet (metric): <https://nssdc.gsfc.nasa.gov/planetary/factsheet/>
- NASA Science, Solar System Facts: <https://science.nasa.gov/solar-system/solar-system-facts/>
- NASA Science, Sun Facts: <https://science.nasa.gov/sun/facts/>

Prefab hiện tại chỉ dùng Unity primitive + material tự tạo. Không có texture hoặc model bên thứ ba, nên không phát sinh yêu cầu attribution cho asset hình ảnh.
