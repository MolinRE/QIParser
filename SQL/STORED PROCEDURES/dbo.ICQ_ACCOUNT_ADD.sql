USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Комаров К.
-- Create date: 2017-10-28
-- Description:	Добавить аккаунт пользователя.
-- =============================================
CREATE PROCEDURE [dbo].[ICQ_ACCOUNT_ADD]
	@uin int,
	@nick nvarchar(100)
AS
	BEGIN
		INSERT INTO [dbo].[ICQ_ACCOUNT] ([UIN], [GLOBAL_ID], [NICKNAME])
		VALUES (@uin, NULL, @nick)
	END;
GO

