CREATE TABLE [dbo].[Roles]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,

    [RoleName] NVARCHAR(50) NOT NULL
)

GO

CREATE UNIQUE INDEX IX_Roles_RoleName
ON dbo.Roles(RoleName);