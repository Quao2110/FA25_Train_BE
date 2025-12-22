USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Fakebook')
BEGIN
    ALTER DATABASE Fakebook SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Fakebook;
END
GO

CREATE DATABASE Fakebook;
GO

USE Fakebook;
GO

-- =======================================================
-- 1. USER MANAGEMENT & PROFILE (Tách Profile để tối ưu)
-- =======================================================

CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(500) NOT NULL,
    PhoneNumber VARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    LastLogin DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2
);

CREATE TABLE UserProfiles (
    ProfileId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    DisplayName NVARCHAR(100), 
    AvatarUrl NVARCHAR(500),
    CoverPhotoUrl NVARCHAR(500),
    Bio NVARCHAR(500),
    DateOfBirth DATE,
    Gender NVARCHAR(20),
    City NVARCHAR(100),
    Workplace NVARCHAR(100),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);


-- =======================================================
-- 2. SOCIAL GRAPH (FRIENDS & FOLLOWERS)
-- =======================================================

CREATE TABLE Friendships (
    RequesterId UNIQUEIDENTIFIER NOT NULL,
    AddresseeId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Pending', 'Accepted', 'Blocked', 'Declined')),
    ActionUserId UNIQUEIDENTIFIER NOT NULL, -- Ai là người thực hiện hành động cuối (để biết ai block ai)
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    
    PRIMARY KEY (RequesterId, AddresseeId),
    FOREIGN KEY (RequesterId) REFERENCES Users(UserId),
    FOREIGN KEY (AddresseeId) REFERENCES Users(UserId)
);

-- Index tìm bạn bè cho nhanh
CREATE NONCLUSTERED INDEX IX_Friendships_User ON Friendships(RequesterId, AddresseeId, Status);

-- =======================================================
-- 3. CONTENT CORE (POSTS & MEDIA)
-- =======================================================

CREATE TABLE Posts (
    PostId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(MAX),
    PrivacyLevel VARCHAR(20) DEFAULT 'Public' CHECK (PrivacyLevel IN ('Public', 'Friends', 'OnlyMe')),
    
    -- Support Share: Nếu bài này là share từ bài khác
    OriginalPostId UNIQUEIDENTIFIER NULL, 
    
    -- Counters (Denormalization để tăng tốc độ hiển thị Feed thay vì count(*))
    LikeCount INT DEFAULT 0,
    CommentCount INT DEFAULT 0,
    ShareCount INT DEFAULT 0,
    
    IsDeleted BIT DEFAULT 0, -- Soft Delete
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (OriginalPostId) REFERENCES Posts(PostId) -- Self Reference
);

-- Bảng chứa file đa phương tiện (Ảnh, Video) cho Post
CREATE TABLE PostMedia (
    MediaId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PostId UNIQUEIDENTIFIER NOT NULL,
    MediaUrl NVARCHAR(MAX) NOT NULL,
    MediaType VARCHAR(20) CHECK (MediaType IN ('Image', 'Video')),
    DisplayOrder INT DEFAULT 0, -- Thứ tự hiển thị ảnh trong album
    FOREIGN KEY (PostId) REFERENCES Posts(PostId) ON DELETE CASCADE
);

-- =======================================================
-- 4. REELS & STORIES (Short Video & Ephemeral Content)
-- =======================================================

CREATE TABLE Reels (
    ReelId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Caption NVARCHAR(500),
    VideoUrl NVARCHAR(MAX) NOT NULL,
    ThumbnailUrl NVARCHAR(MAX),
    DurationSeconds INT,
    MusicTrack NVARCHAR(200),
    ViewCount INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Stories (
    StoryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    MediaUrl NVARCHAR(MAX) NOT NULL,
    MediaType VARCHAR(20) CHECK (MediaType IN ('Image', 'Video')),
    ExpiresAt DATETIME2 NOT NULL, -- Story tự xóa sau 24h
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- =======================================================
-- 5. INTERACTIONS (COMMENTS & REACTIONS)
-- =======================================================

CREATE TABLE Comments (
    CommentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PostId UNIQUEIDENTIFIER NULL, -- Comment vào Post
    ReelId UNIQUEIDENTIFIER NULL, -- Comment vào Reel
    UserId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(MAX),
    ParentCommentId UNIQUEIDENTIFIER NULL, -- Reply comment
    MediaUrl NVARCHAR(MAX) NULL, -- Comment bằng ảnh
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (PostId) REFERENCES Posts(PostId),
    FOREIGN KEY (ReelId) REFERENCES Reels(ReelId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ParentCommentId) REFERENCES Comments(CommentId)
);

-- Reactions (Like, Love, Haha...) - Dùng chung cho Post, Comment, Reel
CREATE TABLE Reactions (
    ReactionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Type VARCHAR(20) CHECK (Type IN ('Like', 'Love', 'Haha', 'Wow', 'Sad', 'Angry')),
    
    -- Polymorphic Associations (Nullable FKs)
    PostId UNIQUEIDENTIFIER NULL,
    CommentId UNIQUEIDENTIFIER NULL,
    ReelId UNIQUEIDENTIFIER NULL,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Ràng buộc: Một record chỉ được thuộc về 1 loại đối tượng
    CHECK (
        (PostId IS NOT NULL AND CommentId IS NULL AND ReelId IS NULL) OR
        (PostId IS NULL AND CommentId IS NOT NULL AND ReelId IS NULL) OR
        (PostId IS NULL AND CommentId IS NULL AND ReelId IS NOT NULL)
    ),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PostId) REFERENCES Posts(PostId),
    FOREIGN KEY (CommentId) REFERENCES Comments(CommentId),
    FOREIGN KEY (ReelId) REFERENCES Reels(ReelId)
);

-- =======================================================
-- 6. MESSAGING (CHATTING)
-- =======================================================

-- Cuộc hội thoại (Nhóm hoặc 1-1)
CREATE TABLE Conversations (
    ConversationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(100) NULL, -- Tên nhóm chat (Null nếu chat 1-1)
    IsGroup BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL -- Để sort inbox theo tin nhắn mới nhất
);

-- Thành viên trong cuộc hội thoại
CREATE TABLE ConversationParticipants (
    ConversationId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    JoinedAt DATETIME2 DEFAULT GETUTCDATE(),
    Role VARCHAR(20) DEFAULT 'Member', -- Admin, Member
    PRIMARY KEY (ConversationId, UserId),
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Tin nhắn
CREATE TABLE Messages (
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    ConversationId UNIQUEIDENTIFIER NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(MAX), -- Text
    MediaUrl NVARCHAR(MAX) NULL, -- Ảnh/Video/File
    IsRead BIT DEFAULT 0, -- Đánh dấu đã đọc (Simple version)
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId) ON DELETE CASCADE,
    FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

-- =======================================================
-- 7. NOTIFICATIONS
-- =======================================================

CREATE TABLE Notifications (
    NotificationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId UNIQUEIDENTIFIER NOT NULL, -- Người nhận thông báo
    ActorId UNIQUEIDENTIFIER NOT NULL, -- Người gây ra thông báo (VD: A like bài của B -> Actor là A)
    Type VARCHAR(50) NOT NULL, -- 'LikePost', 'CommentPost', 'FriendRequest', 'MissedCall'
    
    -- Reference ID để click vào nhảy tới trang tương ứng
    ReferenceId UNIQUEIDENTIFIER NULL, -- Có thể là PostId, CommentId, hoặc UserId
    ReferenceType VARCHAR(20), -- 'Post', 'Comment', 'User'
    
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(UserId), -- No Cascade để giữ lịch sử
    FOREIGN KEY (ActorId) REFERENCES Users(UserId)
);

GO

-- 1. Tối ưu bảng PostMedia (Lấy ảnh của bài viết nhanh hơn)
CREATE NONCLUSTERED INDEX IX_PostMedia_PostId ON PostMedia(PostId);

-- 2. Tối ưu bảng Comments (Lấy comment của bài viết nhanh hơn)
-- Bao gồm cả CreatedAt để sắp xếp comment cũ -> mới
CREATE NONCLUSTERED INDEX IX_Comments_PostId_CreatedAt ON Comments(PostId, CreatedAt);
CREATE NONCLUSTERED INDEX IX_Comments_UserId ON Comments(UserId); -- Để xem lịch sử comment của user

-- 3. Tối ưu bảng Reactions (Đếm like hoặc kiểm tra user đã like chưa)
CREATE NONCLUSTERED INDEX IX_Reactions_PostId_Type ON Reactions(PostId, Type);
CREATE NONCLUSTERED INDEX IX_Reactions_UserId ON Reactions(UserId);

-- 4. Tối ưu bảng Notifications (Lấy thông báo cho User)
CREATE NONCLUSTERED INDEX IX_Notifications_UserId_CreatedAt ON Notifications(UserId, CreatedAt DESC);

-- 5. Tối ưu bảng Messages (Load tin nhắn trong hội thoại)
CREATE NONCLUSTERED INDEX IX_Messages_ConversationId_CreatedAt ON Messages(ConversationId, CreatedAt DESC);

-- Index này giúp lọc bài viết theo User và sắp xếp ngày tháng cực nhanh
-- INCLUDE (PrivacyLevel) giúp query lấy thông tin này mà không cần quay lại bảng chính (Lookup)
CREATE NONCLUSTERED INDEX IX_Posts_UserId_CreatedAt 
ON Posts(UserId, CreatedAt DESC) 
INCLUDE (PrivacyLevel, IsDeleted);


DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @NormalUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Users (UserId, Username, Email, Password, PhoneNumber, IsActive, CreatedAt)
VALUES 
-- Admin Account
(@AdminId, 'admin', 'admin@fakebook.com', '$2a$11$Z...HASH_MAT_KHAU_ADMIN...', '0909000111', 1, GETUTCDATE()),

-- User Account
(@NormalUserId, 'user', 'user@fakebook.com', '$2a$11$Z...HASH_MAT_KHAU_USER...', '0909000222', 1, GETUTCDATE());

-- =======================================================
-- 2. TẠO PROFILE (TABLE USERPROFILES)
-- =======================================================

INSERT INTO UserProfiles (UserId, FirstName, LastName, AvatarUrl, CoverPhotoUrl, Bio, City, Workplace, DateOfBirth)
VALUES 
-- Profile cho Admin
(@AdminId, N'Quản Trị', N'Viên', 
 N'https://ui-avatars.com/api/?name=Admin+System&background=0D8ABC&color=fff', -- Link tạo avatar tự động
 N'https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5', 
 N'Hệ thống quản trị Fakebook. Vui lòng liên hệ nếu gặp sự cố.', 
 N'Hồ Chí Minh', N'Fakebook HQ', '1990-01-01'),

-- Profile cho User thường
(@NormalUserId, N'Nguyễn', N'Văn A', 
 N'https://ui-avatars.com/api/?name=Nguyen+Van+A&background=random', 
 N'https://images.unsplash.com/photo-1493246507139-91e8fad9978e', 
 N'Yêu màu tím, thích sự thủy chung. Sống nội tâm hay khóc thầm.', 
 N'Hà Nội', N'Freelancer', '2000-05-15');

-- =======================================================
-- 3. KIỂM TRA DỮ LIỆU
-- =======================================================
SELECT  u.UserId, u.Username, u.Email, p.FirstName, p.LastName, p.DisplayName 
FROM Users u
JOIN UserProfiles p ON u.UserId = p.UserId;