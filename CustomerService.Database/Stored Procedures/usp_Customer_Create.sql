CREATE PROCEDURE [dbo].[usp_Customer_Create]
(
    @CustomerNumber NVARCHAR(20),
    @UserId INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @PhoneNumber NVARCHAR(20) = NULL,
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
    @CreatedBy NVARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    INSERT INTO Customers
    (
        CustomerNumber,
        UserId,
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
        CreatedBy
    )

    VALUES
    (
        @CustomerNumber,
        @UserId,
        @FirstName,
        @LastName,
        @Email,
        @PhoneNumber,
        @DateOfBirth,
        @Gender,
        @PANNumber,
        @AadhaarNumber,
        @Occupation,
        @AnnualIncome,
        @Address,
        @City,
        @State,
        @Country,
        @PostalCode,
        @CreatedBy
    );

    SELECT SCOPE_IDENTITY();

END