
CREATE TABLE [dbo].[NavigatorReportDetail](
	[NavigatorReportDetailID] [int] IDENTITY(1,1) NOT NULL,
	[NavigatorReportID] [int] NOT NULL,
	[TeacherID] [int] NULL,
	[ClassID] [int] NULL,
	[StudentID] [int] NULL,
	[FromPage] [int] NULL,
	[ToPage] [int] NULL,
	[Status] [varchar](10) NULL,
 CONSTRAINT [pk_NavigatorReportDetail] PRIMARY KEY CLUSTERED 
(
	[NavigatorReportDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NavigatorReportDetail]  WITH CHECK ADD  CONSTRAINT [fk_NavigatorReport] FOREIGN KEY([NavigatorReportID])
REFERENCES [dbo].[NavigatorReport] ([NavigatorReportID])
GO

ALTER TABLE [dbo].[NavigatorReportDetail] CHECK CONSTRAINT [fk_NavigatorReport]
GO


