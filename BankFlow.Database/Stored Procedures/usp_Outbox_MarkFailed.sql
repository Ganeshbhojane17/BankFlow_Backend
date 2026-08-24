CREATE PROCEDURE dbo.usp_Outbox_MarkFailed
(
    @Id BIGINT,
    @ErrorMessage NVARCHAR(MAX)
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE OutboxMessages
    SET
        RetryCount = RetryCount + 1,
        ErrorMessage = @ErrorMessage
    WHERE Id = @Id;

END