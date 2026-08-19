CREATE PROCEDURE [dbo].[usp_Customer_Delete]
(
    @Id INT,

    @ModifiedBy NVARCHAR(100)
)
AS
BEGIN

SET NOCOUNT ON;

UPDATE Customers

SET

    IsDeleted = 1,

    IsActive = 0,

    ModifiedDate = GETUTCDATE(),

    ModifiedBy = @ModifiedBy

WHERE Id=@Id

AND IsDeleted=0;

END
