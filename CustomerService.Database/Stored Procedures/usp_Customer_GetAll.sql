CREATE PROCEDURE [dbo].[usp_Customer_GetAll]
(
      @PageNumber INT,

      @PageSize INT,

      @Search NVARCHAR(100)=NULL,

      @SortBy NVARCHAR(50),

      @SortDirection NVARCHAR(4)
)
AS
BEGIN

SET NOCOUNT ON;

SELECT *

FROM Customers

WHERE IsDeleted=0

AND
(
    @Search IS NULL

    OR

    FirstName LIKE '%'+@Search+'%'

    OR

    LastName LIKE '%'+@Search+'%'

    OR

    Email LIKE '%'+@Search+'%'

    OR

    PhoneNumber LIKE '%'+@Search+'%'
)

ORDER BY

CASE
WHEN @SortBy='CreatedDate'
AND @SortDirection='DESC'

THEN CreatedDate

END DESC,

CASE
WHEN @SortBy='CreatedDate'
AND @SortDirection='ASC'

THEN CreatedDate

END ASC

OFFSET

(@PageNumber-1)*@PageSize ROWS

FETCH NEXT @PageSize ROWS ONLY;

SELECT COUNT(*)

FROM Customers

WHERE IsDeleted=0

AND
(
    @Search IS NULL

    OR

    FirstName LIKE '%'+@Search+'%'

    OR

    LastName LIKE '%'+@Search+'%'

    OR

    Email LIKE '%'+@Search+'%'

    OR

    PhoneNumber LIKE '%'+@Search+'%'
);

END
