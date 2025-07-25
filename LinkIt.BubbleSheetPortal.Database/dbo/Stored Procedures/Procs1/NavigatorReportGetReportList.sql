
IF EXISTS
(
    SELECT 1
    FROM sys.procedures
    WHERE Name = 'NavigatorReportGetReportList'
)
    BEGIN
        DROP PROCEDURE [dbo].[NavigatorReportGetReportList];
END;
GO
CREATE PROCEDURE [dbo].[NavigatorReportGetReportList]
(@userid      INT, 
 @districtID  INT           = NULL, 
 @schoolID    INT           = NULL, 
 @year        NVARCHAR(10)  = NULL, 
 @category    NVARCHAR(100) = NULL, 
 @reportingPeriod NVARCHAR(100) = NULL
)
AS
    BEGIN
        SET NOCOUNT ON;

        -- mask
        DECLARE @intType INT;
        DECLARE @bitType BIT;
        DECLARE @stringtype NVARCHAR(1000);
        DECLARE @datetimetype DATETIME;
        -- mask
        IF 1 = 0
            BEGIN
                SELECT @intType NavigatorReportID, 
                       @stringtype AS [Year], 
                       @datetimetype AS ReceivedDate, 
                       @datetimetype AS CreatedTime, 
                       @datetimetype AS PublishedDate, 
                       @stringtype AS NavigatorCategory, 
                       @stringtype AS ReportingPeriod, 
                       @stringtype AS School, 
                       @intType AS SchoolID, 
                       @intType AS DistrictID, 
                       @stringtype AS FileName;
                RETURN;
        END;
            ELSE
            BEGIN
                SELECT @intType NavigatorReportID, 
                       @stringtype AS [Year], 
                       @datetimetype AS ReceivedDate, 
                       @datetimetype AS CreatedTime, 
                       @datetimetype AS PublishedDate, 
                       @stringtype AS NavigatorCategory, 
                       @stringtype AS ReportingPeriod, 
                       @stringtype AS School, 
                       @intType AS SchoolID, 
                       @intType AS DistrictID, 
                       @stringtype AS FileName
                INTO #ExportList;
                DELETE #ExportList;
        END;
        SELECT NavigatorReportID, 
               DistrictID, 
               SchoolID
        INTO #ReportList
        FROM dbo.NavigatorReport re
        WHERE STATUS IS NOT NULL
              AND STATUS <> 'Deleted'
              AND (NULLIF(@districtID, 0) IS NULL
                   OR re.DistrictID = @districtID)
              AND (NULLIF(@SchoolID, 0) IS NULL
                   OR re.SchoolID = @SchoolID)
              AND (NULLIF(@year, '') IS NULL
                   OR re.[Year] = @year)
              AND (NULLIF(@category, '') IS NULL
                   OR re.NavigatorCategory = @category)
              AND (NULLIF(@reportingPeriod, '') IS NULL
                   OR re.ReportingPeriod = @reportingPeriod);

        -- begin check right
        DECLARE @CurrRoleId INT;
        SELECT @CurrRoleId = RoleID
        FROM dbo.[User]
        WHERE userid = @userid;
        IF @@ROWCOUNT = 0
           OR @CurrRoleId IS NULL
            BEGIN
                DELETE #ReportList;
        END;
            ELSE
            IF @CurrRoleId = 3
               OR @CurrRoleId = 27 -- district admin -- 27: network admin
                BEGIN
                    -- delete other district
                    DELETE #ReportList
                    WHERE DistrictID IS NULL
                          OR DistrictID NOT IN
                    (
                        SELECT DistrictID
                        FROM dbo.[NetworkAdminView] v
                        WHERE v.UserID = @userid
                    );
            END;
                ELSE
                IF @CurrRoleId = 8 -- school admin
                    BEGIN
                        DECLARE @SchooId INT;
                        SELECT @schoolID = SchoolID
                        FROM dbo.[User]
                        WHERE userid = @userid;
                        --remove invalid schoolId
                        DELETE #ReportList
                        WHERE SchoolID IS NULL
                              OR SchoolID <> @schoolID;

                        --remove if don't have right
                        DELETE re
                        FROM #ReportList re
                        WHERE NOT EXISTS
                        (
                            SELECT 1
                            FROM NavigatorReportPublish p
                            WHERE re.NavigatorReportID = p.NavigatorReportID
                                  AND p.UserID = @userid
                                  AND p.IsPublished = 1
                        );
                END;
                    ELSE
                    IF @CurrRoleId <> 5 --teacher -- delete other class ~> <>5 then except Publisher
                        BEGIN
                            PRINT 211;
                            -- published
                            DELETE re
                            FROM #ReportList re
                            WHERE NOT EXISTS
                            (
                                SELECT 1
                                FROM NavigatorReportPublish p
                                WHERE re.NavigatorReportID = p.NavigatorReportID
                                      AND p.UserID = @userid
                                      AND p.IsPublished = 1
                            );
                            -- teacher of this class
                            --begin remove not related
                            DELETE re
                            FROM #ReportList re
                            WHERE re.NavigatorReportID NOT IN
                            (
                                SELECT re.NavigatorReportID -- ~>valid reportid
                                FROM #ReportList re
                                     INNER JOIN NavigatorReportDetail de ON re.NavigatorReportID = de.NavigatorReportID -- get classId
                                     INNER JOIN ClassUser cu ON de.ClassID = cu.ClassID
                                                                AND cu.UserID = @userid
                                     INNER JOIN UserSchool uc ON uc.SchoolID = re.SchoolID
                            );
                            --end remove not related
                    END;
        -- end check right

        INSERT INTO #ExportList
        (NavigatorReportID, 
         Year, 
         ReceivedDate, 
         CreatedTime, 
         PublishedDate, 
         NavigatorCategory, 
         ReportingPeriod, 
         School, 
         SchoolID, 
         DistrictID, 
         FileName
        )
               SELECT re.NavigatorReportID, 
                      na.[Year], 
                      na.CreatedTime, 
                      na.CreatedTime, 
                      na.PublishedDate, 
                      na.NavigatorCategory, 
                      na.ReportingPeriod, 
                      sc.[Name] AS School, 
                      na.SchoolID, 
                      na.DistrictID, 
                      na.S3FileFullName
               FROM #ReportList re
                    INNER JOIN NavigatorReport na ON re.NavigatorReportID = na.NavigatorReportID
                    LEFT JOIN School sc ON sc.SchoolID = na.SchoolID;
        SELECT *
        FROM #ExportList;
    END;