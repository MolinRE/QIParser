USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ICQ_HISTORY_ADD]
	@from int,
	@to int,
	@date DATETIME,
	@text NVARCHAR(MAX)
AS
	DECLARE @result TABLE([ID] INT);
	INSERT INTO [dbo].[ICQ_HISTORY] ([FROM], [TO], [DATE], [MESSAGE])
	OUTPUT INSERTED.ID INTO @result
	VALUES (@from, @to, @date, @text)

	SELECT [ID]
	FROM @result
