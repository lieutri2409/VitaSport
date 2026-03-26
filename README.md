# 🏪 VitaSport - E-commerce Website (ASP.NET Core MVC)

## 📌 Giới thiệu

VitaSport là website thương mại điện tử bán đồ thể thao, được xây dựng bằng ASP.NET Core MVC.

Hệ thống hỗ trợ:

* Mua hàng online
* Quản lý giỏ hàng
* Đặt hàng (COD)
* Mã giảm giá
* Sổ địa chỉ
* Quản trị hệ thống

---

## 🧑‍💻 Công nghệ sử dụng

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* Bootstrap 5
* jQuery / AJAX
* Identity (Authentication & Authorization)

---

## 🚀 Cách chạy project

### 1. Clone project

```bash
git clone https://github.com/lieutri2409/VitaSport
cd VitaSport
```

### 2. Mở bằng Visual Studio

* Open solution `.sln`
* Restore NuGet packages (auto)

---

### 3. Cấu hình database

Mở file:

```
appsettings.json
```

Sửa connection string:

```json
"ConnectionStrings": {
  "DbConnection": "Server=.;Database=VitaSport;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### 4. Update database

```bash
Update-Database
```

---

### 5. Chạy project

```bash
Ctrl + F5
```

---

## 🔐 Tài khoản mặc định

| Role  | Email                                     | Password  |
| ----- | ----------------------------------------- | --------- |
| Admin | [admin@gmail.com](mailto:admin@gmail.com) | Admin@123 |

---

## 🛠️ Chức năng chính

### 👤 Người dùng

* Đăng ký / đăng nhập
* Xem sản phẩm
* Tìm kiếm + lọc sản phẩm
* Thêm vào giỏ hàng
* Đặt hàng (COD)
* Áp mã giảm giá
* Quản lý địa chỉ

---

### 🛒 Giỏ hàng

* Thêm / xóa sản phẩm
* Tăng giảm số lượng
* Kiểm tra tồn kho

---

### 📦 Đặt hàng

* Chọn địa chỉ có sẵn hoặc nhập mới
* Áp mã giảm giá
* Lưu thông tin giao hàng
* Thanh toán COD

---

### 🎁 Mã giảm giá

* Admin tạo mã
* Hỗ trợ:

  * % hoặc số tiền
  * Giảm tối đa
  * Số lượt dùng
  * Thời hạn

---

### 🏠 Trang chủ (Homepage)

* Hero banner
* Search sản phẩm
* Danh mục
* Sản phẩm nổi bật
* Sản phẩm theo từng loại:

  * Giày
  * Gym
  * Bóng
* Flash sale + countdown
* Mã giảm giá HOT

---

## 🔥 NOTE (để làm báo cáo)

### 📌 Search & Filter

* Sử dụng `GET method`
* Query truyền qua URL
* Có AJAX để không reload trang

---

### 📌 Checkout

* Có 2 cách nhập địa chỉ:

  1. Sổ địa chỉ
  2. Nhập mới (fallback)

---

### 📌 Coupon

* Áp dụng ở server (PlaceOrder)
* Tránh gian lận phía client

---

### 📌 Security

* Dùng Identity
* Role-based (Admin / User)
* AntiForgeryToken

---

### 📌 UX/UI

* Bootstrap 5
* Responsive
* Hiệu ứng hover + animation

---

## 👥 Workflow làm việc nhóm (QUAN TRỌNG)

### 🔁 Quy trình chuẩn:


git pull
(sửa code)
git add .
git commit -m "message"
git push


### ⚠️ Lưu ý:

* LUÔN `git pull` trước khi code
* Không sửa trực tiếp file người khác đang làm
* Commit rõ ràng

---

## 🧠 Định hướng nâng cấp

* Search realtime (AJAX)
* AI recommend sản phẩm
* Thanh toán online (VNPay, Momo)
* Chat hỗ trợ khách hàng

---

## 👨‍🎓 Thông tin project

* Môn: Lập trình .NET
* Mục tiêu: Website thương mại điện tử hoàn chỉnh
* Kiến trúc: MVC + EF Core

---

## 📞 Liên hệ

Nếu có vấn đề khi chạy project:
→ liên hệ leader team 😎

---

# 🎯 DONE

Project đã sẵn sàng để phát triển và demo 🚀
