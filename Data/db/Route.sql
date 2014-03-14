USE [Toprope]
GO

/****** Object:  Table [dbo].[Route]    Script Date: 3/3/2013 5:59:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Route](
	[Id] [uniqueidentifier] NOT NULL,
	[SectorId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](512) NULL,
	[Description] [nvarchar](max) NULL,
	[Grade] [float] NULL,
	[Order] [int] NULL,
	[Climbing] [int] NULL,
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Route]  WITH CHECK ADD  CONSTRAINT [FK_Route_Sector] FOREIGN KEY([SectorId])
REFERENCES [dbo].[Sector] ([Id])
GO

ALTER TABLE [dbo].[Route] CHECK CONSTRAINT [FK_Route_Sector]
GO

