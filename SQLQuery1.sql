use QuanlyCafe;

CREATE TABLE Customer
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Points INT DEFAULT 0,
    CreatedTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME NULL
);

CREATE TABLE Menu
(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    Price INT,
    Category NVARCHAR(50),
    CreatedTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME NULL
);
select * from Menu;
select * from Customer;

CREATE TABLE Invoice   -- hóa đơn
(
    Id INT IDENTITY PRIMARY KEY,          -- Mã hóa đơn

    CustomerName NVARCHAR(100),            -- Tên khách
    Phone VARCHAR(20),                     -- SĐT khách

    TotalAmount INT,                       -- Tổng tiền (29000)
    Status NVARCHAR(50),                   -- Chờ xử lý / Đã thanh toán / Hủy

    CreatedTime DATETIME DEFAULT GETDATE(),-- Ngày tạo hóa đơn
    UpdateTime DATETIME NULL               -- Ngày cập nhật
);