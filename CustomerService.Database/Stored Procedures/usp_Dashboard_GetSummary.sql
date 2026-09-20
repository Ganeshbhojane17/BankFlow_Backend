CREATE PROCEDURE [dbo].[usp_Dashboard_GetSummary]
AS
BEGIN
   SET NOCOUNT ON;

   SELECT 
      COUNT(*) As TotalCustomers,
      
      SUM(
          CASE 
              WHEN IsActive = 1 THEN 1
              ELSE 0
          END
      ) AS ActiveCustomers,

      SUM(
            CASE
                WHEN IsActive = 0 THEN 1
                ELSE 0
            END
        ) AS InactiveCustomers

   FROm dbo.Customers;
END;
Go

