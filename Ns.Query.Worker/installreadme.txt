SC CREATE "NsQueryWorker1" binpath="C:\Users\almaz\source\repos\Ns.Query.Worker\Ns.Query.Worker\bin\Debug\Ns.Query.Worker.exe"


Хранимая процедура для выполнения запросов:

CREATE PROCEDURE [dbo].[tsp_NsPerformQueryWithoutResult] (
	 @sqlQuery nvarchar(max), @queryId nvarchar(50), @affected int OUTPUT, @err nvarchar(4000) OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @ParamDefs nvarchar(500)
		DECLARE @rowc int
		SET @sqlQuery = @sqlQuery + ';SELECT @rowc = @@rowcount'
	
		SET @ParamDefs = N' @rowc int OUTPUT'

		EXECUTE sp_executesql @sqlQuery, @ParamDefs,@rowc OUTPUT;
		SET @affected = @rowc
		SET @err = 'Success'
	END TRY
	BEGIN CATCH
		SET @affected = -1
		SET @err = 'Error: ' + CONVERT(nvarchar(10), ERROR_NUMBER()) + ' ' + ERROR_MESSAGE()
	END CATCH
END
GO

Добавление нужных полей если их нет
IF COL_LENGTH('NsResultTable', 'QueryId') IS NULL
BEGIN
    ALTER TABLE [NsResultTable]
    ADD [QueryId] UNIQUEIDENTIFIER
END

IF COL_LENGTH('NsResultTable', 'DebtId') IS NULL
BEGIN
    ALTER TABLE [NsResultTable]
    ADD DebtId INT
END

IF COL_LENGTH('NsResultTable', 'ClientId') IS NULL
BEGIN
    ALTER TABLE [NsResultTable]
    ADD ClientId INT
END

IF COL_LENGTH('NsActionTable', 'QuantityPerMinute') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD QuantityPerMinute INT
END

IF COL_LENGTH('NsActionTable', 'IsCheckPDS') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD IsCheckPDS BIT
END

IF COL_LENGTH('NsActionTable', 'UseTimeZone') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD UseTimeZone BIT
END

IF COL_LENGTH('NsActionTable', 'IsSuccess90day') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD IsSuccess90day BIT
END

IF COL_LENGTH('NsActionTable', 'IsOnNextDay') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD IsOnNextDay BIT
END

IF COL_LENGTH('NsActionTable', 'IsAccordingLaw') IS NULL
BEGIN
    ALTER TABLE [NsActionTable]
    ADD IsAccordingLaw BIT
END

CREATE TABLE [dbo].[NsActionEvent]( [Id] [uniqueidentifier] NOT NULL, [CreatedOn] [datetime2](7) NOT NULL, [EventCode] [int] NOT NULL, [ClientId] [int] NULL, [DebtId] [int] NULL, [PristavDebtId] [int] NULL, [IsHandled] [bit] NOT NULL) ON [PRIMARY]

CREATE TABLE [dbo].[NsActionRing](
	[CreatedOn] [datetime2](7) NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[PristavDebtId] [int] NULL,
	[TaskTypeId] [int] NULL,
	[DialingTypeId] [nvarchar](50) NULL,
	[CallTargetId] [nvarchar](50) NULL,
	[SenderId] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[DayPart] [nvarchar](50) NULL,
	[Comment] [nvarchar](250) NULL,
	[CommentAction] [nvarchar](50) NULL,
	[CommTypeId] [nvarchar](50) NULL,
	[CommStateId] [nvarchar](50) NULL,
	[RepeatQuantity] [int] NULL,
	[StartDate] [date] NULL,
	[StartTime] [time](7) NULL,
	[ActivePeriodMax] [int] NULL
) ON [PRIMARY]



IF COL_LENGTH('NsActionTable', 'UseTimeZone') IS NULL
BEGIN
    ALTER TABLE NsActionTable
    ADD UseTimeZone bit
END


ALTER TABLE "NsActionTable" DROP COLUMN "UserId";
IF COL_LENGTH('NsActionTable', 'UserId') IS NULL
BEGIN
    ALTER TABLE NsActionTable
    ADD UserId [uniqueidentifier]
END


IF COL_LENGTH('NsActionTable', 'UserName') IS NULL
BEGIN
    ALTER TABLE NsActionTable
    ADD UserName [nvarchar](50)
END

ALTER TABLE "NsActionRing" DROP COLUMN "UserId";
IF COL_LENGTH('NsActionRing', 'UserId') IS NULL
BEGIN
    ALTER TABLE NsActionRing
    ADD UserId [uniqueidentifier]
END


IF COL_LENGTH('NsActionRing', 'UserName') IS NULL
BEGIN
    ALTER TABLE NsActionRing
    ADD UserName [nvarchar](50)
END

CREATE TABLE [dbo].[NsActionIVM](
	[CreatedOn] [datetime2](7) NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[PristavDebtId] [int] NULL,
	[TaskTypeId] [int] NULL,
	[DialingTypeId] [nvarchar](50) NULL,
	[CallTargetId] [nvarchar](50) NULL,
	[SenderId] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[DayPart] [nvarchar](50) NULL,
	[Comment] [nvarchar](250) NULL,
	[CommentAction] [nvarchar](50) NULL,
	[CommTypeId] [nvarchar](50) NULL,
	[CommStateId] [nvarchar](50) NULL,
	[RepeatQuantity] [int] NULL,
	[StartDate] [date] NULL,
	[StartTime] [time](7) NULL,
	[ActivePeriodMax] [int] NULL,
	[TemplateIVMId] [nvarchar](50) NULL,
	[UseTransferCall] [bit] NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](250) NULL,
) ON [PRIMARY]


IF COL_LENGTH('NsActionTable', 'IsCheckHLR') IS NULL
BEGIN
    ALTER TABLE NsActionTable
    ADD IsCheckHLR bit
END


IF COL_LENGTH('NsActionRing', 'Comment') IS NOT NULL
BEGIN
	ALTER TABLE NsActionRing DROP COLUMN "Comment";
END
IF COL_LENGTH('NsActionRing', 'Descr') IS NULL
BEGIN
    ALTER TABLE NsActionRing ADD Descr [nvarchar](500)
END


IF COL_LENGTH('NsActionIVM', 'Comment') IS NOT NULL
BEGIN
	ALTER TABLE NsActionIVM DROP COLUMN "Comment";
END
IF COL_LENGTH('NsActionIVM', 'Descr') IS NULL
BEGIN
    ALTER TABLE NsActionIVM ADD Descr [nvarchar](500)
END


CREATE TABLE [dbo].[NsActionRobot](
	[CreatedOn] [datetime2](7) NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[PristavDebtId] [int] NULL,
	[TaskTypeId] [int] NULL,
	[TemplateRobotId] [nvarchar](50) NULL,
	[CallTargetId] [nvarchar](50) NULL,
	[SenderId] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[Descr] [nvarchar](500) NULL,
	[CommentAction] [nvarchar](50) NULL,
	[CommTypeId] [nvarchar](50) NULL,
	[CommStateId] [nvarchar](50) NULL,
	[RepeatQuantity] [int] NULL,
	[StartDate] [datetime] NULL,
	[ActivePeriodMax] [int] NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](250) NULL,
) ON [PRIMARY]
