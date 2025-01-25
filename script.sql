CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Role VARCHAR(20) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME
);

CREATE TABLE Classes (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassName VARCHAR(50) NOT NULL,
    TeacherID INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (TeacherID) REFERENCES Users(UserID)
);

CREATE TABLE StudentClasses (
    StudentClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT,
    StudentID INT,
    JoinedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID),
    FOREIGN KEY (StudentID) REFERENCES Users(UserID)
);

CREATE TABLE Quizzes (
    QuizID INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    CreatedBy INT,
    TimeLimit INT,
    PassingScore INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
);

CREATE TABLE Questions (
    QuestionID INT IDENTITY(1,1) PRIMARY KEY,
    QuizID INT FOREIGN KEY REFERENCES Quizzes(QuizID),
    QuestionText NVARCHAR(MAX),
    OptionA NVARCHAR(500),
    OptionB NVARCHAR(500),
    OptionC NVARCHAR(500),
    OptionD NVARCHAR(500),
    CorrectAnswer CHAR(1),
    QuestionOrder INT
)

CREATE TABLE Options (
    OptionID INT IDENTITY(1,1) PRIMARY KEY,
    QuestionID INT,
    OptionText TEXT NOT NULL,
    IsCorrect BIT DEFAULT 0,
    OrderNumber INT,
    FOREIGN KEY (QuestionID) REFERENCES Questions(QuestionID)
);

CREATE TABLE QuizResults (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    QuizID INT,
    UserID INT,
    Score INT,
    TimeTaken INT,
    CompletedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (QuizID) REFERENCES Quizzes(QuizID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);