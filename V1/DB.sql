USE [ClassScheduleDB]
GO

/****** Object:  Table [dbo].[Children]    Script Date: 2018/7/15 17:17:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Children](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Birthday] [varchar](50) NULL,
	[Sex] [nvarchar](50) NULL,
	[Background] [varchar](50) NULL,
	[HeadPortrait] [varchar](500) NULL,
	[OpenID] [varchar](50) NULL,
 CONSTRAINT [PK_Children] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/*----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

USE [ClassScheduleDB]
GO

/****** Object:  Table [dbo].[Course]    Script Date: 2018/7/15 17:17:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Course](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CourseName] [nvarchar](50) NULL,
	[StartTime] [varchar](50) NULL,
	[EndTime] [varchar](50) NULL,
	[Address] [nvarchar](500) NULL,
	[Teacher] [nvarchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[RemindTime] [varchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ChildrenID] [int] NULL,
	[SchoolName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


