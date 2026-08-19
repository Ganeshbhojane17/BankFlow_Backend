CREATE PROCEDURE dbo.usp_Customer_ChangeStatus
(
      @Id INT,
      @IsActive BIT,
      @ModifiedBy NVARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE Customers

    SET

        IsActive = @IsActive,

        ModifiedDate = GETUTCDATE(),

        ModifiedBy = @ModifiedBy

    WHERE Id = @Id

    AND IsDeleted = 0;

END