USE [C32]
GO
/****** Object:  StoredProcedure [dbo].[Box_Insert]    Script Date: 4/12/2017 9:04:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Box_Insert]
				 @ProfileId int
				,@ShippingCost decimal(18, 4)
				,@Tax decimal(18, 4)
				,@CreatedBy nvarchar(128)
				,@CreatedDate datetime2(7)	= NULL
				,@ModifiedDate datetime2(7)	= NULL
				,@Id int OUTPUT

AS

/*
DECLARE
		 @ProfileId int = '9'
		 ,@ShippingCost decimal = '7'
		 ,@Tax decimal = '9.1'
		 ,@CreatedBy nvarchar(128) = 'agg'
		 ,@CreatedDate	datetime2(7) = '04/11/2017'
		 ,@ModifiedDate datetime2(7) = '04/11/2017'
		 ,@Id int

  EXEC Box_Insert
		@ProfileId 
		,@ShippingCost
		,@Tax 
		,@CreatedBy 
		,@CreatedDate
		,@ModifiedDate 
		,@Id
SELECT * 
FROM dbo.Box
WHERE Id = @Id	

*/
BEGIN
	DECLARE @ModifiedBy nvarchar(128) = @CreatedBy
		IF (@CreatedDate IS null)
	BEGIN
		SET @CreatedDate = GETUTCDATE()
	END

	SET @ModifiedDate = COALESCE(@ModifiedDate, GETUTCDATE())
    -- take 1st not null value
	INSERT INTO dbo.Box
				(ProfileId
				,ShippingCost
				,Tax
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate
				)
	VALUES		(@ProfileId
				,@ShippingCost
				,@Tax
				,@CreatedBy
				,@CreatedDate
				,@ModifiedBy
				,@ModifiedDate
				)

			SELECT @Id = SCOPE_IDENTITY() 
END

