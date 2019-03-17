SC CREATE "NsQueryWorker1" binpath="C:\Users\almaz\source\repos\Ns.Query.Worker\Ns.Query.Worker\bin\Debug\Ns.Query.Worker.exe"


Хранимая процедура для выполнения запросов:

CREATE PROCEDURE [dbo].[tsp_NsPerformQueryWithoutResult] (
	 @sqlQuery nvarchar(max), @queryId nvarchar(50), @affected int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ParamDefs nvarchar(500)
	DECLARE @rowc int
	SET @sqlQuery = @sqlQuery + ';SELECT @rowc = @@rowcount'
	
	SET @ParamDefs = N' @rowc int OUTPUT'

	EXECUTE sp_executesql @sqlQuery, @ParamDefs,@rowc OUTPUT;
	SET @affected = @rowc

END
GO


