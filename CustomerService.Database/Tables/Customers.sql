CREATE TABLE [dbo].[Customers]
(
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    UserId              INT NOT NULL UNIQUE,

    CustomerNumber      NVARCHAR(20) NOT NULL UNIQUE,

    FirstName           NVARCHAR(100) NOT NULL,

    LastName            NVARCHAR(100) NOT NULL,

    Email               NVARCHAR(200) NOT NULL,

    PhoneNumber         NVARCHAR(20) NOT NULL,

    DateOfBirth         DATE NULL,

    Gender              NVARCHAR(20) NULL,

    PANNumber           NVARCHAR(20) NULL,

    AadhaarNumber       NVARCHAR(20) NULL,

    Occupation          NVARCHAR(100) NULL,

    AnnualIncome        DECIMAL(18,2) NULL,

    Address             NVARCHAR(300) NULL,

    City                NVARCHAR(100) NULL,

    State               NVARCHAR(100) NULL,

    Country             NVARCHAR(100) NULL,

    PostalCode          NVARCHAR(20) NULL,

    IsActive            BIT NOT NULL DEFAULT(1),

    CreatedDate         DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),

    CreatedBy           NVARCHAR(100) NULL,

    ModifiedDate        DATETIME2 NULL,

    ModifiedBy          NVARCHAR(100) NULL,

    IsDeleted           BIT NOT NULL DEFAULT(0)
);