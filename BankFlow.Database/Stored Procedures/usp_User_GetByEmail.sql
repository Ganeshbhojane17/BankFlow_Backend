CREATE PROCEDURE dbo.usp_User_GetByEmail

    @Email NVARCHAR(200)

AS
BEGIN

SET NOCOUNT ON;

SELECT
    Id,
    FirstName,
    LastName,
    Email,
    PasswordHash,
    Role,
    IsActive,
    CreatedDate,
    ModifiedDate
FROM Users
WHERE Email=@Email;

END