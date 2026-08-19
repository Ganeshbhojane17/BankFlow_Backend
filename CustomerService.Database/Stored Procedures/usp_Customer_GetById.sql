CREATE PROCEDURE [dbo].[usp_Customer_GetById]
(
    @Id INT
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT *

    FROM Customers

    WHERE Id = @Id
      AND IsDeleted = 0;

END
