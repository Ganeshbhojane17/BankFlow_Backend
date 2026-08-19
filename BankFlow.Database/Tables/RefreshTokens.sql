CREATE TABLE [dbo].[RefreshTokens]
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,

    Token NVARCHAR(500) NOT NULL,

    ExpiryDate DATETIME2 NOT NULL,

    IsRevoked BIT NOT NULL DEFAULT(0),

    CreatedDate DATETIME2 NOT NULL DEFAULT(GETDATE()),

    RevokedDate DATETIME2 NULL,

    CONSTRAINT FK_RefreshTokens_Users
        FOREIGN KEY(UserId)
        REFERENCES Users(Id)
);