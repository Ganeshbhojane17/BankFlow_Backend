CREATE PROCEDURE dbo.usp_Customer_UpdateUserId
(
    @CustomerId INT,
    @UserId INT
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE Customers
    SET
        UserId = @UserId
    WHERE Id = @CustomerId;

END