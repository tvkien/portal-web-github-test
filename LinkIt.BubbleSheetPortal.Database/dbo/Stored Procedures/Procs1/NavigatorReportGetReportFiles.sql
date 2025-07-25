

IF EXISTS
(
    SELECT 1
    FROM sys.procedures
    WHERE Name = 'NavigatorReportGetReportFiles'
)
    BEGIN
        DROP PROCEDURE [dbo].[NavigatorReportGetReportFiles];
END;
GO
CREATE PROCEDURE [dbo].[NavigatorReportGetReportFiles]
(@userid             INT, 
 @navigatorReportIds VARCHAR(MAX)
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
                SELECT @stringtype AS S3FileFullName, 
                       @intType AS NavigatorReportID, 
                       @intType AS FromPage, 
                       @stringtype AS MasterFileName;
                RETURN;
        END;
            ELSE
            BEGIN
                SELECT @stringtype AS S3FileFullName, 
                       @intType AS NavigatorReportID, 
                       @intType AS FromPage, 
                       @stringtype AS MasterFileName
                INTO #ExportList;
                DELETE #ExportList;
        END;
        SELECT CAST(Items AS INT) AS NavigatorReportId
        INTO #ReportIds
        FROM dbo.SplitString(@navigatorReportIds, ';') s
        WHERE ISNUMERIC(Items) = 1;

        -- begin check right
        DECLARE @CurrRoleId INT;
        SELECT @CurrRoleId = RoleID
        FROM dbo.[User]
        WHERE userid = @userid;
        IF @@ROWCOUNT = 0
           OR @CurrRoleId IS NULL
            BEGIN
                GOTO Selectdata;
        END;
            ELSE
            IF @CurrRoleId = 5
                BEGIN
                    --publisher then print all
                    INSERT INTO #ExportList
                    (NavigatorReportID, 
                     S3FileFullName
                    )
                           SELECT na.NavigatorReportID, 
                                  na.S3FileFullName
                           FROM NavigatorReport na
                           WHERE na.NavigatorReportID IN
                           (
                               SELECT NavigatorReportId
                               FROM #ReportIds
                           ) -- in select list
                                 AND ISNULL(na.STATUS, 'Deleted') <> 'Deleted'; -- status is not deleted
            END;
                ELSE
                IF @CurrRoleId = 3
                   OR @CurrRoleId = 27 -- district admin -- 27: network admin
                    BEGIN
                        INSERT INTO #ExportList
                        (NavigatorReportID, 
                         S3FileFullName
                        )
                               SELECT na.NavigatorReportID, 
                                      na.S3FileFullName
                               FROM NavigatorReport na
                               WHERE na.NavigatorReportID IN
                               (
                                   SELECT NavigatorReportId
                                   FROM #ReportIds
                               ) -- in select list
                                     AND ISNULL(na.STATUS, 'Deleted') <> 'Deleted' -- status is not deleted
                                     AND DistrictID IN
                               (
                                   SELECT DistrictID
                                   FROM dbo.[NetworkAdminView] v
                                   WHERE v.UserID = @userid
                               ); -- and in current district
                END;
                    ELSE
                    IF @CurrRoleId = 8 -- school admin
                        BEGIN
                            DECLARE @schoolID INT;
                            SELECT @schoolID = SchoolID
                            FROM dbo.[User]
                            WHERE userid = @userid;
                            --remove invalid schoolId

                            INSERT INTO #ExportList
                            (NavigatorReportID, 
                             S3FileFullName
                            )
                                   SELECT na.NavigatorReportID, 
                                          na.S3FileFullName
                                   FROM NavigatorReport na
                                   WHERE na.NavigatorReportID IN
                                   (
                                       SELECT NavigatorReportId
                                       FROM #ReportIds
                                   ) -- in select list
                                         AND ISNULL(na.STATUS, 'Deleted') <> 'Deleted' -- status is not deleted
                                         AND SchoolID = @schoolID -- in this school
                                         AND EXISTS
                                   (
                                       SELECT 1
                                       FROM NavigatorReportPublish p
                                       WHERE p.NavigatorReportID = na.NavigatorReportID
                                             AND p.UserID = @userid
                                   ); -- and has right
                    END;
                        ELSE
                        IF @CurrRoleId <> 5 --teacher -- delete other class ~> <>5 then except Publisher
                            BEGIN
                                PRINT 211;
                                -- published
                                -- teacher then page by page
                                --      string _pageFileName = $"{Path.GetFileNameWithoutExtension(fileNameByConvention)}_{page.Metadata.FromPageIndex}_{page.Metadata.ToPageIndex}.PDF";

                                INSERT INTO #ExportList
                                (NavigatorReportID, 
                                 S3FileFullName, 
                                 FromPage, 
                                 MasterFileName
                                )
                                       SELECT na.NavigatorReportID, 
                                              REPLACE(na.S3FileFullName, '.pdf', '') + '_' + CAST(de.FromPage AS NVARCHAR(10)) + '_' + CAST(de.ToPage AS NVARCHAR(10)) + '.PDF', 
                                              de.FromPage, 
                                              na.S3FileFullName AS MasterFileName
                                       FROM NavigatorReport na
                                            INNER JOIN NavigatorReportDetail de ON na.NavigatorReportID = de.NavigatorReportID -- get classId
                                            INNER JOIN ClassUser cu ON de.ClassID = cu.ClassID
                                                                       AND cu.UserID = @userid
                                       WHERE na.NavigatorReportID IN
                                       (
                                           SELECT NavigatorReportId
                                           FROM #ReportIds
                                       ) -- in select list
                                             AND ISNULL(na.STATUS, 'Deleted') <> 'Deleted' -- status is not deleted
                                             AND EXISTS
                                       (
                                           SELECT 1
                                           FROM NavigatorReportPublish p
                                           WHERE p.NavigatorReportID = na.NavigatorReportID
                                                 AND p.UserID = @userid
                                       ); -- and has right

                        END;
        -- end check right
        UPDATE #ExportList
          SET 
              MasterFileName = S3FileFullName
        WHERE MasterFileName IS NULL;
        Selectdata:
        SELECT *
        FROM #ExportList;
    END;