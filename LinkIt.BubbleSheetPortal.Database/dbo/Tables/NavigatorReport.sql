
CREATE TABLE [dbo].[NavigatorReport](
	[NavigatorReportID] [int] IDENTITY(1,1) NOT NULL,
	[S3FileFullName] [varchar](256) NULL,
	[ReportType] [varchar](50) NULL,
	[DistrictID] [int] NOT NULL,
	[SchoolID] [int] NOT NULL,
	[Year] [varchar](10) NOT NULL,
	[BenchmarkCategory] [varchar](50) NOT NULL,
	[UserDefined] [varchar](50) NOT NULL,
	[CreatedTime] [datetime] NULL,
	[PublishedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[Status] [varchar](10) NULL,
 CONSTRAINT [pk_NavigatorReport] PRIMARY KEY CLUSTERED 
(
	[NavigatorReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


