CREATE PROCEDURE dbo.usp_Customer_GetAll
(
    @PageNumber INT,
    @PageSize INT,
    @Search NVARCHAR(200) = NULL,
    @IsActive BIT = NULL
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @Offset INT =
        (@PageNumber - 1) * @PageSize;


    SELECT
        Id,
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

        -- Files
        ProfileImagePath,
        DocumentPath,
        DocumentName,
        DocumentContentType,

        IsActive,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy

    FROM Customers

    WHERE
        (
            @Search IS NULL
            OR FirstName LIKE '%' + @Search + '%'
            OR LastName LIKE '%' + @Search + '%'
            OR Email LIKE '%' + @Search + '%'
            OR CustomerNumber LIKE '%' + @Search + '%'
        )
        AND
        (
            @IsActive IS NULL
            OR IsActive = @IsActive
        )

    ORDER BY Id DESC

    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;


    -- Total records

    SELECT COUNT(1)

    FROM Customers

    WHERE
        (
            @Search IS NULL
            OR FirstName LIKE '%' + @Search + '%'
            OR LastName LIKE '%' + @Search + '%'
            OR Email LIKE '%' + @Search + '%'
            OR CustomerNumber LIKE '%' + @Search + '%'
        )
        AND
        (
            @IsActive IS NULL
            OR IsActive = @IsActive
        );

END