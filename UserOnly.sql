USE master;
GO

DROP DATABASE IF EXISTS UserOnly;
GO

CREATE DATABASE UserOnly;
GO

USE UserOnly;
GO

CREATE TABLE Account
(
    UserId UNIQUEIDENTIFIER NOT NULL  PRIMARY KEY DEFAULT NEWSEQUENTIALID(),  
    Username VARCHAR(255) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Avatar NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);
GO


INSERT INTO Account (Username, Password, Avatar, UpdatedAt)
VALUES 
-- 1. Người dùng Admin, có Avatar, chưa cập nhật lần nào (UpdatedAt là NULL)
('admin', '1', 'https://example.com/avatars/admin.png', NULL),

-- 2. Người dùng bình thường, không có Avatar (NULL), đã cập nhật
('user1', '1', NULL, GETDATE()),

-- 3. Người dùng khác, có Avatar
('user2', '1', 'https://example.com/avatars/b.jpg', NULL),

-- 4. Người dùng với tên đơn giản
('user3', '1', NULL, NULL),

-- 5. Người dùng mới tạo
('user4', '1', 'https://example.com/avatars/c.png', NULL);
GO

select * from Account