# VitaSport - Danh sách chức năng website

## 1. Chức năng tài khoản người dùng
- Đăng ký tài khoản
- Đăng nhập / đăng xuất
- Phân quyền tài khoản:
  - Admin
  - Nhân viên
  - Khách hàng
- Xem thông tin cá nhân
- Cập nhật số điện thoại
- Hiển thị vai trò người dùng trong trang tài khoản

## 2. Quản lý sổ địa chỉ
- Thêm địa chỉ mới
- Xóa địa chỉ
- Chọn địa chỉ mặc định
- Lưu nhiều địa chỉ cho một tài khoản
- Dùng địa chỉ có sẵn khi đặt hàng

## 3. Chức năng sản phẩm
- Xem danh sách sản phẩm
- Xem chi tiết sản phẩm
- Hiển thị hình ảnh, tên, mô tả, giá, số lượng tồn
- Hiển thị trạng thái hết hàng / còn hàng
- Hiển thị danh mục sản phẩm

## 4. Tìm kiếm, lọc và sắp xếp sản phẩm
- Tìm kiếm theo từ khóa
- Lọc theo danh mục
- Lọc theo tình trạng hàng
- Lọc theo khoảng giá
- Lọc nhanh bằng thanh kéo giá
- Sắp xếp theo:
  - Mới nhất
  - Giá tăng dần
  - Giá giảm dần
  - Tên A-Z
  - Tên Z-A
- Phân trang sản phẩm
- AJAX filter không cần reload toàn trang

## 5. Giỏ hàng
- Thêm sản phẩm vào giỏ hàng
- Tăng số lượng sản phẩm
- Giảm số lượng sản phẩm
- Xóa sản phẩm khỏi giỏ hàng
- Lưu giỏ hàng bằng Session
- Kiểm tra tồn kho khi thêm sản phẩm

## 6. Kiểm soát truy cập mua hàng
- Nếu chưa đăng nhập mà bấm mua hàng:
  - Hiển thị popup yêu cầu đăng nhập
- Chặn thao tác thêm vào giỏ hàng khi chưa xác thực tài khoản

## 7. Đặt hàng / Checkout
- Xem lại danh sách sản phẩm trước khi đặt hàng
- Chọn địa chỉ từ sổ địa chỉ
- Nhập địa chỉ giao hàng mới
- Kiểm tra dữ liệu người nhận:
  - Tên người nhận
  - Số điện thoại
  - Địa chỉ chi tiết
  - Phường/Xã
  - Quận/Huyện
  - Tỉnh/Thành phố
- Hỗ trợ đặt hàng theo hình thức COD

## 8. Mã giảm giá
- Nhập mã giảm giá khi checkout
- Kiểm tra hiệu lực mã giảm giá
- Hỗ trợ 2 loại mã:
  - Giảm theo phần trăm
  - Giảm theo số tiền cố định
- Giảm tối đa theo cấu hình
- Đơn hàng tối thiểu để dùng mã
- Giới hạn số lượt sử dụng
- Tự tính số tiền giảm và tổng tiền sau giảm
- Lưu mã giảm giá đã dùng vào đơn hàng

## 9. Quản lý đơn hàng
### Đối với khách hàng
- Xem danh sách đơn hàng của mình
- Xem chi tiết từng đơn hàng
- Xem thông tin người nhận
- Xem địa chỉ giao hàng
- Xem sản phẩm trong đơn hàng
- Xem tổng tiền và trạng thái đơn

### Đối với Admin / Nhân viên
- Quản lý toàn bộ đơn hàng
- Lọc đơn theo trạng thái
- Cập nhật trạng thái đơn hàng:
  - Pending
  - Shipping
  - Completed

## 10. Quản lý mã giảm giá (Admin)
- Tạo mã giảm giá
- Sửa mã giảm giá
- Xóa mã giảm giá
- Bật / tắt mã giảm giá
- Quản lý:
  - Giá trị giảm
  - Loại giảm giá
  - Mức giảm tối đa
  - Đơn tối thiểu
  - Số lượt dùng
  - Thời gian bắt đầu
  - Thời gian kết thúc

## 11. Trang chủ (Homepage)
- Hero banner / slider
- Thanh top bar
- Thanh tìm kiếm nổi bật
- Section danh mục nổi bật
- Section thương hiệu nổi bật
- Section sản phẩm nổi bật
- Section sản phẩm theo từng danh mục:
  - Giày nổi bật
  - Dụng cụ gym nổi bật
  - Bóng thể thao nổi bật
- Section Flash Deal
- Section mã giảm giá hot
- Section đánh giá khách hàng
- Countdown cho deal và voucher

## 12. Giao diện và trải nghiệm người dùng
- Giao diện responsive
- Hiệu ứng hover card sản phẩm
- Carousel sản phẩm nổi bật
- Badge trạng thái sản phẩm
- Popup đăng nhập
- Thiết kế đồng bộ theo phong cách thương mại điện tử hiện đại

## 13. Bảo mật và kỹ thuật
- ASP.NET Core Identity
- Role-based authorization
- Validate dữ liệu phía server
- AntiForgeryToken cho form
- Entity Framework Core
- SQL Server
- Session cho giỏ hàng
- Partial View để render danh sách sản phẩm
- AJAX để lọc sản phẩm mượt hơn

## 14. Hướng phát triển thêm
- Gợi ý sản phẩm thông minh
- Thanh toán online
- Search realtime
- Wishlist
- Đánh giá sản phẩm từ người dùng
