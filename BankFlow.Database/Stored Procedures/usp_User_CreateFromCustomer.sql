CREATE PROCEDURE dbo.usp_User_CreateFromCustomer
(
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @PasswordHash NVARCHAR(500),
    @Role NVARCHAR(50),
    @IsActive BIT
)
AS
BEGIN

    SET NOCOUNT ON;

    INSERT INTO Users
    (
        FirstName,
        LastName,
        Email,
        PasswordHash,
        Role,
        IsActive,
        CreatedDate
    )
    VALUES
    (
        @FirstName,
        @LastName,
        @Email,
        @PasswordHash,
        @Role,
        @IsActive,
        GETUTCDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS UserId;

END