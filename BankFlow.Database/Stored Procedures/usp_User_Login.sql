CREATE PROCEDURE dbo.usp_User_Login

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
IsActive

FROM Users

WHERE Email=@Email;

END