
IF EXISTS
(
    SELECT 1
    FROM sys.procedures
    WHERE Name = 'NavigatorReportGetUploadedReportsInfo'
)
    BEGIN
        DROP PROCEDURE [dbo].[NavigatorReportGetUploadedReportsInfo];
END;
GO
CREATE PROCEDURE [dbo].[NavigatorReportGetUploadedReportsInfo]
(@userid INT, 
 @IdList NVARCHAR(MAX) = ''
)
AS
    BEGIN
        SET NOCOUNT ON;
        -- mask
        DECLARE @intType INT;
        DECLARE @bitType BIT;
        DECLARE @stringtype NVARCHAR(MAX);
        DECLARE @datetimetype DATETIME;
        -- mask
        IF 1 = 0
            BEGIN
                SELECT @intType NavigatorReportID, 
                       @stringtype AS FileName, 
                       @stringtype AS ReportType, 
                       @bitType AS Parsed, 
                       @stringtype AS Error;
                RETURN;
        END;
            ELSE
            BEGIN
                SELECT @intType NavigatorReportID, 
                       @stringtype AS FileName, 
                       @stringtype AS ReportType, 
                       @bitType AS Parsed, 
                       @stringtype AS Error
                INTO #ExportList;
                DELETE #ExportList;
        END;
        DECLARE @CurrRoleId INT;
        SELECT @CurrRoleId = RoleID
        FROM dbo.[User]
        WHERE userid = @userid;
        SELECT CAST(Items AS INT) AS Id
        INTO #IdList
        FROM dbo.SplitString(@IdList, ';') splt
        WHERE ISNUMERIC(Items) = 1;

        -- end check right
        INSERT INTO #ExportList
        (NavigatorReportID, 
         FileName, 
         ReportType, 
         Parsed, 
         Error
        )
               SELECT NavigatorReportID, 
                      re.S3FileFullName, 
                      ReportType, 
                      CAST(CASE
                               WHEN re.STATUS = 'successed'
                               THEN 1
                               ELSE 0
                           END AS BIT) AS Parsed, 
               (
                   SELECT Message + '; '
                   FROM dbo.NavigatorReportLog p2
                   WHERE p2.NavigatorReportID = re.NavigatorReportID
                         AND LEN(Message) > 0 FOR XML PATH('')
               ) AS Error
               FROM dbo.NavigatorReport re
                    INNER JOIN #IdList i ON re.NavigatorReportID = i.Id
               WHERE @CurrRoleId = 5 -- publisher
                     AND re.STATUS IS NOT NULL
                     AND re.STATUS <> 'Deleted';
        SELECT *
        FROM #ExportList;
    END;