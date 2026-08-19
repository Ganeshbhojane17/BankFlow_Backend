CREATE PROCEDURE [dbo].[usp_Customer_GetByEmail]
(
    @Email NVARCHAR(200)
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT TOP 1 *

    FROM Customers

    WHERE Email=@Email

    AND IsDeleted=0;

END