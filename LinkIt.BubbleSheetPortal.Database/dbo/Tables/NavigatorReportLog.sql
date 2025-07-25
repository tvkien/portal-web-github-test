
CREATE TABLE [dbo].[NavigatorReportLog](
	[NavigatorReportLogId] [int] IDENTITY(1,1) NOT NULL,
	[NavigatorReportID] [int] NULL,
	[NavigatorReportDetailID] [int] NULL,
	[LogTime] [datetime] NULL,
	[Message] [varchar](1000) NULL,
 CONSTRAINT [pk_NavigatorReportLog] PRIMARY KEY CLUSTERED 
(
	[NavigatorReportLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


