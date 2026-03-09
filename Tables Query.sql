--CREATE TABLE Users
--(
--    UserId BIGINT IDENTITY PRIMARY KEY,
--    FullName NVARCHAR(150),
--    Email NVARCHAR(150) UNIQUE,
--    Mobile NVARCHAR(20),
--    PasswordHash NVARCHAR(500),
--    RoleId INT,

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,

--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT NULL,
--    UpdatedOn DATETIME NULL
--)

--CREATE TABLE Courses
--(
--    CourseId BIGINT IDENTITY PRIMARY KEY,
--    CourseTitle NVARCHAR(200),
--    Description NVARCHAR(MAX),
--    CategoryId INT,
--    InstructorId BIGINT,
--    Price DECIMAL(10,2),
--    Thumbnail NVARCHAR(300),

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,

--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT NULL,
--    UpdatedOn DATETIME NULL
--)

--CREATE TABLE CourseVideos
--(
--    VideoId BIGINT IDENTITY PRIMARY KEY,
--    SectionId BIGINT,
--    Title NVARCHAR(200),
--    VideoUrl NVARCHAR(500),
--    Duration INT,
--    IsDemo BIT,

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,

--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT NULL,
--    UpdatedOn DATETIME NULL
--)



--CREATE TABLE StudentProfile(
--    StudentId BIGINT PRIMARY KEY,
--    DOB DATE,
--    Gender NVARCHAR(10),
--    Address NVARCHAR(300),
--    ProfileImage NVARCHAR(300),

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,
--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT,
--    UpdatedOn DATETIME
--)


--CREATE TABLE InstructorProfile(
--    InstructorId BIGINT PRIMARY KEY,
--    Bio NVARCHAR(MAX),
--    ExperienceYears INT,
--    ProfileImage NVARCHAR(300),

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,
--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT,
--    UpdatedOn DATETIME
--)


--CREATE TABLE CourseCategories(
--    CategoryId INT IDENTITY PRIMARY KEY,
--    CategoryName NVARCHAR(150),
--    ParentCategoryId INT,

--    IsActive BIT DEFAULT 1,
--    IsDeleted BIT DEFAULT 0,
--    CreatedBy BIGINT,
--    CreatedOn DATETIME DEFAULT GETDATE(),
--    UpdatedBy BIGINT,
--    UpdatedOn DATETIME
--)


CREATE TABLE Courses(
    CourseId BIGINT IDENTITY PRIMARY KEY,
    CourseTitle NVARCHAR(200),
    Description NVARCHAR(MAX),
    CategoryId INT,
    InstructorId BIGINT,
    Price DECIMAL(10,2),
    Thumbnail NVARCHAR(300),
    Language NVARCHAR(50),
    Level NVARCHAR(50),
    IsPublished BIT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE CourseSections(
    SectionId BIGINT IDENTITY PRIMARY KEY,
    CourseId BIGINT,
    SectionTitle NVARCHAR(200),
    SortOrder INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE CourseVideos(
    VideoId BIGINT IDENTITY PRIMARY KEY,
    SectionId BIGINT,
    Title NVARCHAR(200),
    VideoUrl NVARCHAR(500),
    Duration INT,
    IsDemo BIT,
    SortOrder INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE CourseAttachments(
    Id BIGINT IDENTITY PRIMARY KEY,
    CourseId BIGINT,
    FileName NVARCHAR(200),
    FileUrl NVARCHAR(500),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE SubscriptionPlans(
    PlanId INT IDENTITY PRIMARY KEY,
    PlanName NVARCHAR(100),
    Price DECIMAL(10,2),
    DurationDays INT,
    Description NVARCHAR(300),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE StudentSubscriptions(
    SubscriptionId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    PlanId INT,
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Payments(
    PaymentId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    SubscriptionId BIGINT,
    Amount DECIMAL(10,2),
    PaymentGateway NVARCHAR(100),
    TransactionId NVARCHAR(200),
    Status NVARCHAR(50),
    PaymentDate DATETIME,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Coupons(
    CouponId INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(50),
    DiscountPercent INT,
    ExpiryDate DATE,
    MaxUsage INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE CouponUsage(
    Id BIGINT IDENTITY PRIMARY KEY,
    CouponId INT,
    StudentId BIGINT,
    UsedDate DATETIME,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE CourseEnrollment(
    EnrollmentId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    CourseId BIGINT,
    EnrollDate DATETIME,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE VideoProgress(
    Id BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    VideoId BIGINT,
    WatchTime INT,
    Completed BIT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE PQJ_Questions(
    QuestionId BIGINT IDENTITY PRIMARY KEY,
    CourseId BIGINT,
    QuestionText NVARCHAR(MAX),
    DifficultyLevel NVARCHAR(50),
    Marks INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE PQJ_Options(
    OptionId BIGINT IDENTITY PRIMARY KEY,
    QuestionId BIGINT,
    OptionText NVARCHAR(300),
    IsCorrect BIT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE PQJ_Attempts(
    AttemptId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    CourseId BIGINT,
    AttemptDate DATETIME,
    Score INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE PQJ_AttemptAnswers(
    Id BIGINT IDENTITY PRIMARY KEY,
    AttemptId BIGINT,
    QuestionId BIGINT,
    SelectedOptionId BIGINT,
    IsCorrect BIT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Certificates(
    CertificateId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    CourseId BIGINT,
    CertificateUrl NVARCHAR(500),
    IssuedDate DATETIME,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Reviews(
    ReviewId BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    CourseId BIGINT,
    Rating INT,
    Comment NVARCHAR(500),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Leaderboard(
    Id BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    CourseId BIGINT,
    Score INT,
    Rank INT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Notifications(
    NotificationId BIGINT IDENTITY PRIMARY KEY,
    Title NVARCHAR(200),
    Message NVARCHAR(MAX),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE StudentNotifications(
    Id BIGINT IDENTITY PRIMARY KEY,
    StudentId BIGINT,
    NotificationId BIGINT,
    IsRead BIT,

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE ContactMessages(
    Id BIGINT IDENTITY PRIMARY KEY,
    Name NVARCHAR(150),
    Email NVARCHAR(150),
    Message NVARCHAR(MAX),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

CREATE TABLE Roles(
    RoleId INT PRIMARY KEY,
    RoleName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO Roles(RoleId,RoleName) VALUES
(1,'Admin'),
(2,'Student'),
(3,'Instructor')

CREATE TABLE GenderMaster(
    GenderId INT PRIMARY KEY,
    GenderName NVARCHAR(20),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO GenderMaster VALUES
(1,'Male',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Female',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Other',1,0,NULL,GETDATE(),NULL,NULL)


CREATE TABLE CourseLevels(
    LevelId INT PRIMARY KEY,
    LevelName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO CourseLevels VALUES
(1,'Beginner',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Intermediate',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Advanced',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE Languages(
    LanguageId INT PRIMARY KEY,
    LanguageName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO Languages VALUES
(1,'English',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Hindi',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Hinglish',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE DifficultyLevels(
    DifficultyId INT PRIMARY KEY,
    DifficultyName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO DifficultyLevels VALUES
(1,'Easy',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Medium',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Hard',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE PaymentStatus(
    StatusId INT PRIMARY KEY,
    StatusName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO PaymentStatus VALUES
(1,'Pending',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Success',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Failed',1,0,NULL,GETDATE(),NULL,NULL),
(4,'Refunded',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE SubscriptionStatus(
    StatusId INT PRIMARY KEY,
    StatusName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO SubscriptionStatus VALUES
(1,'Active',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Expired',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Cancelled',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE NotificationTypes(
    TypeId INT PRIMARY KEY,
    TypeName NVARCHAR(100),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO NotificationTypes VALUES
(1,'General',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Course Update',1,0,NULL,GETDATE(),NULL,NULL),
(3,'Offer',1,0,NULL,GETDATE(),NULL,NULL),
(4,'System Alert',1,0,NULL,GETDATE(),NULL,NULL)

CREATE TABLE CouponTypes(
    CouponTypeId INT PRIMARY KEY,
    TypeName NVARCHAR(50),

    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedBy BIGINT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    UpdatedBy BIGINT,
    UpdatedOn DATETIME
)

INSERT INTO CouponTypes VALUES
(1,'Percentage Discount',1,0,NULL,GETDATE(),NULL,NULL),
(2,'Fixed Amount',1,0,NULL,GETDATE(),NULL,NULL)