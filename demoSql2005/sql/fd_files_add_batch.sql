USE [HttpUploader6]
GO
/****** 对象:  StoredProcedure [dbo].[fd_add_batch]    脚本日期: 07/28/2016 17:42:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		zysoft
-- Create date: 2016-08-04
-- 更新 2016-09-06 使用临时表解决ID数量过多的问题。
-- Description:	批量分配文件夹ID和文件ID，提供给上传文件夹使用，在初始化时使用
-- =============================================
CREATE PROCEDURE [dbo].[fd_files_add_batch]	
	 @f_count int	--文件总数，要单独增加一个文件夹
	,@fd_count int	--文件夹总数
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--使用临时表存储ID
	create table #tb_ids(t_file bit,t_id int)

    DECLARE @i int;
	set @i = 0;

	/*批量添加文件夹*/
	while @i < @fd_count
	begin
		insert into up6_folders(fd_pid) values(0);
		insert into #tb_ids values(0,@@IDENTITY)
		set @i = @i + 1;
	end

	/*批量添加文件*/
	set @i = 0;
	while @i < @f_count
	begin
		insert into up6_files(f_pid) values(0)
		insert into #tb_ids values(1,@@IDENTITY)
		set @i = @i+1
	end
	
	--清除,
	select * from #tb_ids;
END
