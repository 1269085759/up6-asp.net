USE [HttpUploader6]
GO
/****** 对象:  StoredProcedure [dbo].[fd_add_batch]    脚本日期: 07/28/2016 17:42:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		zysoft
-- Create date: 2016-07-31
-- Description:	批量添加文件
-- =============================================
CREATE PROCEDURE [dbo].[fd_add_batch]
	-- Add the parameters for the stored procedure here
	 @f_count int	--文件总数，要单独增加一个文件夹
	,@uid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--使用临时表存ID
	create table #tb_ids(t_id int)

    DECLARE @i int;
	set @i = 0;
	
	while @i < @f_count
	begin
		insert into down_files(f_uid) values(@uid);
		insert into #tb_ids values(@@IDENTITY)
		set @i = @i + 1;
	end
	
	--返回ids
	select * from #tb_ids
END