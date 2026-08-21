CREATE PROCEDURE [dbo].[usp_Customer_GetAll]
(
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Search NVARCHAR(200) = NULL,
    @IsActive BIT = NULL,
    @SortBy NVARCHAR(50) = 'CreatedDate',
    @SortDirection NVARCHAR(4) = 'DESC'
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @Offset INT =
        (@PageNumber - 1) * @PageSize;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        City,
        State,
        IsActive,
        CreatedDate
    FROM Customers
    WHERE
        (
            @Search IS NULL
            OR FirstName LIKE '%' + @Search + '%'
            OR LastName LIKE '%' + @Search + '%'
            OR Email LIKE '%' + @Search + '%'
        )
        AND
        (
            @IsActive IS NULL
            OR IsActive = @IsActive
        )
    ORDER BY
        CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM Customers
    WHERE
        (
            @Search IS NULL
            OR FirstName LIKE '%' + @Search + '%'
            OR LastName LIKE '%' + @Search + '%'
            OR Email LIKE '%' + @Search + '%'
        )
        AND
        (
            @IsActive IS NULL
            OR IsActive = @IsActive
        );

END