CREATE PROCEDURE [dbo].[usp_Dashboard_GetRecentCustomers]
    @Count INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Count)
        Id,
        CustomerNumber,
        CONCAT(FirstName, ' ', LastName) AS FullName,
        Email,
        IsActive

    FROM dbo.Customers

    ORDER BY Id DESC;
END;
GO