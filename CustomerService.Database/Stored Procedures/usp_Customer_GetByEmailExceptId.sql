CREATE PROCEDURE [dbo].[usp_Customer_GetByEmailExceptId]
(
    @Email NVARCHAR(200),
    @Id INT
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT TOP 1 *

    FROM Customers

    WHERE Email = @Email
      AND Id <> @Id
      AND IsDeleted = 0;

END