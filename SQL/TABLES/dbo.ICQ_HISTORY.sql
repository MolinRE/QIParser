USE [PersonalLogStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ICQ_HISTORY] (
    [ID]      INT            IDENTITY (1000, 1) NOT NULL,
    [FROM]    INT            NOT NULL,
    [TO]      INT            NOT NULL,
    [DATE]    DATETIME       NOT NULL,
    [MESSAGE] NVARCHAR (MAX) NOT NULL
);