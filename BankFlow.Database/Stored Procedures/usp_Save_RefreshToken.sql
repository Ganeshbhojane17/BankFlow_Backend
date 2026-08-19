CREATE PROCEDURE dbo.usp_Save_RefreshToken
(
    @UserId INT,
    @Token NVARCHAR(500),
    @ExpiryDate DATETIME2
)
AS
BEGIN

    SET NOCOUNT ON;

    INSERT INTO RefreshTokens
    (
        UserId,
        Token,
        ExpiryDate
    )
    VALUES
    (
        @UserId,
        @Token,
        @ExpiryDate
    );

END