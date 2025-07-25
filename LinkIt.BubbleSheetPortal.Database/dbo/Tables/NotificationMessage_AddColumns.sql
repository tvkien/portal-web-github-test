
IF NOT EXISTS
(
    SELECT *
    FROM sys.columns
    WHERE Name = N'ReceivingUserID'
          AND Object_ID = OBJECT_ID(N'NotificationMessage')
)
    BEGIN
        ALTER TABLE NotificationMessage
        ADD ReceivingUserID INT NULL;
END;
GO
IF NOT EXISTS
(
    SELECT *
    FROM sys.columns
    WHERE Name = N'NotificationType'
          AND Object_ID = OBJECT_ID(N'NotificationMessage')
)
    BEGIN
        ALTER TABLE NotificationMessage
        ADD NotificationType VARCHAR(50) NULL;
END;