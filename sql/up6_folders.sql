USE [up6]
GO

/****** Object:  Table [dbo].[up6_folders]    Script Date: 05/19/2019 14:22:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[up6_folders](
	[f_id] [char](32) NOT NULL,
	[f_nameLoc] [varchar](50) NULL,
	[f_pid] [char](32) NULL,
	[f_uid] [int] NULL,
	[f_lenLoc] [bigint] NULL,
	[f_sizeLoc] [varchar](50) NULL,
	[f_pathLoc] [varchar](255) NULL,
	[f_pathSvr] [varchar](255) NULL,
	[f_folders] [int] NULL,
	[f_fileCount] [int] NULL,
	[f_filesComplete] [int] NULL,
	[f_complete] [bit] NULL,
	[f_delete] [bit] NULL,
	[f_time] [datetime] NULL,
	[f_pidRoot] [char](32) NULL,
	[f_pathRel] [nvarchar](255) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件夹名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_nameLoc'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父级ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_pid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_uid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数字化的大小。以字节为单位。示例：1023652' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_lenLoc'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'格式化的大小。示例：10G' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_sizeLoc'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件夹在客户端的路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_pathLoc'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件夹在服务端的路径' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_pathSvr'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件夹数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_folders'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_fileCount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'已上传完的文件数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_filesComplete'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已上传完毕' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_complete'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_delete'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上传时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'up6_folders', @level2type=N'COLUMN',@level2name=N'f_time'
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_name]  DEFAULT ('') FOR [f_nameLoc]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_pid]  DEFAULT ((0)) FOR [f_pid]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_uid]  DEFAULT ((0)) FOR [f_uid]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_length]  DEFAULT ((0)) FOR [f_lenLoc]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_size]  DEFAULT ('') FOR [f_sizeLoc]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_pathLoc]  DEFAULT ('') FOR [f_pathLoc]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_pathSvr]  DEFAULT ('') FOR [f_pathSvr]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_folders]  DEFAULT ((0)) FOR [f_folders]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_files]  DEFAULT ((0)) FOR [f_fileCount]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_filesComplete]  DEFAULT ((0)) FOR [f_filesComplete]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_complete]  DEFAULT ((0)) FOR [f_complete]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_delete]  DEFAULT ((0)) FOR [f_delete]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_timeUpload]  DEFAULT (getdate()) FOR [f_time]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_pidRoot]  DEFAULT ((0)) FOR [f_pidRoot]
GO

ALTER TABLE [dbo].[up6_folders] ADD  CONSTRAINT [DF_up6_folders_fd_pathRel]  DEFAULT ('') FOR [f_pathRel]
GO