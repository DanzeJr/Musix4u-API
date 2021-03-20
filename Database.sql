USE [Musix4u]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FavoriteTrack]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FavoriteTrack](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TrackId] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_FavoriteTrack] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Playlist]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Playlist](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[OwnerId] [bigint] NOT NULL,
	[IsPublic] [bit] NOT NULL,
 CONSTRAINT [PK_Playlist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlaylistTrack]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaylistTrack](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PlaylistId] [bigint] NOT NULL,
	[TrackId] [bigint] NOT NULL,
 CONSTRAINT [PK_PlaylistTrack] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Track]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Track](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](max) NULL,
	[CoverUrl] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Performers] [nvarchar](max) NULL,
	[Album] [nvarchar](max) NULL,
	[Year] [bigint] NULL,
	[Duration] [bigint] NOT NULL,
	[UploaderId] [bigint] NULL,
	[IsPublic] [bit] NOT NULL,
 CONSTRAINT [PK_Track] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/17/2021 2:10:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[FavoriteTrack]  WITH CHECK ADD  CONSTRAINT [FK_FavoriteTrack_Track_TrackId] FOREIGN KEY([TrackId])
REFERENCES [dbo].[Track] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FavoriteTrack] CHECK CONSTRAINT [FK_FavoriteTrack_Track_TrackId]
GO
ALTER TABLE [dbo].[FavoriteTrack]  WITH CHECK ADD  CONSTRAINT [FK_FavoriteTrack_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FavoriteTrack] CHECK CONSTRAINT [FK_FavoriteTrack_User_UserId]
GO
ALTER TABLE [dbo].[Playlist]  WITH CHECK ADD  CONSTRAINT [FK_Playlist_User_OwnerId] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Playlist] CHECK CONSTRAINT [FK_Playlist_User_OwnerId]
GO
ALTER TABLE [dbo].[PlaylistTrack]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistTrack_Playlist_PlaylistId] FOREIGN KEY([PlaylistId])
REFERENCES [dbo].[Playlist] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlaylistTrack] CHECK CONSTRAINT [FK_PlaylistTrack_Playlist_PlaylistId]
GO
ALTER TABLE [dbo].[PlaylistTrack]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistTrack_Track_TrackId] FOREIGN KEY([TrackId])
REFERENCES [dbo].[Track] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlaylistTrack] CHECK CONSTRAINT [FK_PlaylistTrack_Track_TrackId]
GO
ALTER TABLE [dbo].[Track]  WITH CHECK ADD  CONSTRAINT [FK_Track_User_UploaderId] FOREIGN KEY([UploaderId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Track] CHECK CONSTRAINT [FK_Track_User_UploaderId]
GO
