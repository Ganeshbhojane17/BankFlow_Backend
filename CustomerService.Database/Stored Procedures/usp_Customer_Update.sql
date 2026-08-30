CREATE PROCEDURE [dbo].[usp_Customer_Update]
(
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @PhoneNumber NVARCHAR(20),
    @DateOfBirth DATE = NULL,
    @Gender NVARCHAR(20) = NULL,
    @PANNumber NVARCHAR(20) = NULL,
    @AadhaarNumber NVARCHAR(20) = NULL,
    @Occupation NVARCHAR(100) = NULL,
    @AnnualIncome DECIMAL(18,2) = NULL,
    @Address NVARCHAR(300) = NULL,
    @City NVARCHAR(100) = NULL,
    @State NVARCHAR(100) = NULL,
    @Country NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL,
    @ProfileImagePath NVARCHAR(500) = NULL,

    @DocumentPath NVARCHAR(500) = NULL,
    @DocumentName NVARCHAR(255) = NULL,
    @DocumentContentType NVARCHAR(100) = NULL,
    @ModifiedBy NVARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE Customers
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        PANNumber = @PANNumber,
        AadhaarNumber = @AadhaarNumber,
        Occupation = @Occupation,
        AnnualIncome = @AnnualIncome,
        Address = @Address,
        City = @City,
        State = @State,
        Country = @Country,
        PostalCode = @PostalCode,
        ModifiedDate = GETUTCDATE(),
        ModifiedBy = @ModifiedBy,
        ProfileImagePath = @ProfileImagePath,
        DocumentPath = @DocumentPath,
        DocumentName = @DocumentName,
        DocumentContentType = @DocumentContentType
    WHERE Id = @Id
      AND IsDeleted = 0;
END
