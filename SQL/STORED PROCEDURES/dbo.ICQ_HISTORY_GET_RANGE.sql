USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Комаров К.
-- Create date: 2017-10-28
-- Description:	Получить сообщения за указанный промежуток.
-- =============================================
CREATE PROCEDURE [dbo].[ICQ_HISTORY_GET_RANGE]
	@uin int,
	@from datetime,
	@to datetime
AS
	BEGIN
		SELECT [MESSAGE_ID]
		, [IS_MY]
		, [CONTACT_UIN]
		, [DATE]
		, [TEXT]
		FROM [dbo].[ICQ_HISTORY]
		WHERE 
			[CONTACT_UIN] = @uin AND ([DATE] >= @from AND [DATE] <= @to)
	END;
GO

