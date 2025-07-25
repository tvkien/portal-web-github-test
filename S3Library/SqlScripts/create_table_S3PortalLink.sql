/****** Object:  Table [dbo].[S3Link]    Script Date: 01/24/2013 14:48:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[S3PortalLink](
	[S3PortalLinkID] [int] IDENTITY(1,1) NOT NULL,
	[ServiceType] [int] NULL,
	[DistrictID] [int] NULL,
	[BucketName] [varchar](255) NULL,
	[FilePath] [varchar](500) NULL,
	[DateCreated] [datetime] NULL,
	[PortalKey] [varchar](22) NULL,
 CONSTRAINT [PK_S3Link] PRIMARY KEY CLUSTERED 
(
	[S3PortalLinkID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


