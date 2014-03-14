USE [Toprope]
GO

/****** Object:  Table [dbo].[Area]    Script Date: 3/3/2013 5:59:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Area](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](512) NULL,
	[Description] [nvarchar](max) NULL,
	[Tags] [nvarchar](max) NULL,
	[Season] [int] NULL,
	[Climbing] [int] NULL,
	[Location] [geography] NULL,
	[Origin] [nvarchar](512) NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

