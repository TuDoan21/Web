-- Tạo database
CREATE DATABASE QuanLyBanGiay;
GO
USE QuanLyBanGiay;
GO

-- Bảng LoaiGiay (Category)
CREATE TABLE LoaiGiay (
    MaLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL
);

-- Bảng SanPham (Product)
CREATE TABLE SanPham (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(200) NOT NULL,
    MaLoai INT NOT NULL REFERENCES LoaiGiay(MaLoai),
    Gia DECIMAL(18,2) NOT NULL,
    SoLuong INT NOT NULL,
    ThuongHieu NVARCHAR(100),
    MoTa NVARCHAR(1000),
    AnhUrl NVARCHAR(500)
);

-- Bảng KhachHang / Users (cung quản lý admin bằng IsAdmin)
CREATE TABLE KhachHang (
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash VARBINARY(64) NOT NULL,
    Ten NVARCHAR(200),
    SDT VARCHAR(20),
    DiaChi NVARCHAR(300),
    IsAdmin BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng HoaDon (Orders)
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaKH INT NOT NULL REFERENCES KhachHang(MaKH),
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL,
    PhiShip DECIMAL(18,2) DEFAULT 20000,
    DiaChi NVARCHAR(300),
    SDT VARCHAR(20),
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xác nhận'  
);

-- Bảng ChiTietHoaDon (OrderItems)
CREATE TABLE ChiTietHoaDon (
    MaCTHD INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL REFERENCES HoaDon(MaHD),
    MaSP INT NOT NULL REFERENCES SanPham(MaSP),
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL
);

-- Bảng Cart & CartItems (tạm)
CREATE TABLE Cart (
    CartId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaKH INT NULL REFERENCES KhachHang(MaKH),
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE CartItem (
    CartItemId INT IDENTITY(1,1) PRIMARY KEY,
    CartId UNIQUEIDENTIFIER REFERENCES Cart(CartId),
    MaSP INT REFERENCES SanPham(MaSP),
    SoLuong INT NOT NULL
);

IF TYPE_ID(N'dbo.CartItemType') IS NOT NULL
    DROP TYPE dbo.CartItemType;
GO

CREATE TYPE dbo.CartItemType AS TABLE
(
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL
);
GO
-- Seed: LoaiGiay
INSERT INTO LoaiGiay (TenLoai) VALUES (N'Chạy bộ'), (N'Bóng đá'), (N'Sneaker'), (N'Training');

-- Seed: 20 SanPham (mẫu)
INSERT INTO SanPham (TenSP, MaLoai, Gia, SoLuong, ThuongHieu, MoTa, AnhUrl) VALUES
(N'Nike Air Zoom Pegasus', 1, 2500000, 20, N'Nike', N'Giày chạy bộ nhẹ, êm', N'/images/pegasus.jpg'),
(N'Adidas Ultraboost', 1, 3200000, 15, N'Adidas', N'Đệm boost êm', N'/images/ultraboost.jpg'),
(N'ASICS Gel-Nimbus', 1, 2100000, 12, N'ASICS', N'Ổn định cho chạy dài', N'/images/nimbus.jpg'),
(N'Puma Speedcat', 3, 1200000, 25, N'Puma', N'Sneaker casual', N'/images/speedcat.jpg'),
(N'Converse Chuck Taylor', 3, 900000, 40, N'Converse', N'Classic', N'/images/chuck.jpg'),
(N'Adidas Predator', 2, 2800000, 10, N'Adidas', N'Giày đá bóng sân cỏ', N'/images/predator.jpg'),
(N'Nike Mercurial', 2, 3000000, 8, N'Nike', N'Giày đá bóng tốc độ', N'/images/mercurial.jpg'),
(N'New Balance 990', 1, 1800000, 18, N'NewBalance', N'Ổn định', N'/images/990.jpg'),
(N'Vans Old Skool', 3, 950000, 22, N'Vans', N'Skate/sneaker', N'/images/vans.jpg'),
(N'Under Armour HOVR', 1, 1700000, 14, N'UnderArmour', N'Đệm HOVR', N'/images/hovr.jpg'),
(N'Nike Air Force 1', 3, 1500000, 30, N'Nike', N'Sneaker cổ điển', N'/images/af1.jpg'),
(N'Adidas Samba', 3, 1100000, 26, N'Adidas', N'Casual', N'/images/samba.jpg'),
(N'Puma Future', 2, 2400000, 9, N'Puma', N'Giày đá bóng', N'/images/future.jpg'),
(N'Brooks Ghost', 1, 2000000, 11, N'Brooks', N'Giày chạy độ bền', N'/images/ghost.jpg'),
(N'Hoka Clifton', 1, 2600000, 7, N'Hoka', N'Đệm tốt cho chạy dài', N'/images/clifton.jpg'),
(N'Adidas Nemeziz', 2, 2300000, 6, N'Adidas', N'Giày đá bóng linh hoạt', N'/images/nemeziz.jpg'),
(N'Nike React Infinity', 1, 2700000, 10, N'Nike', N'Đệm React', N'/images/react.jpg'),
(N'Fila Disruptor', 3, 800000, 20, N'Fila', N'Fashion sneaker', N'/images/disruptor.jpg'),
(N'Crocs LiteRide', 3, 500000, 50, N'Crocs', N'Comfort', N'/images/literide.jpg'),
(N'Salomon Speedcross', 1, 2200000, 5, N'Salomon', N'Giày trail', N'/images/speedcross.jpg');

-- Seed: 5 Users (3 khách, 2 admin)
INSERT INTO KhachHang (Email, PasswordHash, Ten, SDT, DiaChi, IsAdmin) VALUES
('kh1@example.com', HASHBYTES('SHA2_256','password1'), N'Nguyễn Văn A','0912345678', N'Hà Nội', 0),
('kh2@example.com', HASHBYTES('SHA2_256','password2'), N'Trần Thị B','0933222111', N'TPHCM', 0),
('kh3@example.com', HASHBYTES('SHA2_256','password3'), N'Lê Văn C','0987654321', N'Đà Nẵng', 0),
('admin1@example.com', HASHBYTES('SHA2_256','adminpass1'), N'Admin One','0900111222', N'HCM', 1),
('admin2@example.com', HASHBYTES('SHA2_256','adminpass2'), N'Admin Two','0900333444', N'HCM', 1);


