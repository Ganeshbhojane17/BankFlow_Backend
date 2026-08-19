CREATE PROCEDURE [dbo].[usp_User_GetById]
(
    @UserId INT
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT *

    FROM Users

    WHERE Id = @UserId;

END
