-- =============================================
-- Author:		zysoft
-- 2016-08-04 创建
-- 2016-09-06 更新，单个md5长度和md5字符串总长度由外部传入
-- Description:	批量查询相同MD5的文件
-- =============================================
CREATE PROCEDURE [dbo].[fd_files_check]	
	 @md5s		varchar(8000)	--md5列表:a,b,c,d
	,@md5_len	int--单个MD5长度
	,@md5s_len	int --md5字符串总长度
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--拆分md5		
	declare @md5_item varchar(40)	
	declare @md5_cur int
	declare @split_pos int--当前分割符位置	
	DECLARE @t_md5 TABLE(md5 varchar(40))
	
	set @md5_cur = 0
	set @split_pos = charindex(',',@md5s)
	
	
	--有多个md5
	if @md5s_len > @md5_len
	begin
		while @md5_cur < @md5s_len
		begin
			set @md5_item = substring(@md5s,@md5_cur+1,@md5_len)			
			insert into @t_md5(md5) values(@md5_item)
			set @md5_cur = @md5_cur + @md5_len + 1
		end
	end
	else--只有一个md5
		insert into @t_md5(md5) values(@md5s)

	--查询数据库
	
	select *
	from (select * from up6_files where f_id in (select max(f_id) from up6_files group by f_md5)) as fs
	inner join @t_md5 t
	on t.md5 = fs.f_md5 
END