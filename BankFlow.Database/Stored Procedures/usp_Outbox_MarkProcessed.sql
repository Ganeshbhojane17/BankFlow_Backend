CREATE PROCEDURE dbo.usp_Outbox_MarkProcessed
(
    @Id BIGINT
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE OutboxMessages
    SET
        ProcessedOn = GETUTCDATE(),
        ErrorMessage = NULL
    WHERE Id = @Id;

END