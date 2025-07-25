
IF EXISTS
(
    SELECT 1
    FROM sys.procedures
    WHERE Name = 'NavigatorReportGetAssociateUserByReportId'
)
    BEGIN
        DROP PROCEDURE [dbo].[NavigatorReportGetAssociateUserByReportId];
END;
GO
CREATE PROCEDURE [dbo].[NavigatorReportGetAssociateUserByReportId]
(@NavigatorReportId INT, 
 @userId            INT, 
 @RoleId            INT, 
 @IsPublished       BIT
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
                SELECT @intType UserID, 
                       @bitType IsPublished, 
                       @datetimetype PublishTime, 
                       @stringtype UserFullName, 
                       @stringtype UserName, 
                       @stringtype RoleName, 
                       @stringtype SchoolName;
                RETURN;
        END;
        CREATE TABLE #Result
        (UserID       INT, 
         IsPublished  BIT, 
         PublishTime  DATETIME, 
         UserFullName NVARCHAR(1000), 
         UserName     NVARCHAR(1000), 
         RoleName     NVARCHAR(1000), 
         SchoolName   NVARCHAR(1000)
        );
        DECLARE @DistrictID INT;
        DECLARE @SchoolID INT;
        SELECT @DistrictID = DistrictId, 
               @SchoolID = SchoolID
        FROM NavigatorReport
        WHERE NavigatorReportID = @NavigatorReportID
              AND STATUS <> 'Deleted';
        SELECT DISTINCT 
               u.UserID, 
               u.UserName, 
               u.NameLast, 
               u.NameFirst, 
               u.UserStatusID, 
               r.RoleID, 
               r.Display AS RoleName, 
               us.SchoolID, 
        (
            SELECT ISNULL(NULLIF(LTRIM(RTRIM(s.NAME)), '') + '|', '')
            FROM UserSchool us, 
                 School s
            WHERE us.SchoolID = s.SchoolID
                  AND us.UserID = u.UserId
            ORDER BY s.NAME FOR XML PATH('')
        ) AS SchoolList, 
        (
            SELECT CAST(us.SchoolID AS VARCHAR(10)) + '|'
            FROM UserSchool us
            WHERE us.UserID = u.UserId FOR XML PATH('')
        ) AS SchoolIDList
        INTO #AllUserList
        FROM dbo.[User] AS u
             LEFT OUTER JOIN dbo.District AS d ON u.DistrictID = d.DistrictID
             LEFT OUTER JOIN dbo.STATE AS st ON d.StateID = st.StateID
             INNER JOIN dbo.ROLE AS r ON u.RoleID = r.RoleID
             LEFT JOIN dbo.UserSchool AS us ON u.UserID = us.UserID
        WHERE u.UserStatusID IN(1, 3) --TODO: Only Show Active & Inactive User  
             AND u.RoleID IN(2, 8, 27)
        AND u.DistrictID = @DistrictID
        AND
        (
            SELECT CAST(us.SchoolID AS VARCHAR(10)) + '|'
            FROM UserSchool us
            WHERE us.UserID = u.UserId
                  AND us.SchoolID = @SchoolID FOR XML PATH('')
        ) IS NOT NULL;
        IF @RoleId = 8 -- school admin  
            BEGIN
                DELETE #AllUserList
                WHERE RoleName NOT LIKE '%teacher%';
        END;

        -- remove not related teachers  
        SELECT DISTINCT 
               cu.UserID
        INTO #teachers
        FROM NavigatorReport na
             INNER JOIN NavigatorReportDetail de ON na.NavigatorReportID = de.NavigatorReportID
             INNER JOIN ClassUser cu ON cu.ClassID = de.ClassId -- ralted teachers  
             INNER JOIN [User] us ON cu.UserID = us.UserID
                                     AND us.RoleID = 2
                                     AND us.UserStatusID IN(1, 3) -- teacher  
        WHERE na.NavigatorReportID = @NavigatorReportId;
        DELETE #AllUserList
        WHERE RoleName LIKE '%teacher%'
              AND UserID NOT IN
        (
            SELECT t.UserId
            FROM #teachers t
        );
        DROP TABLE #teachers;  
        -- end remove not related teachers   

        INSERT INTO #Result
        (UserID, 
         IsPublished, 
         PublishTime, 
         UserFullName, 
         UserName, 
         RoleName, 
         SchoolName
        )
               SELECT ul.UserID, 
                      ISNULL(pb.IsPublished, 0) AS IsPublished, 
                      pb.PublishTime, 
                      concat(ISNULL(ul.NameLast, ''),
                                                   CASE
                                                       WHEN NULLIF(ul.NameFirst, '') IS NOT NULL
                                                       THEN ',' + ul.NameFirst
                                                       ELSE ''
                                                   END) AS UserFullName, 
                      ul.UserName, 
                      ul.RoleName, 
                      s.Name AS SchoolName
               FROM #AllUserList ul
                    LEFT JOIN dbo.NavigatorReportPublish pb ON ul.UserID = pb.UserID
                                                               AND pb.NavigatorReportID = @NavigatorReportId
                    LEFT JOIN dbo.[User] puser ON pb.PublisherId = puser.UserID
                    LEFT JOIN dbo.School s ON ul.SchoolID = s.SchoolID
               WHERE ul.SchoolID =
               (
                   SELECT SchoolID
                   FROM NavigatorReport
                   WHERE NavigatorReportID = @NavigatorReportId
               );
        UPDATE #Result
          SET 
              PublishTime = NULL
        WHERE ISNULL(IsPublished, 0) = 0;
        IF @IsPublished = 0
            BEGIN
                SELECT *
                FROM #Result r
                WHERE ISNULL(r.IsPublished, 0) = @IsPublished;
        END;
            ELSE
            BEGIN
                SELECT *
                FROM #Result;
        END;
    END;