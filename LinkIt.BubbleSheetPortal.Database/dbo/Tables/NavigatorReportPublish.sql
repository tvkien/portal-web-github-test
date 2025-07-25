
CREATE TABLE [dbo].[NavigatorReportPublish](
	[NavigatorReportID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[IsPublished] [bit] NULL,
	[PublishFirstTime] [datetime] NULL,
	[PublishTime] [datetime] NULL,
	[PublisherId] [int] NOT NULL,
 CONSTRAINT [pk_NavigatorReportPublish] PRIMARY KEY CLUSTERED 
(
	[NavigatorReportID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NavigatorReportPublish]  WITH CHECK ADD  CONSTRAINT [fk_Publish_NavigatorReport] FOREIGN KEY([NavigatorReportID])
REFERENCES [dbo].[NavigatorReport] ([NavigatorReportID])
GO

ALTER TABLE [dbo].[NavigatorReportPublish] CHECK CONSTRAINT [fk_Publish_NavigatorReport]
GO


