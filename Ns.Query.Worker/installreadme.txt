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