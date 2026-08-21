CREATE PROCEDURE dbo.usp_Customer_GetByUserId
(
    @UserId INT
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        Id,
        UserId,
        CustomerNumber,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Gender,
        PANNumber,
        AadhaarNumber,
        Occupation,
        AnnualIncome,
        Address,
        City,
        State,
        Country,
        PostalCode,
        IsActive,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy
    FROM Customers
    WHERE UserId = @UserId;

END