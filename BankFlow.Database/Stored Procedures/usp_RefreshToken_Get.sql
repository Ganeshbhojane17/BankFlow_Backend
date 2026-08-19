CREATE PROCEDURE [dbo].[usp_RefreshToken_Get]
(
    @Token NVARCHAR(500)
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT TOP 1 *

    FROM RefreshTokens

    WHERE Token = @Token
      AND IsRevoked = 0
      AND ExpiryDate > GETDATE();

END