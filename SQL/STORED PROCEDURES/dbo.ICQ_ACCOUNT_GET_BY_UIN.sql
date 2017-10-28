USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Комаров К.
-- Create date: 2017-10-28
-- Description:	Получить аккаунт пользователя.
-- =============================================
CREATE PROCEDURE [dbo].[ICQ_ACCOUNT_GET_BY_UIN]
	@uin int
AS
	BEGIN
		SELECT [UIN]
		, [NICKNAME]
		FROM [dbo].[ICQ_ACCOUNT]
		WHERE [UIN] = @uin
	END;
GO

