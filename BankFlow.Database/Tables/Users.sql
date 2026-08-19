CREATE TABLE [dbo].[Users]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    [FirstName] NVARCHAR(100) NOT NULL,

    [LastName] NVARCHAR(100) NOT NULL,

    [Email] NVARCHAR(200) NOT NULL,

    [PasswordHash] NVARCHAR(MAX) NOT NULL,

    [Role] NVARCHAR(50) NOT NULL,

    [IsActive] BIT NOT NULL DEFAULT(1),

    [CreatedDate] DATETIME2 NOT NULL DEFAULT(GETDATE()),

    [ModifiedDate] DATETIME2 NULL
)

GO

CREATE UNIQUE INDEX IX_Users_Email
ON dbo.Users(Email);
