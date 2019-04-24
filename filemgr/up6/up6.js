/*
	版权所有 2009-2019 荆门泽优软件有限公司
	保留所有权利
	官方网站：http://www.ncmem.com/
	产品首页：http://www.ncmem.com/webapp/up6.2/index.asp
	产品介绍：http://www.cnblogs.com/xproer/archive/2012/05/29/2523757.html
	开发文档-ASP：http://www.cnblogs.com/xproer/archive/2012/02/17/2355458.html
	开发文档-PHP：http://www.cnblogs.com/xproer/archive/2012/02/17/2355467.html
	开发文档-JSP：http://www.cnblogs.com/xproer/archive/2012/02/17/2355462.html
	开发文档-ASP.NET：http://www.cnblogs.com/xproer/archive/2012/02/17/2355469.html
	升级日志：http://www.cnblogs.com/xproer/archive/2012/02/17/2355449.html
	证书补丁：http://www.ncmem.com/download/WoSignRootUpdate.rar
	VC运行库：http://www.microsoft.com/en-us/download/details.aspx?id=29
	联系信箱：1085617561@qq.com
	联系QQ：1085617561
    版本：2.3.6
	更新记录：
		2009-11-05 创建。
		2015-07-31 优化更新进度逻辑
        2019-03-18 完善文件夹粘帖功能，完善文件夹初始化逻辑。
*/
var HttpUploaderErrorCode = {
	  "0": "发送数据错误"
	, "1": "接收数据错误"
	, "2": "访问本地文件错误"
	, "3": "域名未授权"
	, "4": "文件大小超过限制"
	, "5": "文件大小为0"
	, "6": "文件被占用"
    , "7": "文件夹子元素数量超过限制"
    , "8": "文件夹大小超过限制"
    , "9": "子文件大小超过限制"
    , "10": "文件夹数量超过限制"
	, "100": "服务器错误"
};
var up6_err_solve = {
    errFolderCreate: "请检查UrlFdCreate地址配置是否正确\n请检查浏览器缓存是否已更新\n请检查数据库是否创建\n请检查数据库连接配置是否正确"
    , errFolderComplete: "请检查UrlFdComplete地址配置是否正确\n请检查浏览器缓存是否已更新\n请检查数据库是否创建\n请检查数据库连接配置是否正确"
    , errFileComplete: "请检查UrlComplete地址配置是否正确\n请检查浏览器缓存是否已更新"
};
var HttpUploaderState = {
	Ready: 0,
	Posting: 1,
	Stop: 2,
	Error: 3,
	GetNewID: 4,
	Complete: 5,
	WaitContinueUpload: 6,
	None: 7,
	Waiting: 8
	,MD5Working:9
    , scan: 10
};

function debugMsg(m) { $("#msg").append(m); }
function HttpUploaderMgr()
{
    var _this = this;
	this.Config = {
		  "EncodeType"		: "utf-8"
		, "Company"			: "荆门泽优软件有限公司"
		, "Version"			: "2,7,118,5241"
		, "License"			: ""//
		, "Authenticate"	: ""//域验证方式：basic,ntlm
		, "AuthName"		: ""//域帐号
		, "AuthPass"		: ""//域密码
        , "CryptoType"      : "md5"//验证方式：md5,sha1,crc
		, "FileFilter"		: "*"//文件类型。所有类型：*。自定义类型：jpg,bmp,png,gif,rar,zip,7z,doc
		, "FileSizeLimit"	: "0"//自定义允许上传的文件大小，以字节为单位。0表示不限制。字节计算工具：http://www.beesky.com/newsite/bit_byte.htm
		, "FilesLimit"		: "0"//文件选择数限制。0表示不限制
		, "AllowMultiSelect": true//多选开关。1:开启多选。0:关闭多选
		, "RangeSize"		: "2097152"//文件块大小，以字节为单位。必须为64KB的倍数。推荐大小：2MB。
        , "BlockMd5"		: false//开启文件块MD5验证
		, "Debug"			: false//是否打开调式模式。true,false
		, "LogFile"			: "F:\\log.txt"//日志文件路径。需要先打开调试模式。
		, "InitDir"			: ""//初始化路径。示例：D:\\Soft
		, "AppPath"			: ""//网站虚拟目录名称。子文件夹 web
        , "Cookie"			: ""//服务器cookie
        , "Md5Folder"       : false//上传文件夹时是否计算子文件md5
        , "QueueCount"      : 3//同时上传的任务数
        , "Md5Thread"       : 10//最大为10
        , "FolderThread"    : 3//最大为10
        , "FdSizeLimit"     : 0//文件夹大小限制。0表示不限制
        , "FdChildLimit"    : 0//文件夹子元素数量限制（子文件+子文件夹）。0表示不限制
        , "ProcSaveTm"      : 60//定时保存进度。单位：秒，默认：1分钟
		//文件夹操作相关
		, "UrlFdCreate"		: page.path.fe + "biz/up6_svr.aspx?op=fd-init"
		, "UrlFdComplete"	: page.path.fe + "biz/up6_svr.aspx?op=fd-comp"
		, "UrlFdDel"	    : page.path.fe + "biz/up6_svr.aspx?op="
		//文件操作相关
		, "UrlCreate"		: page.path.fe + "biz/up6_svr.aspx?op=init"
		, "UrlPost"			: page.path.fe + "biz/up6_svr.aspx?op=post"
        , "UrlProcess"		: page.path.fe + "biz/up6_svr.aspx?op=proc"
		, "UrlComplete"		: page.path.fe + "biz/up6_svr.aspx?op=cmp"
		, "UrlList"			: page.path.fe + "biz/up6_svr.aspx?op="
		, "UrlDel"			: page.path.fe + "biz/up6_svr.aspx?op="
	    //x86
        , ie: {
              drop: { clsid: "0868BADD-C17E-4819-81DE-1D60E5E734A6", name: "Xproer.HttpDroper6" }
            , part: { clsid: "BA0B719E-F4B7-464b-A664-6FC02126B652", name: "Xproer.HttpPartition6" }
            , path: "http://www.ncmem.com/download/up6.3/up6.cab"
        }
	    //x64
        , ie64: {
              drop: { clsid: "7B9F1B50-A7B9-4665-A6D1-0406E643A856", name: "Xproer.HttpDroper6x64" }
            , part: { clsid: "307DE0A1-5384-4CD0-8FA8-500F0FFEA388", name: "Xproer.HttpPartition6x64" }
            , path: "http://www.ncmem.com/download/up6.3/up64.cab"
        }
        , firefox: { name: "", type: "application/npHttpUploader6", path: "http://www.ncmem.com/download/up6.3/up6.xpi" }
        , chrome: { name: "npHttpUploader6", type: "application/npHttpUploader6", path: "http://www.ncmem.com/download/up6.3/up6.crx" }
        , edge: {protocol:"up6",port:9100,visible:false}
        , exe: { path: "http://www.ncmem.com/download/up6.3/up6.exe" }
		, "SetupPath": "http://localhost:4955/demoAccess/js/setup.htm"
        , "Fields": {"uname": "test","upass": "test","uid":"0"}
        , ui: {
            icon: {
                file: page.path.fe + "up6/file.png"
                , folder: page.path.fe + "up6/folder.png"
                , stop: page.path.fe + "up6/stop.png"
                , del: page.path.fe + "up6/del.png"
                , post: page.path.fe + "up6/post.png"
                , ok: page.path.fe + "res/imgs/16/ok.png"
            }
        }
    };
    //biz event
	this.event = {
	      "md5Complete": function (obj/*HttpUploader对象*/, md5) { }
        , "scanComplete": function (obj/*文件夹扫描完毕，参考：FolderUploader*/) { }
        , "fileAppend": function (obj/*文件添加完毕，参考：FileUploader*/) { }
        , "folderAppend": function (obj/*文件添加完毕，参考：FileUploader*/) { }
        , "fileComplete": function (obj/*文件上传完毕，参考：FileUploader*/) { }
        , "fdComplete": function (obj/*文件夹上传完毕，参考：FolderUploader*/) { }
        , "queueComplete": function () {/*队列上传完毕*/ }
        , "addFdError": function (json) {/*添加文件夹失败*/ }
        , "after_sel_file": function () {/**/ }
        , "loadComplete": function () {/*控件加载完毕*/ }
	};

	this.working = false;

    this.FileFilter = this.Config.FileFilter.split(","); //文件过滤器
	this.filesMap = new Object(); //本地文件列表映射表
	this.QueueFiles = new Array();//文件队列，数据:id1,id2,id3
	this.QueueWait = new Array(); //等待队列，数据:id1,id2,id3
	this.QueuePost = new Array(); //上传队列，数据:id1,id2,id3
	this.arrFilesComplete = new Array(); //已上传完的文件列表
    this.filesUI = null;//上传列表面板
    this.ieParter = null;
	this.parter = null;
    this.Droper = null;
    this.ui = {
        list:null
        , file: null
        , ele: {
            name: 'span[name="name"]'
            , ico_file: 'img[name="file"]'
            , ico_folder: 'img[name="folder"]'
            , ico_ok: 'img[name="ico-ok"]'
            , size: 'div[name="size"]'
            , path: 'div[name="path"]'
            , state: 'div[name="msg"]'
            , process: 'div[name="process"]'
            , msg: 'span[name="msg"]'
            , btn_post: 'i[name="post"]'
            , btn_stop: 'i[name="stop"]'
            , btn_del: 'i[name="del"]'
            , btn_cancel: 'i[name="cancel"]'
        }
    };
	this.tmpSpliter = null;
	this.uiSetupTip = null;
	this.btnSetup = null;
    //检查版本 Win32/Win64/Firefox/Chrome
	var browserName = navigator.userAgent.toLowerCase();
	this.ie = browserName.indexOf("msie") > 0;
    //IE11检查
	this.ie = this.ie ? this.ie : browserName.search(/(msie\s|trident.*rv:)([\w.]+)/) != -1;
	this.firefox = browserName.indexOf("firefox") > 0;
	this.chrome = browserName.indexOf("chrome") > 0;
	this.chrome45 = false;
	this.nat_load = false;
	this.edge_load = false;
	this.chrVer = navigator.appVersion.match(/Chrome\/(\d+)/);
	this.ffVer = navigator.userAgent.match(/Firefox\/(\d+)/);
	this.edge = navigator.userAgent.indexOf("Edge") > 0;
    this.edgeApp = new WebServerUp6(this);
    this.edgeApp.ent.on_close = function () { _this.socket_close(); };
    this.app = up6_app;
    this.app.edgeApp = this.edgeApp;
    this.app.Config = this.Config;
    this.app.ins = this;
	if (this.edge) { this.ie = this.firefox = this.chrome = this.chrome45 = false;}


	//容器的HTML代码
	this.GetHtmlContainer = function()
	{
	    //npapi
	    var com = '<embed name="ffParter" type="' + this.Config.firefox.type + '" pluginspage="' + this.Config.firefox.path + '" width="1" height="1"/>';
	    if (this.chrome45) com = "";
	    if (false)
	    {
	        //拖拽组件
	        com += '<object name="droper" classid="clsid:' + this.Config.ie.drop.clsid + '"';
	        com += ' codebase="' + this.Config.ie.path + '#version=' + this.Config.Version + '" width="192" height="192" >';
	        com += '</object>';
	    }
	    if (this.edge) com = '';
	    //文件夹选择控件
	    com += '<object name="parter" classid="clsid:' + this.Config.ie.part.clsid + '"';
	    com += ' codebase="' + this.Config.ie.path + '#version=' + this.Config.Version + '" width="1" height="1" ></object>';

	    return com;
    };    
	this.set_config = function (v) { jQuery.extend(this.Config, v);};
	this.open_files = function (json)
    {
        this.event.after_sel_file();
        for (var i = 0, l = json.files.length; i < l; ++i)
        {
            var f = this.addFileLoc(json.files[i]);
            this.event.fileAppend(f);
        }
	    setTimeout(function () { _this.PostFirst(); },500);
	};
	this.open_folders = function (json)
    {
        this.event.after_sel_file();
        for (var i = 0, l = json.folders.length; i < l; ++i) {
            var f = this.addFolderLoc(json.folders[i]);
            this.event.folderAppend(f);
        }
	    setTimeout(function () { _this.PostFirst(); }, 500);
	};
	this.paste_files = function (json)
	{
	    for (var i = 0, l = json.files.length; i < l; ++i)
	    {
	        this.addFileLoc(json.files[i]);
	    }
	};
	this.post_process = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.post_process(json);
	};
	this.post_error = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.post_error(json);
	};
	this.post_complete = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.post_complete(json);
    };
    this.post_stoped = function (json) {
        var p = this.filesMap[json.id];
        p.post_stoped(json);
    };
    this.scan_process = function (json) {
        var p = this.filesMap[json.id];
        p.scan_process(json);
    };
    this.scan_complete = function (json) {
        var p = this.filesMap[json.id];
        p.scan_complete(json);
    };
	this.md5_process = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.md5_process(json);
	};
	this.md5_complete = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.md5_complete(json);
	};
	this.md5_error = function (json)
	{
	    var p = this.filesMap[json.id];
	    p.md5_error(json);
    };
    this.load_complete = function (json)
    {
        //this.btnSetup.hide();
        var needUpdate = true;
        if (typeof (json.version) != "undefined") {
            if (json.version == this.Config.Version) {
                needUpdate = false;
            }
        }
        if (needUpdate) this.update_notice();
        else { }
        this.event.loadComplete();
    };
	this.load_complete_edge = function (json)
	{
	    this.edge_load = true;
        //this.btnSetup.hide();
        _this.app.init();
    };
    this.add_folder_error = function (json) {
        this.event.addFdError(json);
    };
    this.socket_close = function () {
        while (_this.QueuePost.length > 0)
        {
            _this.filesMap[_this.QueuePost[0]].post_stoped(null);
        }
		_this.QueuePost.length = 0;
    };
	this.recvMessage = function (str)
	{
	    var json = JSON.parse(str);
	         if (json.name == "open_files") { _this.open_files(json); }
	    else if (json.name == "open_folders") { _this.open_folders(json); }
	    else if (json.name == "paste_files") { _this.paste_files(json); }
	    else if (json.name == "post_process") { _this.post_process(json); }
	    else if (json.name == "post_error") { _this.post_error(json); }
	    else if (json.name == "post_complete") { _this.post_complete(json); }
	    else if (json.name == "post_stoped") { _this.post_stoped(json); }
	    else if (json.name == "scan_process") { _this.scan_process(json); }
	    else if (json.name == "scan_complete") { _this.scan_complete(json); }
	    else if (json.name == "md5_process") { _this.md5_process(json); }
	    else if (json.name == "md5_complete") { _this.md5_complete(json); }
	    else if (json.name == "md5_error") { _this.md5_error(json); }
	    else if (json.name == "add_folder_error") { _this.add_folder_error(json); }
	    else if (json.name == "load_complete") { _this.load_complete(json); }
	    else if (json.name == "load_complete_edge") { _this.load_complete_edge(json); }
	    else if (json.name == "extension_complete")
        {
            setTimeout(function () {
                var param = { name: "init", config: _this.Config };
                _this.app.postMessage(param);
                 }, 1000);
        }
	};

	this.checkBrowser = function ()
	{
	    //Win64
	    if (window.navigator.platform == "Win64")
	    {
	        jQuery.extend(this.Config.ie, this.Config.ie64);
	    }
	    if (this.firefox)
	    {
	        if (!this.app.checkFF() || parseInt(this.ffVer[1]) >= 50)//仍然支持npapi
            {
                this.edge = true;
                this.app.postMessage = this.app.postMessageEdge;
                this.edgeApp.run = this.edgeApp.runChr;
            }
	    }
	    else if (this.chrome)
	    {
            this.app.check = this.app.checkFF;
	        jQuery.extend(this.Config.firefox, this.Config.chrome);
	        //44+版本使用Native Message
	        if (parseInt(this.chrVer[1]) >= 44)
	        {
	            _this.firefox = true;
	            if (!this.app.checkFF())//仍然支持npapi
                {
                    this.edge = true;
                    this.app.postMessage = this.app.postMessageEdge;
                    this.edgeApp.run = this.edgeApp.runChr;
	            }
	        }
	    }
	    else if (this.edge)
	    {
            this.app.postMessage = this.app.postMessageEdge;
	    }
	};
    this.checkBrowser();
    //升级通知
    this.update_notice = function () {
        
    };
	//安装控件
	this.Install = function ()
	{
        if (!_this.app.Check())
		{
            _this.app.Setup();
		}
		else
		{
			$("body").empty();
			$("body").append("插件安装成功");
		}
	};

	//安全检查，在用户关闭网页时自动停止所有上传任务。
	this.SafeCheck = function(event)
	{
		$(window).bind("beforeunload", function(event)
		{
			if (_this.QueuePost.length > 0)
			{
				event.returnValue = "您还有程序正在运行，确定关闭？";
			}
		});

		$(window).bind("unload", function()
		{
            if(this.edge) _this.edgeApp.close();
			if (_this.QueuePost.length > 0)
            {
				_this.StopAll();
            }
		});
	};

	this.loadAuto = function ()
	{
	    var dom = $(document.body).append(this.GetHtmlContainer());
	    this.initUI(dom);
	};

	//加载容器，上传面板，文件列表面板
	this.load_to = function(oid)
    {
        var dom = $("#" + oid).append(this.GetHtmlContainer());
        //dom = $("#" + oid);
	    this.initUI(dom);
	};

	this.initUI = function (dom)
	{
        this.parter  = dom.find('embed[name="ffParter"]').get(0);
        this.ieParter= dom.find('object[name="parter"]').get(0);
        this.Droper = dom.find('object[name="droper"]').get(0);
        this.ui.file = dom.find("div[name='file']");
        this.ui.list = dom.find("div[name='list']");

	    //var panel = filesLoc.html(this.GetHtmlFiles());

	    ////添加多个文件
	    //panel.find('span[name="btnAddFiles"]').click(function () { _this.openFile(); });
	    ////添加文件夹
     //   panel.find('span[name="btnAddFolder"]').click(function () { _this.openFolder(); });
	    ////粘贴文件
     //   panel.find('span[name="btnPasteFile"]').click(function () { _this.pasteFiles(); });
	    ////清空已完成文件
     //   panel.find('span[name="btnClear"]').click(function () { _this.ClearComplete(); })
     //       .hover(function () {
     //           $(this).addClass("btn-footer-hover");
     //       }, function () {
     //           $(this).removeClass("btn-footer-hover");
     //       });

	    this.SafeCheck();

        $(function ()
        {
            if (!_this.edge) {
                if (_this.ie) {
                    _this.parter = _this.ieParter;
                    if (null != _this.Droper) _this.Droper.recvMessage = _this.recvMessage;
                }
                _this.parter.recvMessage = _this.recvMessage;
            }

            if (_this.edge) {
                _this.edgeApp.connect();
            }
            else {
                _this.app.init();
            }
        });
	};

    //清除已完成文件
	this.ClearComplete = function()
	{
	    $.each(this.arrFilesComplete, function (i, n) { n.remove(); });
	    this.arrFilesComplete.length = 0;
	};

	//上传队列是否已满
	this.IsPostQueueFull = function()
	{
		//目前只支持同时上传三个文件
	    return (_this.QueuePost.length + 1) > this.Config.QueueCount;
	};

	//添加到上传队列
	this.AppendQueuePost = function(fid)
	{
	    _this.QueuePost.push(fid);
	};

    //从上传队列删除
	this.RemoveQueuePost = function (fid) {
	    if (_this.QueuePost.length < 1) return;
	    this.QueuePost = $.grep(this.QueuePost, function (n, i) {
            return n == fid;
        }, true);
	};
	
	//添加到上传队列
	this.AppendQueue = function(fid)
	{
		_this.QueueFiles.push(fid);
	};

	//从队列中删除
	this.RemoveQueue = function(fid)
	{ 
	    if (this.QueueFiles.length < 1) return;
	    this.QueueFiles = $.grep(this.QueueFiles, function (n, i) {
            return n == fid;
        }, true);
	};
	
	//添加到未上传ID列表，(停止，出错)
	this.AppendQueueWait = function(fid)
	{
		_this.QueueWait.push(fid);
	};
	
	//从未上传ID列表删除，(上传完成)
	this.RemoveQueueWait = function(fid)
	{ 
	    if (this.QueueWait.length < 1) return;
	    this.QueueWait = $.grep(this.QueueWait, function (n, i) {
            return n == fid;
        }, true);
	};

	//停止所有上传项
	this.StopAll = function()
	{
		for (var i = 0, l = _this.QueuePost.length; i < l; ++i)
		{
			_this.filesMap[_this.QueuePost[i]].stop_manual();
		}
		_this.QueuePost.length = 0;
	};

	//传送当前队列的第一个文件
	this.PostFirst = function ()
	{
		//上传列表不为空
		if (_this.QueueFiles.length > 0)
		{
		    while (_this.QueueFiles.length > 0)
			{
				//上传队列已满
				if (_this.IsPostQueueFull()) return;
				var index = _this.QueueFiles.shift();
				_this.filesMap[index].post();
				_this.working = true;//
			}
		}
	};
	
	//启动下一个传输
	this.PostNext = function()
	{
		if (this.IsPostQueueFull()) return; //上传队列已满

		if (this.QueueFiles.length > 0)
		{
		    var index = this.QueueFiles.shift();
			var obj = this.filesMap[index];

			//空闲状态
			if (HttpUploaderState.Ready == obj.State)
			{
				obj.post();
			}
		} //全部上传完成
		else
		{
		    if (this.QueueFiles.length == 0//文件队列为空
                && this.QueuePost.length == 0//上传队列为空
                && this.QueueWait.length == 0)//等待队列为空
			{
		        if (this.working)
		        {
		            this.event.queueComplete();
		            this.working = false;
		        }
			}
		}
	};

	/*
	验证文件名是否存在
	参数:
	[0]:文件名称
	*/
	this.Exist = function()
	{
		var fn = arguments[0];

		for (a in _this.filesMap)
		{
		    var fileSvr = _this.filesMap[a].fileSvr;
		    if (_this.filesMap[a].isFolder) fileSvr = _this.filesMap[a].folderSvr;
		    if (fileSvr.pathLoc == fn)
		    {
		        return true;
		    }
		}
		return false;
	};

	/*
	根据ID删除上传任务
	参数:
		fid 上传项ID。唯一标识
	*/
	this.Delete = function(fid)
	{
		//_this.RemoveQueue(fid); //从队列中删除
		_this.RemoveQueueWait(fid);//从未上传列表中删除
	};

	/*
	判断文件类型是否需要过滤
	根据文件后缀名称来判断。
	*/
	this.NeedFilter = function(fname)
    {
        if (this.Config.FileFilter == "*") return false;
        var exArr = fname.split(".");
        var ext = exArr[exArr.length - 1].toLowerCase();//扩展名
        var allowExt = this.Config.FileFilter.split(",");
        for (var i = 0, l = allowExt.length; i < l; ++i)
        {
            if (allowExt[i].toLowerCase() == ext) return false;
        }
        return true;
	};
	
	//打开文件选择对话框
	this.openFile = function()
	{
        this.app.openFiles();
	};
	
	//打开文件夹选择对话框
	this.openFolder = function()
	{
        _this.app.openFolders();
	};

	//粘贴文件
	this.pasteFiles = function()
    {
        _this.app.pasteFiles();
	};

	this.ResumeFile = function (fileSvr)
	{
	    //本地文件名称存在
	    if (_this.Exist(fileSvr.pathLoc)) return;
	    var uper = this.addFileLoc(fileSvr);

	    setTimeout(function () { _this.PostFirst();},500);
    };

    this.find_ui = function (obj) {
        var tmp = {
            msg: obj.find(this.ui.ele.msg)
            , name: obj.find(this.ui.ele.name)
            , ico_file: obj.find(this.ui.ele.ico_file)
            , ico_folder: obj.find(this.ui.ele.ico_folder)
            , ico_ok: obj.find(this.ui.ele.ico_ok)
            , process: obj.find(this.ui.ele.process)
            , size: obj.find(this.ui.ele.size)
            , path: obj.find(this.ui.ele.path)
            , percent: obj.find(this.ui.ele.percent)
            , btn: {
                del: obj.find(this.ui.ele.btn_del)
                , cancel: obj.find(this.ui.ele.btn_cancel)
                , post: obj.find(this.ui.ele.btn_post)
                , stop: obj.find(this.ui.ele.btn_stop)
            }
            ,div:obj
        }
        return tmp;
    };

	//fileLoc:name,id,ext,size,length,pathLoc,md5,lenSvr,id
	this.addFileLoc = function(fileLoc)
	{
		//此类型为过滤类型
		if (_this.NeedFilter(fileLoc.ext)) return;

		var nameLoc = fileLoc.nameLoc;
		_this.AppendQueue(fileLoc.id);//添加到队列

		var ui = _this.ui.file.clone();//文件信息
		_this.ui.list.append(ui);//添加文件信息
        ui.css("display", "block");
        ui = this.find_ui(ui);
		var upFile = new FileUploader(fileLoc, _this);
        this.filesMap[fileLoc.id] = upFile;//添加到映射表
        upFile.ui = ui;
        
		ui.name.text(nameLoc).attr("title", nameLoc);
        ui.size.text(fileLoc.sizeLoc);
        ui.percent.text("(0%)");
        ui.btn.cancel.show();
        ui.btn.cancel.click(function()
		{
			upFile.stop();
			upFile.remove();
			_this.PostFirst();//
		});
        ui.btn.post.click(function ()
		{
		    btnPost.hide();
		    btnDel.hide();
		    btnCancel.hide();
		    btnStop.show();
		    if (!_this.IsPostQueueFull())
		    {
		        upFile.post();
		    }
		    else
		    {
		        upFile.Ready();
		        //添加到队列
                _this.AppendQueue(fileLoc.id);
		    }
		});
        ui.btn.stop.click(function ()
		{
		    upFile.stop();
		});
        ui.btn.del.click(function () { upFile.remove(); });
		
		upFile.Ready(); //准备
		return upFile;
	};

	//添加文件夹,json为文件夹信息字符串
	this.addFolderLoc = function (json)
	{
	    var fdLoc = json;
		//本地文件夹存在
	    //if (this.Exist(fdLoc.pathLoc)) return;
        //针对空文件夹的处理
        if (json.files == null) jQuery.extend(fdLoc, { files: [] });

		this.AppendQueue(json.id);//添加到队列

		var ui = this.ui.file.clone();//文件夹信息
		this.ui.list.append(ui);//添加到上传列表面板
		ui.css("display", "block");
        ui = this.find_ui(ui);
        ui.ico_folder.removeClass("hide");
        ui.ico_file.addClass("hide");

        ui.msg.text("(0%)");
        ui.process.css("width",fdLoc.perSvr);
        ui.name.text(fdLoc.nameLoc);
        ui.name.attr("title", fdLoc.nameLoc + "\n文件：" + fdLoc.files.length + "\n文件夹：" + fdLoc.foldersCount + "\n大小：" + fdLoc.sizeLoc);
        ui.size.text("0字节");

        var fdTask = new FolderUploader(fdLoc, this);
        fdTask.ui = ui;
        this.filesMap[fdLoc.id] = fdTask;//添加到映射表
        fdTask.ui.btn.cancel.click(function ()
		{
			fdTask.stop();
			fdTask.remove();
							
	    });
        fdTask.ui.btn.post.click(function ()
	    {
	        btnPost.hide();
	        btnDel.hide();
	        btnCancel.hide();
	        btnStop.show();

	        if (!_this.IsPostQueueFull())
	        {
	            fdTask.post();
	        }
	        else
	        {
	            fdTask.Ready();
	            _this.AppendQueue(fdTask.id);
	        }
	    });
        fdTask.ui.btn.stop.click(function ()
	    {
	        fdTask.stop();
	    });
        fdTask.ui.btn.del.click(function(){fdTask.remove();});
		fdTask.Ready(); //准备
		return fdTask;
	};

	this.ResumeFolder = function (fileSvr)
	{
	    var fd = this.addFolderLoc(fileSvr);
        fd.folderInit = true;
        fd.Scaned = true;
        fd.ui.size.text(fileSvr.sizeLoc);
        fd.ui.percent.text(fileSvr.perSvr);
	    //
		if (null == fileSvr.files)
		{
		    alert("文件为空");
		    return;
		}
	};
}