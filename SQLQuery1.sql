﻿-- Tạo bảng NHANVIEN
CREATE TABLE NHANVIEN (
    MSNV NVARCHAR(10) PRIMARY KEY,
    HOTEN NVARCHAR(100),
    GIOITINH CHAR(3),
    NGAYSINH DATE,
    HINHANH VARCHAR(MAX),
    QUEQUAN NVARCHAR(100),
    TAMTRU NVARCHAR(100),
    SDT VARCHAR(15),
    EMAIL VARCHAR(100),
    DANTOC VARCHAR(50),
    HOCVAN NVARCHAR(100),
    NGAYVAOLAM DATE,
    CCCD VARCHAR(20),
    MSHD VARCHAR(10)
);
ALTER TABLE NHANVIEN
ALTER COLUMN GIOITINH NVARCHAR(10);

--ALTER TABLE NHANVIEN
--ALTER COLUMN HINHANH VARBINARY(MAX);

-- Tạo bảng tạm mới với kiểu dữ liệu VARBINARY(MAX) cho cột HINHANH
--CREATE TABLE NHANVIEN_TMP (
--    MSNV NVARCHAR(10) PRIMARY KEY,
--    HOTEN NVARCHAR(100),
--    GIOITINH CHAR(3),
--    NGAYSINH DATE,
--    HINHANH VARBINARY(MAX), -- Kiểu dữ liệu đã được thay đổi
--    QUEQUAN NVARCHAR(100),
--    TAMTRU NVARCHAR(100),
--    SDT VARCHAR(15),
--    EMAIL VARCHAR(100),
--    DANTOC VARCHAR(50),
--    HOCVAN NVARCHAR(100),
--    NGAYVAOLAM DATE,
--    CCCD VARCHAR(20),
--    MSHD VARCHAR(10)
--);
-- Chuyển dữ liệu từ bảng NHANVIEN sang bảng NHANVIEN_TMP và chuyển đổi kiểu dữ liệu của cột HINHANH
--INSERT INTO NHANVIEN_TMP (MSNV, HOTEN, GIOITINH, NGAYSINH, HINHANH, QUEQUAN, TAMTRU, SDT, EMAIL, DANTOC, HOCVAN, NGAYVAOLAM, CCCD, MSHD)
--SELECT MSNV, HOTEN, GIOITINH, NGAYSINH, CONVERT(VARBINARY(MAX), HINHANH), QUEQUAN, TAMTRU, SDT, EMAIL, DANTOC, HOCVAN, NGAYVAOLAM, CCCD, MSHD
--FROM NHANVIEN;

-- Tạo bảng HOCVANKINHNGHIEM
CREATE TABLE HOCVANKINHNGHIEM (
	HOCVAN NVARCHAR(100) PRIMARY KEY,
    MSNV NVARCHAR(10),
    KINHNGHIEM INT,
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);

-- Tạo bảng CHUCVU
CREATE TABLE CHUCVU (
    IDCHUCVU INT PRIMARY KEY,
    MSNV NVARCHAR(10),
    VITRI NVARCHAR(100),
    CHUCVU NVARCHAR(100),
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);

---- Tạo bảng CHAMCONG
--CREATE TABLE CHAMCONG (
--    IDCC INT PRIMARY KEY,
--    MSNV NVARCHAR(10),
--    CHAMCONG INT,
--    NGAY DATE,
--    GIOCHAMCONG TIME,
--    LOAI VARCHAR(50),
--    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
--);
-- Tạo bảng CHAMCONG
CREATE TABLE CHAMCONG (
    IDCC INT PRIMARY KEY IDENTITY,
    MSNV NVARCHAR(10),
    NGAY DATE,
    GIOCHAMCONG TIME,
    LOAI VARCHAR(50),
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);

CREATE TABLE LUONG(
    IDL INT PRIMARY KEY IDENTITY,
    MSNV NVARCHAR(10),
    LCB INT,
    SONGAYLAM INT,
    BHYT INT,
    BHXH INT,
    BHTN INT,
    TONGPHUCAP INT,
    SOGIOTANGCA INT,
    TONGTIENTANGCA INT,
    TONGTIENNHAN INT,
	THANG DATETIME,
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);
---- Tạo bảng LUONG
--CREATE TABLE LUONG (
--    IDL INT PRIMARY KEY,
--    MSNV NVARCHAR(10),
--    LCB INT,
--    SONGAYLAM INT,
--    BHYT INT,
--    BHXH INT,
--    BHTN INT,
--    TONGPHUCAP INT,
--    SOGIOTANGCA INT,
--    TONGTIENTANGCA INT,
--    TONGTIENNHAN INT,
--    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
--);

-- Tạo bảng TANGCA
CREATE TABLE TANGCA (
    IDTC INT PRIMARY KEY,
    MSNV NVARCHAR(10),
    GIOBATDAU TIME,
    GIOKETTHUC TIME,
    SOGIO INT,
    NGAY DATE,
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);

-- Tạo bảng QUANLYNGHIPHEP
CREATE TABLE QUANLYNGHIPHEP (
    MANP INT PRIMARY KEY,
    MSNV NVARCHAR(10),
    LOAINGHI NVARCHAR(50),
    NGAYBATDAU DATE,
    NGAYKETTHUC DATE,
    SONGAYNGHI INT,
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);

---- Tạo bảng HOPDONGLAODONG
--CREATE TABLE HOPDONGLAODONG (
--    MAHOPDONG INT PRIMARY KEY,
--    MSNV NVARCHAR(10),
--    LOAIHOPDONG NVARCHAR(50),
--    NGAYKY DATE,
--    NGAYHETHAN DATE,
--    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
--);

-- Tạo bảng HOPDONGLAODONG
CREATE TABLE HOPDONGLAODONG (
    MAHOPDONG INT PRIMARY KEY  IDENTITY,
    MSNV NVARCHAR(10),
    LOAIHOPDONG NVARCHAR(50),
    NGAYKY DATE,
    NGAYHETHAN DATE,
	LUONGCOBAN INT,
    FOREIGN KEY (MSNV) REFERENCES NHANVIEN(MSNV)
);
INSERT INTO HOPDONGLAODONG_TEMP (MSNV, LOAIHOPDONG, NGAYKY, NGAYHETHAN)
SELECT MSNV, LOAIHOPDONG, NGAYKY, NGAYHETHAN
FROM HOPDONGLAODONG;
DROP TABLE HOPDONGLAODONG;

EXEC sp_rename 'HOPDONGLAODONG_TEMP', 'HOPDONGLAODONG';
DBCC CHECKIDENT ('HOPDONGLAODONG', RESEED, 3)

--tạo bảng admin
CREATE TABLE ADMIN(
	MAADMIN VARCHAR(20)PRIMARY KEY,
	MATKHAU VARCHAR(20)
)
DROP TABLE ADMIN

--dữ liệu
INSERT INTO NHANVIEN (MSNV, HOTEN, GIOITINH, NGAYSINH, HINHANH, QUEQUAN, TAMTRU, SDT, EMAIL, DANTOC, HOCVAN, NGAYVAOLAM, CCCD, MSHD) 
VALUES 
('NV001', N'Nguyễn Văn A', N'Nam', '1990-05-15', 'nv001.jpg', N'Hà Nội', N'Hồ Chí Minh', '0987654321', 'nva@example.com', N'Kinh', N'Cử nhân CNTT', '2020-01-10', '123456789', 'HD001'),
('NV002', N'Trần Thị B', N'Nữ', '1985-08-20', 'nv002.jpg', N'Hải Phòng', N'Đà Nẵng', '0123456789', 'ttb@example.com', N'Kinh', N'Thạc sĩ Kinh doanh', '2018-03-05', '987654321', 'HD002'),
('NV003', N'Lê Văn C', N'Nam', '1995-11-12', 'nv003.jpg', N'Nghệ An', N'Hà Nội', '0369876543', 'lvc@example.com', N'Kinh', N'Cử nhân Luật', '2019-07-20', '456123789', 'HD003');

INSERT INTO HOCVANKINHNGHIEM (HOCVAN, MSNV, KINHNGHIEM)
VALUES
(N'Cử nhân CNTT', 'NV001', 5),
(N'Thạc sĩ Kinh doanh', 'NV002', 8),
(N'Cử nhân Luật', 'NV003', 3);

INSERT INTO CHUCVU (IDCHUCVU, MSNV, VITRI, CHUCVU)
VALUES
(1, 'NV001', N'Trưởng phòng', N'Quản lý'),
(2, 'NV002', N'Nhân viên', N'Nhân sự'),
(3, 'NV003', N'Kế toán trưởng', N'Tài chính');

INSERT INTO CHAMCONG (IDCC, MSNV, CHAMCONG, NGAY, GIOCHAMCONG, LOAI)
VALUES
(1, 'NV001', 8, '2024-04-15', '08:00:00', N'Chính thức'),
(2, 'NV002', 7, '2024-04-15', '08:30:00', N'Chính thức'),
(3, 'NV003', 6, '2024-04-15', '08:15:00', N'Chính thức');

INSERT INTO LUONG (IDL, MSNV, LCB, SONGAYLAM, BHYT, BHXH, BHTN, TONGPHUCAP, SOGIOTANGCA, TONGTIENTANGCA, TONGTIENNHAN)
VALUES
(1, 'NV001', 15000000, 22, 1000000, 500000, 200000, 3000000, 10, 500000, 17700000),
(2, 'NV002', 12000000, 20, 800000, 400000, 150000, 2500000, 8, 400000, 13800000),
(3, 'NV003', 10000000, 25, 700000, 300000, 100000, 2000000, 12, 600000, 12800000);

INSERT INTO TANGCA (IDTC, MSNV, GIOBATDAU, GIOKETTHUC, SOGIO, NGAY)
VALUES
(1, 'NV001', '18:00:00', '21:00:00', 3, '2024-04-10'),
(2, 'NV002', '19:30:00', '22:30:00', 3, '2024-04-10'),
(3, 'NV003', '20:00:00', '23:00:00', 3, '2024-04-10');

INSERT INTO QUANLYNGHIPHEP (MANP, MSNV, LOAINGHI, NGAYBATDAU, NGAYKETTHUC, SONGAYNGHI)
VALUES
(1, 'NV001', N'Nghỉ phép năm', '2024-03-20', '2024-03-25', 6),
(2, 'NV002', N'Nghỉ ốm', '2024-04-05', '2024-04-07', 3),
(3, 'NV003', N'Nghỉ không lương', '2024-04-12', '2024-04-13', 2);

INSERT INTO HOPDONGLAODONG (MAHOPDONG, MSNV, LOAIHOPDONG, NGAYKY, NGAYHETHAN)
VALUES
(1, 'NV001', N'Hợp đồng lao động chính thức', '2023-01-05', '2026-01-05'),
(2, 'NV002', N'Hợp đồng lao động thử việc', '2022-12-10', '2023-03-10'),
(3, 'NV003', N'Hợp đồng lao động xác định thời hạn', '2024-02-15', '2025-02-15');

DELETE FROM NHANVIEN;


INSERT INTO CHAMCONG_TEMP (MSNV, NGAY, GIOCHAMCONG, LOAI)
SELECT MSNV , NGAY, GIOCHAMCONG, LOAI
FROM CHAMCONG;
DROP TABLE CHAMCONG;

EXEC sp_rename 'CHAMCONG_TEMP', 'CHAMCONG';


--INSERT INTO LUONG_TEMP (MSNV, LCB, SONGAYLAM, BHYT, BHXH, BHTN, TONGPHUCAP, SOGIOTANGCA, TONGTIENTANGCA, TONGTIENNHAN)
--SELECT MSNV, LCB, SONGAYLAM, BHYT, BHXH, BHTN, TONGPHUCAP, SOGIOTANGCA, TONGTIENTANGCA, TONGTIENNHAN
--FROM LUONG;
--DROP TABLE LUONG;
--EXEC sp_rename 'LUONG_TEMP', 'LUONG';

DBCC CHECKIDENT ('LUONG', RESEED, 1)
DBCC CHECKIDENT ('CHAMCONG', RESEED, 4)

DELETE FROM LUONG;