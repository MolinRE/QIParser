USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Комаров К.
-- Create date: 2017-10-28
-- Description:	Добавить сообщение в переписку.
-- =============================================
CREATE PROCEDURE [dbo].[ICQ_HISTORY_ADD]
	@uin int,
	@is_my bit,
	@date datetime,
	@text nvarchar(MAX)
AS
	BEGIN
		DECLARE @result TABLE([MESSAGE_ID] INT);
		INSERT INTO [dbo].[ICQ_HISTORY] ([IS_MY], [CONTACT_UIN], [DATE], [TEXT])
		OUTPUT INSERTED.MESSAGE_ID INTO @result
		VALUES (@is_my, @uin, @date, @text)

		SELECT [MESSAGE_ID]
		FROM @result
	END;
GO

