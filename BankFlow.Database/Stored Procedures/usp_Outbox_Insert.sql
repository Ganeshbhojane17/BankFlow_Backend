CREATE PROCEDURE dbo.usp_Outbox_Insert
(
    @EventId UNIQUEIDENTIFIER,
    @EventType NVARCHAR(200),
    @RoutingKey NVARCHAR(200),
    @Payload NVARCHAR(MAX)
)
AS
BEGIN

    SET NOCOUNT ON;

    INSERT INTO OutboxMessages
    (
        EventId,
        EventType,
        RoutingKey,
        Payload
    )
    VALUES
    (
        @EventId,
        @EventType,
        @RoutingKey,
        @Payload
    );

END