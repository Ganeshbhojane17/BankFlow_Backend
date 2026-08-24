CREATE PROCEDURE dbo.usp_Outbox_GetPending
(
    @BatchSize INT
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT TOP (@BatchSize)
        Id,
        EventId,
        EventType,
        RoutingKey,
        Payload,
        CreatedOn,
        ProcessedOn,
        RetryCount,
        ErrorMessage
    FROM OutboxMessages
    WHERE ProcessedOn IS NULL
    ORDER BY Id;

END