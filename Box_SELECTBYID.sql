USE [C32]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[Box_SelectById] @Id int

AS


/* TEST CODE

DECLARE @Id int = 2

EXECUTE dbo.Box_SelectById @Id



*/
BEGIN
SELECT [Id]
      ,[ProfileId]
      ,[ShippingCost]
      ,[Tax]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate]
  FROM [dbo].[Box]
  WHERE Id = @Id
  END



