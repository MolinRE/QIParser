USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Комаров К.
-- Create date: 2017-10-28
-- Description:	Получить последнее сообщение в переписке.
-- =============================================
CREATE PROCEDURE [dbo].[ICQ_HISTORY_GET_LATEST]
	@uin int
AS
	BEGIN
		SELECT TOP 1 [MESSAGE_ID]
		, [IS_MY]
		, [CONTACT_UIN]
		, [DATE]
		, [TEXT]
		FROM [dbo].[ICQ_HISTORY]
		WHERE [CONTACT_UIN] = @uin
		ORDER BY [DATE] DESC
	END;
GO

