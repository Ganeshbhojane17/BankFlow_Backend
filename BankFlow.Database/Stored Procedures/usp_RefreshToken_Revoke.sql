CREATE PROCEDURE [dbo].[usp_RefreshToken_Revoke]
(
    @Token NVARCHAR(500)
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE RefreshTokens

    SET
        IsRevoked = 1,
        RevokedDate = GETDATE()

    WHERE Token = @Token;

END
