CREATE PROCEDURE dbo.usp_User_Register

    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @PasswordHash NVARCHAR(MAX),
    @Role NVARCHAR(50)

AS
BEGIN

SET NOCOUNT ON;

INSERT INTO Users
(
FirstName,
LastName,
Email,
PasswordHash,
Role
)

VALUES
(
@FirstName,
@LastName,
@Email,
@PasswordHash,
@Role
);

END