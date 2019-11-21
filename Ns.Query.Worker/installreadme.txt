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

CREATE TABLE [dbo].[NsActionVisit](
	[CreatedOn] [datetime2](7) NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[PristavDebtId] [int] NULL,
	[TaskTypeId] [int] NULL,
	[VisitTargetId] [nvarchar](50) NULL,

	[SenderId] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[Descr] [nvarchar](500) NULL,
	[VisitAddressId] [nvarchar](50) NULL,

	[RepeatQuantity] [int] NULL,
	[StartDate] [datetime] NULL,
	[ActivePeriodMax] [int] NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](250) NULL,
) ON [PRIMARY]

CREATE TABLE [dbo].[NsFilterFault](
	[CreatedOn] [datetime2](7) NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[BPName] [nvarchar](500) NULL,
	[Reason] [nvarchar](500) NULL,
) ON [PRIMARY]

CREATE TABLE [dbo].[NsActionLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[ActionId] [int] NULL,
	[QueryId] [uniqueidentifier] NULL,
	[ClientId] [int] NULL,
	[DebtId] [int] NULL,
	[PristavDebtId] [int] NULL,
	[DialingTypeId] [nvarchar](50) NULL,
	[CallTypeId] [nvarchar](50) NULL,
	[SenderId] [nvarchar](50) NULL,
	[WorkerId] [int] NULL,
	[CallTargetId] [nvarchar](50) NULL,
	[VisitTargetId] [nvarchar](50) NULL,
	[VisitAddressId] [nvarchar](50) NULL,
	[Line] [nvarchar](50) NULL,
	[DayPart] [nvarchar](50) NULL,
	[CommentAction] [nvarchar](50) NULL,
	[CommTypeId] [nvarchar](50) NULL,
	[CommStateId] [nvarchar](50) NULL,
	[RepeatQuantity] [int] NULL,
	[RepeatInterval] [int] NULL,
	[QuantityPerMinute] [int] NULL,
	[SendSmsSpeed] [int] NULL,
	[StartDate] [date] NULL,
	[StartTime] [time](7) NULL,
	[EndDate] [datetime] NULL,
	[ActivePeriodMax] [int] NULL,
	[TemplateId] [int] NULL,
	[TemplateIVMId] [nvarchar](50) NULL,
	[TemplateRobotId] [nvarchar](50) NULL,
	[SchemeScriptId] [int] NULL,
	[UseTransferCall] [bit] NULL,
	[NeedVerify] [bit] NULL,
	[IsCheckPDS] [bit] NULL,
	[IsCheckHLR] [bit] NULL,
	[UseTimeZone] [bit] NULL,
	[IsSuccess90day] [bit] NULL,
	[IsOnNextDay] [bit] NULL,
	[IsAccordingLaw] [bit] NULL,
	[IsComplienceFZ] [bit] NULL,
	[IsSendSmsSuccess90days] [bit] NULL,
	[IsSendSmsPostponeNextDay] [bit] NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](250) NULL,
	[Descr] [nvarchar](500) NULL,
	[BPName] [nvarchar](500) NULL
) ON [PRIMARY]

IF COL_LENGTH('NsActionTable', 'BPName') IS NULL
BEGIN
    ALTER TABLE NsActionTable
    ADD BPName [nvarchar](500)
END

IF COL_LENGTH('NsActionIVM', 'BPName') IS NULL
BEGIN
    ALTER TABLE NsActionIVM
    ADD BPName [nvarchar](500)
END

IF COL_LENGTH('NsActionRing', 'BPName') IS NULL
BEGIN
    ALTER TABLE NsActionRing
    ADD BPName [nvarchar](500)
END

IF COL_LENGTH('NsActionRobot', 'BPName') IS NULL
BEGIN
    ALTER TABLE NsActionRobot
    ADD BPName [nvarchar](500)
END

IF COL_LENGTH('NsActionVisit', 'BPName') IS NULL
BEGIN
    ALTER TABLE NsActionVisit
    ADD BPName [nvarchar](500)
END


триггеры для nrs_am

CREATE TRIGGER [dbo].[NsMirrorActionIVMToLog]
ON [dbo].[NsActionIVM]
AFTER INSERT
AS
INSERT INTO dbo.NsActionLog(
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
ActionId,
DialingTypeId,
CallTargetId,
SenderId,
Line,
DayPart,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
StartTime,
ActivePeriodMax,
TemplateIVMId,
UseTransferCall,
UserId,
UserName,
Descr,
BPName) 
SELECT 
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
TaskTypeId,
DialingTypeId,
CallTargetId,
SenderId,
Line,
DayPart,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
StartTime,
ActivePeriodMax,
TemplateIVMId,
UseTransferCall,
UserId,
UserName,
Descr,
BPName
FROM inserted



CREATE TRIGGER [dbo].[NsMirrorActionRingToLog]
ON [dbo].[NsActionRing]
AFTER INSERT
AS
INSERT INTO dbo.NsActionLog(
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
ActionId,
DialingTypeId,
CallTargetId,
SenderId,
Line,
DayPart,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
StartTime,
ActivePeriodMax,
UserId,
UserName,
Descr,
BPName) 
SELECT 
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
TaskTypeId
,
DialingTypeId,
CallTargetId,
SenderId,
Line,
DayPart,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
StartTime,
ActivePeriodMax,
UserId,
UserName,
Descr,
BPName
FROM inserted



CREATE TRIGGER [dbo].[NsMirrorActionRobotToLog]
ON [dbo].[NsActionRobot]
AFTER INSERT
AS
INSERT INTO dbo.NsActionLog(
CreatedOn,
QueryId,
ClientId,
DebtId,
ActionId,
PristavDebtId,
TemplateRobotId,
CallTargetId,
SenderId,
Line,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
ActivePeriodMax,
UserId,
UserName,
Descr,
BPName) 
SELECT 
CreatedOn,
QueryId,
ClientId,
DebtId,
TaskTypeId,
PristavDebtId,
TemplateRobotId,
CallTargetId,
SenderId,
Line,
CommentAction,
CommTypeId,
CommStateId,
RepeatQuantity,
StartDate,
ActivePeriodMax,
UserId,
UserName,
Descr,
BPName
FROM inserted



CREATE TRIGGER [dbo].[NsMirrorActionTableToLog]
ON [dbo].[NsActionTable]
AFTER INSERT
AS
INSERT INTO dbo.NsActionLog(
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
ActionId,
SenderId,
CommTypeId,
CommStateId,
NeedVerify,
SchemeScriptId,
ActivePeriodMax,
RepeatInterval,
RepeatQuantity,
WorkerId,
StartDate,
EndDate,
QuantityPerMinute,
IsCheckPDS,
UseTimeZone,
IsSuccess90day,
IsOnNextDay,
IsAccordingLaw,
UserId,
UserName,
IsCheckHLR,
BPName
) 
SELECT 
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
TaskTypeId,
SenderId,
CommTypeId,
CommStateId,
NeedVerify,
SchemeScriptId,
ActivePeriodMax,
RepeatInterval,
RepeatQuantity,
WorkerId,
StartDate,
EndDate,
QuantityPerMinute,
IsCheckPDS,
UseTimeZone,
IsSuccess90day,
IsOnNextDay,
IsAccordingLaw,
UserId,
UserName,
IsCheckHLR,
BPName
FROM inserted


CREATE TRIGGER [dbo].[NsMirrorActionVisitToLog]
ON [dbo].[NsActionVisit]
AFTER INSERT
AS
INSERT INTO dbo.NsActionLog(
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
ActionId,
VisitTargetId,
SenderId,
Line,
Descr,
VisitAddressId,
RepeatQuantity,
StartDate,
ActivePeriodMax,
UserId,
UserName,
BPName) 
SELECT 
CreatedOn,
QueryId,
ClientId,
DebtId,
PristavDebtId,
TaskTypeId,
VisitTargetId,
SenderId,
Line,
Descr,
VisitAddressId,
RepeatQuantity,
StartDate,
ActivePeriodMax,
UserId,
UserName,
BPName
FROM inserted


Представления для БД самого бпм. 

CREATE VIEW [dbo].[NsVwActionLog]
AS
SELECT     NEWID() AS Id, nrs_am.dbo.NsActionLog.CreatedOn, nrs_am.dbo.NsActionLog.QueryId, nrs_am.dbo.NsActionLog.ClientId, nrs_am.dbo.NsActionLog.DebtId, nrs_am.dbo.NsActionLog.ActionId, dbo.NsAction.Name AS ActionType, nrs_am.dbo.NsActionLog.BPName, NULL AS CreatedBy, NULL 
                  AS ModifiedOn, NULL AS ModifiedBy, 0 AS ProcessListeners, CASE WHEN BackType IS NULL THEN N'Завершён' ELSE BackType END AS Status
FROM        nrs_am.dbo.NsActionLog INNER JOIN
                  dbo.NsAction ON dbo.NsAction.NrsId = nrs_am.dbo.NsActionLog.ActionId LEFT OUTER JOIN
                      (SELECT DISTINCT 
                                         nrs_am.dbo.NsActionTable_Back_Actions.Action, nrs_am.dbo.NsActionTable_Back_Types.BackType, nrs_am.dbo.NsActionTable_Back.ClientId, nrs_am.dbo.NsActionTable_Back.DebtId, nrs_am.dbo.NsActionTable_Back.QueryId, 
                                         nrs_am.dbo.NsActionTable_Back.ActionID
                       FROM        nrs_am.dbo.NsActionTable_Back INNER JOIN
                                         nrs_am.dbo.NsActionTable_Back_Actions ON nrs_am.dbo.NsActionTable_Back.ActionID = nrs_am.dbo.NsActionTable_Back_Actions.ID INNER JOIN
                                         nrs_am.dbo.NsActionTable_Back_Types ON nrs_am.dbo.NsActionTable_Back.BackTypeID = nrs_am.dbo.NsActionTable_Back_Types.ID) AS temp1 ON temp1.QueryId = nrs_am.dbo.NsActionLog.QueryId AND temp1.ClientId = nrs_am.dbo.NsActionLog.ClientId AND 
                  temp1.DebtId = nrs_am.dbo.NsActionLog.DebtId AND temp1.ActionID = nrs_am.dbo.NsActionLog.ActionId


CREATE VIEW [dbo].[NsVwFilterFault]
AS
SELECT     NEWID() AS Id, CreatedOn, QueryId, ClientId, DebtId, BPName, Reason, NULL AS CreatedBy, NULL AS ModifiedOn, NULL AS ModifiedBy, 0 AS ProcessListeners
FROM        nrs_am.dbo.NsFilterFault



CREATE TABLE NsGroupsClients(
	ClientId int NULL,
	DebtId int NULL,
	PristavDebtId int NULL, 
	SenderId int NULL, 
	Descr nvarchar(100),
) ON [PRIMARY]



CREATE VIEW NsVwBlock
AS
SELECT s.Code as TempId, s.Name as Name, NEWID() as Id
FROM [NSV].[dbo].[NsLine] s

