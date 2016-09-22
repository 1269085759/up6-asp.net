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
	,@f_ids varchar(8000) output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @i int;
	set @i = 0;
	set @f_ids = '';
	
	while @i < @f_count
	begin
		insert into down_files(f_uid) values(@uid);
		set @f_ids = @f_ids + str(@@IDENTITY) + ',';
		set @i = @i + 1;
	end
	
	--清除,
	set @f_ids = substring(@f_ids,1,len(@f_ids)-1);
END
