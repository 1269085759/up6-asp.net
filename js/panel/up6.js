/*
	版权所有 2009-2021 荆门泽优软件有限公司
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
    版本：2.3.9
*/
var up6_err_solve = {
    errFolderCreate: "请检查UrlFdCreate地址配置是否正确\n请检查浏览器缓存是否已更新\n请检查数据库是否创建\n请检查数据库连接配置是否正确"
    , errFolderComplete: "请检查UrlFdComplete地址配置是否正确\n请检查浏览器缓存是否已更新\n请检查数据库是否创建\n请检查数据库连接配置是否正确"
    , errFileComplete: "请检查UrlComplete地址配置是否正确\n请检查浏览器缓存是否已更新"
};

function debugMsg(m) { $("#msg").append(m); }
function HttpUploaderMgr()
{
	var _this = this;
	this.Config = {
		  "EncodeType"		: "utf-8"
		, "Company"			: "荆门泽优软件有限公司"
		, "Version"			: page.path.version.up6
		, "License2"		: page.path.license2.up6
		, "Authenticate"	: ""//域验证方式：basic,ntlm
		, "AuthName"		: ""//域帐号
		, "AuthPass"		: ""//域密码
        , "CryptoType"      : "md5"//验证方式：md5,sha1,crc
		, "security":{
			"encrypt":page.path.security.encrypt,
			"key":page.path.security.key,
            "token":page.path.security.token
		}
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
        , "AutoConnect"     : {opened:false,time:3000}//启动错误自动重传
		//文件夹操作相关
		, "UrlFdCreate"		: page.path.root+"db/fd_create.aspx"
		, "UrlFdComplete"	: page.path.root+"db/fd_complete.aspx"
		, "UrlFdDel"	    : page.path.root+"db/fd_del.aspx"
		//文件操作相关
		, "UrlCreate"		: page.path.root+"db/f_create.aspx"
		, "UrlPost"			: page.path.root+"db/f_post.aspx"
        , "UrlProcess"		: page.path.root+"db/f_process.aspx"
		, "UrlComplete"		: page.path.root+"db/f_complete.aspx"
		, "UrlList"			: page.path.root+"db/f_list.aspx"
		, "UrlDel"			: page.path.root+"db/f_del.aspx"
	    //x86
        , ie: {
              drop: { clsid: "0868BADD-C17E-4819-81DE-1D60E5E734A6", name: "Xproer.HttpDroper6" }
            , part: { clsid: "BA0B719E-F4B7-464b-A664-6FC02126B652", name: "Xproer.HttpPartition6" }
            , path: page.path.plugin.up6.ie32
        }
	    //x64
        , ie64: {
              drop: { clsid: "7B9F1B50-A7B9-4665-A6D1-0406E643A856", name: "Xproer.HttpDroper6x64" }
            , part: { clsid: "307DE0A1-5384-4CD0-8FA8-500F0FFEA388", name: "Xproer.HttpPartition6x64" }
            , path: page.path.plugin.up6.ie64
        }
        , firefox: { name: "", type: "application/npHttpUploader6", path: page.path.plugin.up6.firefox }
        , chrome: { name: "npHttpUploader6", type: "application/npHttpUploader6", path: page.path.plugin.up6.chr }
        , exe: { path: page.path.plugin.up6.exe }
        , mac: { path: page.path.plugin.up6.mac }
        , linux: { path: page.path.plugin.up6.linux }
        , edge: {protocol:"up6",port:9100,visible:false}
        , arm64: { path: page.path.plugin.up6.arm64 }
        , mips64: { path: page.path.plugin.up6.mips64 }
		, "SetupPath": "http://localhost:4955/demoAccess/js/setup.htm"
        , "Fields": { "uname": "test", "upass": "test", "uid": "0" }
        , ui: {
            icon: {
                upFile: page.path.root + "js/16/upload.png",
                upFolder: page.path.root + "js/16/folder.png",
                paste: page.path.root + "js/16/paste.png",
                clear: page.path.root + "js/16/clear.png",
                file: page.path.root + "js/file.png",
                folder: page.path.root + "js/folder.png",
                stop: page.path.root + "js/stop.png",
                del: page.path.root + "js/del.png",
                post: page.path.root + "js/post.png"
            }
        }
        , errCode: {
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
            , "11": "服务器返回数据错误"
            , "12": "连接服务器失败"
            , "13": "请求超时"
            , "14": "上传地址错误"
            , "15": "文件块MD5不匹配"
            , "16": "读取文件夹配置信息失败"
            , "17": "文件被占用"
            , "100": "服务器错误"
        }
        , state: {
            Ready: 0,
            Posting: 1,
            Stop: 2,
            Error: 3,
            GetNewID: 4,
            Complete: 5,
            WaitContinueUpload: 6,
            None: 7,
            Waiting: 8
            , MD5Working: 9
            , scan: 10
        }
	};

    //biz event
	this.event = {
	    md5Complete: function (obj/*HttpUploader对象*/, md5) { },
        scanComplete: function (obj/*文件夹扫描完毕，参考：FolderUploader*/) { },
        fileComplete: function (obj/*文件上传完毕，参考：FileUploader*/) { },
        fdComplete: function (obj/*文件夹上传完毕，参考：FolderUploader*/) { },
        queueComplete: function () {/*队列上传完毕*/ },
        addFdError: function (json) {/*添加文件夹失败*/ },
        unsetup: function (html) {/*控件未安装事件*/ }
	};
    this.data = {
		browser: {name:navigator.userAgent.toLowerCase(),ie:true,ie64:false,firefox:false,chrome:false,edge:false,arm64:false,mips64:false},
		cmps:[]/**已上传完的文件对象列表 */
	};

	//http://www.ncmem.com/
	this.Domain = "http://" + document.location.host;
    this.working = false;
    this.edgeInited = false;

    this.FileFilter = this.Config.FileFilter.split(","); //文件过滤器
	this.filesMap = new Object(); //本地文件列表映射表
	this.QueueFiles = new Array();//文件队列，数据:id1,id2,id3
	this.QueueWait = new Array(); //等待队列，数据:id1,id2,id3
	this.QueuePost = new Array(); //上传队列，数据:id1,id2,id3
    this.filesUI = null;//上传列表面板
    this.ieParter = null;
	this.parter = null;
	this.Droper = null;
	this.tmpFile = null;
	this.tmpFolder = null;
	this.uiSetupTip = null;
	this.btnSetup = null;
    //检查版本 Win32/Win64/Firefox/Chrome
	this.data.browser.ie = this.data.browser.name.indexOf("msie") > 0;
    //IE11检查
	this.data.browser.ie = this.data.browser.ie ? this.data.browser.ie : this.data.browser.name.search(/(msie\s|trident.*rv:)([\w.]+)/) != -1;
	this.data.browser.firefox = this.data.browser.name.indexOf("firefox") > 0;
	this.data.browser.chrome = this.data.browser.name.indexOf("chrome") > 0;
	this.data.browser.mips64 = this.data.browser.name.indexOf("mips64")>0;
	this.data.browser.arm64 = this.data.browser.name.indexOf("aarch64")>0;
	this.data.browser.edge = this.data.browser.name.indexOf("edge") > 0;
    this.pluginInited = false;
    this.edgeApp = new WebServerUp6(this);
    this.edgeApp.ent.on_close = function () { _this.socket_close(); };
    this.app = up6_app;
    this.app.edgeApp = this.edgeApp;
    this.app.Config = this.Config;
    this.app.ins = this;
	if (this.data.browser.edge) { this.data.browser.ie = this.data.browser.firefox = this.data.browser.chrome = false;}
    
	//容器的HTML代码
	this.GetHtmlContainer = function()
	{
	    //npapi
	    var com = "";
	    if (this.data.browser.ie)
	    {
	        //拖拽组件
	        com += '<object name="droper" classid="clsid:' + this.Config.ie.drop.clsid + '"';
	        com += ' codebase="' + this.Config.ie.path + '#version=' + this.Config.Version + '" width="192" height="192" >';
	        com += '</object>';
	    }
	    if (this.data.browser.edge) com = '';
	    //文件夹选择控件
	    com += '<object name="parter" classid="clsid:' + this.Config.ie.part.clsid + '"';
	    com += ' codebase="' + this.Config.ie.path + '#version=' + this.Config.Version + '" width="1" height="1" ></object>';

        com += '<div name="files-panel" class="files-panel">';

        //文件模板
        com += '<div class="file-item" name="fileItem">\
                    <div class="img-box"><img name="file"/></div>\
					<div class="area-l">\
						<div class="file-head">\
						    <div name="fileName" class="name">HttpUploader程序开发.pdf</div>\
						    <div name="percent" class="percent">(35%)</div>\
						    <div name="fileSize" class="size" child="1">1000.23MB</div>\
                        </div>\
						<div class="process-border"><div name="process" class="process"></div></div>\
						<div name="msg" class="msg top-space">15.3MB 20KB/S 10:02:00</div>\
					</div>\
					<div class="area-r">\
                        <span class="btn-box hide" name="post" title="继续"><img name="post"/><div>继续</div></span>\
                        <span class="btn-box" name="cancel" title="取消"><img name="stop"/><div>取消</div></span>\
						<span class="btn-box hide" name="stop" title="停止"><img name="stop"/><div>停止</div></span>\
						<span class="btn-box hide" name="del" title="删除"><img name="del"/><div>删除</div></span>\
					</div>';
        com += '</div>';
        //文件夹模板
        com += '<div class="file-item" name="folderItem">\
					<div class="img-box"><img name="folder"/></div>\
					<div class="area-l">\
						<div class="file-head">\
						    <div name="fileName" class="name">HttpUploader程序开发.pdf</div>\
						    <div name="percent" class="percent">(35%)</div>\
						    <div name="fileSize" class="size" child="1">1000.23MB</div>\
                        </div>\
						<div class="process-border top-space"><div name="process" class="process"></div></div>\
						<div name="msg" class="msg top-space">15.3MB 20KB/S 10:02:00</div>\
					</div>\
					<div class="area-r">\
                        <span class="btn-box hide" name="post" title="继续"><img name="post"/><div>继续</div></span>\
                        <span class="btn-box" name="cancel" title="取消"><img name="stop"/><div>取消</div></span>\
						<span class="btn-box hide" name="stop" title="停止"><img name="stop"/><div>停止</div></span>\
						<span class="btn-box hide" name="del" title="删除"><img name="del"/><div>删除</div></span>\
					</div>';
        com += '</div>';
        //上传列表
        com += '<div name="post_panel">\
					<div name="post_head" class="files-toolbar">\
						<span class="tool-btn" name="btnAddFiles"><img name="upFile"/>上传文件</span>\
						<span class="tool-btn" name="btnAddFolder"><img name="upFolder"/>上传目录</span>\
						<span class="tool-btn" name="btnPasteFile"><img name="paste"/>粘贴</span>\
						<span class="tool-btn" name="btnSetup">安装控件</span>\
						<span class="tool-btn" name="btnClear"><img name="clear"/>清除已完成</span>\
					</div>\
					<div name="post_content">\
						<div name="post_body" class="files-list"></div>\
					</div>\
				</div>';
        com += '</div>';
	    return com;
	};
	
	//api
	this.addFile = function(f){
        if (!this.pluginCheck()) return;
		this.app.addFile({ pathLoc: f });		
	};

	this.addFolder = function(f){
        if (!this.pluginCheck()) return;
		this.app.addFolder({ pathLoc: f });		
	};

	//打开文件选择对话框
	this.openFile = function()
	{
        if (!this.pluginCheck()) return;
        this.app.openFiles();
	};
	
	//打开文件夹选择对话框
	this.openFolder = function()
	{
        if (!this.pluginCheck()) return;
        this.app.openFolders();
	};

	//粘贴文件
	this.pasteFiles = function()
	{
        if (!this.pluginCheck()) return;
        this.app.pasteFiles();
	};

    //加载未完成列表
    this.load_files = function () {
        var param = $.extend({}, this.Config.Fields, { time: new Date().getTime() });
        $.ajax({
            type: "GET"
            , dataType: 'jsonp'
            , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
            , url: this.Config["UrlList"]
            , data: param
            , success: function (msg) {
                if (msg.value != null) {
                    var files = JSON.parse(decodeURIComponent(msg.value));
                    $.each(files, function (i, n) {
                        if (n.fdTask)
                        {
                            var fd = _this.addFolderLoc(n);
                            if (null == fd) return;
                            fd.folderInit = true;
                            fd.Scaned = true;
                            fd.ui.percent.text("(" + n.perSvr + ")");
                            fd.ui.process.css("width", n.perSvr);
                            fd.ui.btn.post.show();
                            fd.ui.btn.cancel.show();
                            _this.RemoveQueue(fd.fileSvr.id);
                        }
                        else
                        {
                            var f = _this.addFileLoc(n);
                            if (null == f) return;
                            f.ui.percent.text("(" + n.perSvr + ")");
                            f.ui.process.css("width", n.perSvr);
                            f.ui.btn.post.show();
                            f.ui.btn.cancel.show();
                            _this.RemoveQueue(f.fileSvr.id);
                        }
                    });
                }
            }
            , error: function (req, txt, err) { alert("加载文件列表错误！" + req.responseText); }
            , complete: function (req, sta) { req = null; }
        });
    }
    //删除文件对象
    this.del_file = function (id) {
        this.filesMap[id].fileSvr.pathLoc = "";
    };
	this.set_config = function (v) { $.extend(this.Config, v);};

	//msg
	this.open_files = function (json)
	{
	    for (var i = 0, l = json.files.length; i < l; ++i)
        {
	        this.addFileLoc(json.files[i]);
	    }
	    setTimeout(function () { _this.PostFirst(); },500);
	};
	this.open_folders = function (json)
    {
        for (var i = 0, l = json.folders.length; i < l; ++i) {
            this.addFolderLoc(json.folders[i]);
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
        if (this.edgeInited) return;
        this.edgeInited = true;
        this.pluginInited = true;

        this.btnSetup.hide();
        var needUpdate = true;
        if (typeof (json.version) != "undefined") {
            if (json.version == this.Config.Version) {
                needUpdate = false;
            }
        }
        if (needUpdate) this.update_notice();
        else { this.btnSetup.hide(); }
        setTimeout(function () {
            _this.load_files();
        }, 400);
    };
	this.load_complete_edge = function (json)
    {
        this.pluginInited = true;
        this.btnSetup.hide();
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

    this.pluginLoad = function () {
        if (!this.pluginInited) {
            if (this.data.browser.edge) {
                this.edgeApp.connect();
            }
        }
    };
    this.pluginCheck = function () {
        if (!this.pluginInited) {
            var link = "<a href='%url%' style='text-decoration:underline'>安装控件</a>".replace("%url%",this.Config.exe.path);
            var html = '控件没有加载成功，请%link%或等待加载。'.replace("%link%",link);
            this.event.unsetup(html);
            alert("控件没有加载成功，请安装控件或等待加载。");
            this.pluginLoad();
            return false;
        }
        return true;
    };

	this.checkBrowser = function ()
	{
	    //Win64
	    if (window.navigator.platform == "Win64")
	    {
	        $.extend(this.Config.ie, this.Config.ie64);
        }//macOS
        else if (window.navigator.platform == "MacIntel") {
            this.data.browser.edge = true;
            this.app.postMessage = this.app.postMessageEdge;
            this.edgeApp.run = this.edgeApp.runChr;
            this.Config.exe.path = this.Config.mac.path;
        }//linux
        else if (window.navigator.platform == "Linux x86_64") {
            this.data.browser.edge = true;
            this.app.postMessage = this.app.postMessageEdge;
            this.edgeApp.run = this.edgeApp.runChr;
            this.Config.exe.path = this.Config.linux.path;
        }//Linux aarch64
        else if (this.data.browser.arm64)
        {
            this.data.browser.edge = true;
            this.app.postMessage = this.app.postMessageEdge;
            this.edgeApp.run = this.edgeApp.runChr;
            this.Config.exe.path = this.Config.arm64.path;
		}//Linux mips64
        else if (this.data.browser.mips64)
        {
            this.data.browser.edge = true;
            this.app.postMessage = this.app.postMessageEdge;
            this.edgeApp.run = this.edgeApp.runChr;
            this.Config.exe.path = this.Config.mips64.path;
		}
	    else if (this.data.browser.firefox)
	    {
			this.data.browser.edge = true;
			this.app.postMessage = this.app.postMessageEdge;
			this.edgeApp.run = this.edgeApp.runChr;
	    }
	    else if (this.data.browser.chrome)
	    {
			this.data.browser.edge = true;
            this.app.check = this.app.checkFF;
	        $.extend(this.Config.firefox, this.Config.chrome);
			this.app.postMessage = this.app.postMessageEdge;
			this.edgeApp.run = this.edgeApp.runChr;
	    }
	    else if (this.data.browser.edge)
	    {
            this.app.postMessage = this.app.postMessageEdge;
	    }
	};
    this.checkBrowser();
    //升级通知
    this.update_notice = function () {
        this.btnSetup.text("升级控件");
        this.btnSetup.css("color", "red");
        this.btnSetup.show();
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
            if(this.data.browser.edge) _this.edgeApp.close();
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
	    this.initUI(dom);
	};

	this.initUI = function (dom)
	{
        var panel = dom.find('div[name="files-panel"]');
        this.parter  = dom.find('embed[name="ffParter"]').get(0);
        this.ieParter= dom.find('object[name="parter"]').get(0);
	    this.Droper  = dom.find('object[name="droper"]').get(0);

        //更新图标
        $.each(this.Config.ui.icon, function (i, n) {
            panel.find("img[name=\"" + i + "\"]").attr("src", n);
        });

	    var post_body       = dom.find("div[name='post_body']");
        var post_head       = dom.find('div[name="post_head"]');        
        post_body.height(panel.height() - post_head.height());

	    this.filesUI        = post_body;
	    this.tmpFile        = panel.find('div[name="fileItem"]');
	    this.tmpFolder      = panel.find('div[name="folderItem"]');
        this.btnSetup       = panel.find('span[name="btnSetup"]').click(function () {
            window.open(_this.Config.exe.path);
        });//("href",this.Config.exe.path);
	    //drag files

        panel.find('span[class="tool-btn"]').each(function ()
        {
            $(this).hover(function () {
                $(this).addClass("tool-btn-hover");
            }, function () {
                $(this).removeClass("tool-btn-hover");
            });
        });
	    //添加多个文件
	    panel.find('span[name="btnAddFiles"]').click(function () { _this.openFile(); });
	    //添加文件夹
        panel.find('span[name="btnAddFolder"]').click(function () { _this.openFolder(); });
	    //粘贴文件
        panel.find('span[name="btnPasteFile"]').click(function () { _this.pasteFiles(); });
	    //清空已完成文件
        panel.find('span[name="btnClear"]').click(function () { _this.ClearComplete(); })
            .hover(function () {
                $(this).addClass("tool-btn-hover");
            }, function () {
                $(this).removeClass("tool-btn-hover");
            });

	    this.SafeCheck();	    

        setTimeout(function () {
            if (!_this.data.browser.edge) {
                if (_this.data.browser.ie) {
                    _this.parter = _this.ieParter;
                    if (null != _this.Droper) _this.Droper.recvMessage = _this.recvMessage;
                }
                _this.parter.recvMessage = _this.recvMessage;
            }

            if (_this.data.browser.edge) {
                _this.edgeApp.connect();
            }
            else {
                _this.app.init();
            }
        }, 500);
	};

    //清除已完成文件
	this.ClearComplete = function()
	{
	    $.each(this.data.cmps, function (i, n) { n.remove(); });
	    this.data.cmps.length = 0;
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
			if (this.Config.state.Ready == obj.State)
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
	this.Exist = function(fn)
	{
		for (a in _this.filesMap)
		{
		    var fileSvr = _this.filesMap[a].fileSvr;
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

	this.ResumeFile = function (fileSvr)
	{
	    //本地文件名称存在
	    if (_this.Exist(fileSvr.pathLoc)) return;
	    var uper = this.addFileLoc(fileSvr);

	    setTimeout(function () { _this.PostFirst();},500);
	};

	//fileLoc:name,id,ext,size,length,pathLoc,md5,lenSvr,id
	this.addFileLoc = function(fileLoc)
	{
		//本地文件名称存在
        if (_this.Exist(fileLoc.pathLoc)) {
            alert("队列中已存在相同文件，请重新选择。");
            return null;
        }
		//此类型为过滤类型
		if (_this.NeedFilter(fileLoc.ext)) return;

		var nameLoc = fileLoc.nameLoc;
		_this.AppendQueue(fileLoc.id);//添加到队列

		var ui = _this.tmpFile.clone();//文件信息
		_this.filesUI.append(ui);//添加文件信息
		ui.css("display", "block");

		var uiName      = ui.find("div[name='fileName']");
		var uiSize      = ui.find("div[name='fileSize']")
		var uiProcess 	= ui.find("div[name='process']");
		var uiMsg 		= ui.find("div[name='msg']");
		var btnCancel 	= ui.find("span[name='cancel']");
		var btnPost 	= ui.find("span[name='post']");
		var btnStop 	= ui.find("span[name='stop']");
		var btnDel 		= ui.find("span[name='del']");
		var uiPercent	= ui.find("div[name='percent']");
		
		var upFile = new FileUploader(fileLoc, _this);
		this.filesMap[fileLoc.id] = upFile;//添加到映射表
		var ui_eles = { msg: uiMsg, process: uiProcess,percent:uiPercent, btn: { del: btnDel, cancel: btnCancel,post:btnPost,stop:btnStop }, div: ui};
        upFile.ui = ui_eles;
        $.each(ui_eles.btn, function (i, n) {
            $(n).hover(function () {
                $(this).addClass("btn-box-hover");
            }, function () {
                $(this).removeClass("btn-box-hover");
            });
        });

		uiName.text(nameLoc).attr("title", nameLoc);
		uiSize.text(fileLoc.sizeLoc);
		uiMsg.text("");
		uiPercent.text("(0%)");
		
		upFile.Ready(); //准备
		return upFile;
	};

	//添加文件夹,json为文件夹信息字符串
	this.addFolderLoc = function (json)
	{
	    var fdLoc = json;
		//本地文件夹存在
        if (this.Exist(fdLoc.pathLoc)) {
            alert("队列中已存在相同文件夹，请重新选择。");
            return null;
        }
        //针对空文件夹的处理
	    if (json.files == null) $.extend(fdLoc,{files:[]});
	    //if (json.lenLoc == 0) return;

		this.AppendQueue(json.id);//添加到队列

		var ui = this.tmpFolder.clone();//文件夹信息
		this.filesUI.append(ui);//添加到上传列表面板
		ui.css("display", "block");

		var uiName      = ui.find("div[name='fileName']");
		var uiSize      = ui.find("div[name='fileSize']")
		var divProcess 	= ui.find("div[name='process']");
		var divMsg      = ui.find("div[name='msg']");
		var btnCancel   = ui.find("span[name='cancel']");
		var btnPost     = ui.find("span[name='post']");
		var btnStop     = ui.find("span[name='stop']");
		var btnDel      = ui.find("span[name='del']");
		var divPercent	= ui.find("div[name='percent']");
		var ui_eles = { msg: divMsg,size:uiSize, process: divProcess, percent: divPercent, btn: { del: btnDel, cancel: btnCancel, post: btnPost, stop: btnStop }, div: ui };
        $.each(ui_eles.btn, function (i, e) {
            $(e).hover(function () {
            $(this).addClass("btn-box-hover");
            }, function () {
            $(this).removeClass("btn-box-hover");
            });
        });
		divPercent.text("(0%)");
		divMsg.text("");
		uiName.text(fdLoc.nameLoc);
		uiName.attr("title", fdLoc.nameLoc + "\n文件：" + fdLoc.files.length + "\n文件夹：" + fdLoc.foldersCount + "\n大小：" + fdLoc.sizeLoc);
		uiSize.text("0字节");

		var fdTask = new FolderUploader( fdLoc, this);
		this.filesMap[fdLoc.id] = fdTask;//添加到映射表
		fdTask.ui = ui_eles;
		fdTask.Ready(); //准备
		return fdTask;
	};

	this.ResumeFolder = function (fileSvr)
	{
        var fd = this.addFolderLoc(fileSvr);
        if (null == fd) return;
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