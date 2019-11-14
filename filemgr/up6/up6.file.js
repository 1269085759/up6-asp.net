﻿//文件上传对象
function FileUploader(fileLoc, mgr)
{
    var _this = this;
    this.id = fileLoc.id;
    this.ui = { msg: null, process: null, percent: null, btn: { del: null, cancel: null,post:null,stop:null }, div: null};
    this.isFolder = false; //不是文件夹
    this.app = mgr.app;
    this.Manager = mgr; //上传管理器指针
    this.event = mgr.event;
    this.Config = mgr.Config;
    this.fields = jQuery.extend({}, mgr.Config.Fields, fileLoc.fields);//每一个对象自带一个fields幅本
    this.State = this.Config.state.None;
    this.uid = this.fields.uid;
    this.fileSvr = {
          pid: ""
        , id: ""
        , pidRoot: ""
        , f_fdTask: false
        , f_fdChild: false
        , uid: 0
        , nameLoc: ""
        , nameSvr: ""
        , pathLoc: ""
        , pathSvr: ""
        , pathRel: ""
        , md5: ""
        , lenLoc: "0"
        , sizeLoc: ""
        , FilePos: "0"
        , lenSvr: "0"
        , perSvr: "0%"
        , complete: false
        , deleted: false
    };//json obj，服务器文件信息
    this.fileSvr = jQuery.extend(this.fileSvr, fileLoc);

    //准备
    this.Ready = function ()
    {
        this.ui.msg.text("正在上传队列中等待...");
        this.State = this.Config.state.Ready;
        this.ui.btn.post.click(function () {
            _this.ui.btn.post.hide();
            _this.ui.btn.del.hide();
            _this.ui.btn.cancel.hide();
            _this.ui.btn.stop.show();
            if (!_this.Manager.IsPostQueueFull()) {
                _this.post();
            }
            else {
                _this.ui.msg.text("正在上传队列中等待...");
                _this.State = _this.Config.state.Ready;
                $.each(_this.ui.btn, function (i, n) { n.hide(); });
                _this.ui.btn.del.show();
                //添加到队列
                _this.Manager.AppendQueue(_this.fileSvr.id);
            }
        });
        this.ui.btn.stop.click(function () {
            _this.stop();
        });
        this.ui.btn.del.click(function () {
            _this.stop();
            _this.remove();
        });
        this.ui.btn.cancel.click(function () {
            _this.stop();
            _this.remove();
            //_this.PostFirst();//
        });
    };

    this.svr_error = function ()
    {
        alert("服务器返回信息为空，请检查服务器配置");
        this.ui.msg.text("向服务器发送MD5信息错误");
        //this.ui.btn.cancel.text("续传");
        this.ui.btn.stop.hide();
        this.ui.btn.cancel.show();
    };
    this.svr_error_same_name = function () {        
        this.ui.msg.text("服务器存在同名文件");
        this.ui.btn.stop.hide();
        this.ui.btn.cancel.show();
    };
    this.svr_create = function (sv)
    {
        if (sv.value == null)
        {
            this.Manager.RemoveQueuePost(this.fileSvr.id);
            this.svr_error(); return;
        }
        if (!sv.ret) {
            this.Manager.RemoveQueuePost(this.fileSvr.id);
            this.svr_error_same_name(); return;
        }

        var str = decodeURIComponent(sv.value);//
        this.fileSvr = JSON.parse(str);//
        //服务器已存在相同文件，且已上传完成
        if (this.fileSvr.complete)
        {
            this.post_complete_quick();
        } //服务器文件没有上传完成
        else
        {
            this.ui.process.css("width", this.fileSvr.perSvr);
            this.ui.percent.text(this.fileSvr.perSvr);
            this.post_file();
        }
    };
    this.svr_update = function () {
        if (this.fileSvr.lenSvr == 0) return;
        var param = { uid: this.fields["uid"], offset: this.fileSvr.lenSvr, lenSvr: this.fileSvr.lenSvr, perSvr: this.fileSvr.perSvr, id: this.id, time: new Date().getTime() };
        $.ajax({
            type: "GET"
            , dataType: 'jsonp'
            , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
            , url: this.Config["UrlProcess"]
            , data: param
            , success: function (msg) {}
            , error: function (req, txt, err) { alert("更新文件进度错误！" + req.responseText); }
            , complete: function (req, sta) { req = null; }
        });
    };
    this.svr_remove = function ()
    {
        var param = { uid: this.fields["uid"], id: this.fileSvr.id, time: new Date().getTime() };
        $.ajax({
            type: "GET"
            , dataType: 'jsonp'
            , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
            , url: this.Config["UrlDel"]
            , data: param
            , success: function (msg) { }
            , error: function (req, txt, err) { alert("删除文件失败！" + req.responseText); }
            , complete: function (req, sta) { req = null; }
        });
    };
    this.post_process = function (json)
    {
        this.fileSvr.lenSvr = json.lenSvr;//保存上传进度
        this.fileSvr.perSvr = json.percent;
        this.ui.percent.text("("+json.percent+")");
        this.ui.process.css("width", json.percent);
        var str = json.lenPost + " " + json.speed + " " + json.time;
        this.ui.msg.text(str);
    };
    this.post_complete = function (json)
    {
        this.fileSvr.perSvr = "100%";
        this.fileSvr.complete = true;
        $.each(this.ui.btn, function (i, n)
        {
            n.hide();
        });
        this.ui.process.css("width", "100%");
        this.ui.percent.text("(100%)");
        this.ui.msg.text("上传完成");
        this.Manager.arrFilesComplete.push(this);
        this.State = this.Config.state.Complete;
        //从上传列表中删除
        this.Manager.RemoveQueuePost(this.fileSvr.id);
        //从未上传列表中删除
        this.Manager.RemoveQueueWait(this.fileSvr.id);

        var param = { md5: this.fileSvr.md5, uid: this.uid, id: this.fileSvr.id, time: new Date().getTime() };

        $.ajax({
            type: "GET"
			, dataType: 'jsonp'
			, jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
			, url: _this.Config["UrlComplete"]
			, data: param
			, success: function (msg)
			{
			    _this.event.fileComplete(_this);//触发事件
			    _this.post_next();
			}
			, error: function (req, txt, err) { alert("文件-向服务器发送Complete信息错误！" + req.responseText); }
			, complete: function (req, sta) { req = null; }
        });
    };
    this.post_complete_quick = function ()
    {
        this.fileSvr.perSvr = "100%";
        this.fileSvr.complete = true;
        this.ui.btn.stop.hide();
        this.ui.process.css("width", "100%");
        this.ui.percent.text("(100%)");
        this.ui.msg.text("服务器存在相同文件，快速上传成功。");
        this.Manager.arrFilesComplete.push(this);
        this.State = this.Config.state.Complete;
        //从上传列表中删除
        this.Manager.RemoveQueuePost(this.fileSvr.id);
        //从未上传列表中删除
        this.Manager.RemoveQueueWait(this.fileSvr.id);
        //添加到文件列表
        this.post_next();
        this.event.fileComplete(this);//触发事件
    };
    this.post_stoped = function (json)
    {
        this.ui.btn.post.show();
        this.ui.btn.del.show();
        this.ui.btn.cancel.hide();
        this.ui.btn.stop.hide();
        this.ui.msg.text("传输已停止....");

        if (this.Config.state.Ready == this.State)
        {
            this.Manager.RemoveQueue(this.fileSvr.id);
            this.post_next();
            return;
        }
        this.State = this.Config.state.Stop;
        //从上传列表中删除
        this.Manager.RemoveQueuePost(this.fileSvr.id);
        this.Manager.AppendQueueWait(this.fileSvr.id);//添加到未上传列表
        //传输下一个
        this.post_next();
    };
    this.post_error = function (json)
    {
        this.svr_update();
        this.ui.msg.text(this.Config.errCode[json.value]);
        this.ui.btn.stop.hide();
        this.ui.btn.post.show();
        this.ui.btn.del.show();

        this.State = this.Config.state.Error;
        //从上传列表中删除
        this.Manager.RemoveQueuePost(this.fileSvr.id);
        //添加到未上传列表
        this.Manager.AppendQueueWait(this.fileSvr.id);
        this.post_next();
    };
    this.md5_process = function (json)
    {
        var msg = "正在扫描本地文件，已完成：" + json.percent;
        this.ui.msg.text(msg);
    };
    this.md5_complete = function (json)
    {
        this.fileSvr.md5 = json.md5;
        this.ui.msg.text("MD5计算完毕，开始连接服务器...");
        this.event.md5Complete(this, json.md5);//biz event

        var loc_path = encodeURIComponent(this.fileSvr.pathLoc);
        var loc_len = this.fileSvr.lenLoc;
        var loc_size = this.fileSvr.sizeLoc;
        var param = jQuery.extend({}, this.fields, { md5: json.md5, id: this.fileSvr.id, lenLoc: loc_len, sizeLoc: loc_size, pathLoc: loc_path, time: new Date().getTime() });

        $.ajax({
            type: "GET"
            , dataType: 'jsonp'
            , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
            , url: this.Config["UrlCreate"]
            , data: param
            , success: function (sv)
            {
                _this.svr_create(sv);
            }
            , error: function (req, txt, err)
            {
                _this.Manager.RemoveQueuePost(_this.fileSvr.id);
                alert("向服务器发送MD5信息错误！" + req.responseText);
                _this.ui.msg.text("向服务器发送MD5信息错误");
                _this.ui.btn.cancel.show();
                _this.ui.btn.stop.hide();
            }
            , complete: function (req, sta) { req = null; }
        });
    };
    this.md5_error = function (json)
    {
        this.ui.msg.text(this.Config.errCode[json.value]);
        //文件大小超过限制,文件大小为0
        if ("4" == json.value
			|| "5" == json.value)
        {
        	this.ui.btn.stop.hide();
        	this.ui.btn.cancel.show();
        }
        else
        {            
            this.ui.btn.post.show();
            this.ui.btn.stop.hide();
        }
        this.State = this.Config.state.Error;
        //从上传列表中删除
        this.Manager.RemoveQueuePost(this.fileSvr.id);
        //添加到未上传列表
        this.Manager.AppendQueueWait(this.fileSvr.id);

        this.post_next();
    };
    this.post_next = function ()
    {
        var obj = this;
        setTimeout(function () { obj.Manager.PostNext(); }, 300);
    };
    this.post = function ()
    {
        this.Manager.AppendQueuePost(this.fileSvr.id);
        this.Manager.RemoveQueueWait(this.fileSvr.id);
        if (this.fileSvr.md5.length > 0 || this.fileSvr.lenSvr > 0)
        {
            this.post_file();
        }
        else
        {
            this.check_file();
        }
    };
    this.post_file = function ()
    {
        $.each(this.ui.btn, function (i, n) { n.hide();});
        this.ui.btn.stop.show();
        this.State = this.Config.state.Posting;//
        this.app.postFile({ id: this.fileSvr.id, pathLoc: this.fileSvr.pathLoc, pathSvr:this.fileSvr.pathSvr,lenSvr: this.fileSvr.lenSvr, fields: this.fields });
    };
    this.check_file = function ()
    {
        //this.ui.btn.cancel.text("停止").show();
        this.ui.btn.stop.show();
        this.ui.btn.cancel.hide();
        this.State = this.Config.state.MD5Working;
        this.app.checkFile({ id: this.fileSvr.id, pathLoc: this.fileSvr.pathLoc });
    };
    this.stop = function ()
    {
        $.each(this.ui.btn, function (i, n) { n.hide();});
        this.svr_update();
        this.app.stopFile({ id: this.fileSvr.id });        
    };
    //手动停止，一般在StopAll中调用
    this.stop_manual = function ()
    {
        if (this.Config.state.Posting == this.State)
        {
            this.svr_update();
        	this.ui.btn.post.show();
        	this.ui.btn.stop.hide();
        	this.ui.btn.cancel.hide();
            this.ui.msg.text("传输已停止....");
            this.app.stopFile({ id: this.fileSvr.id ,tip:false});
            this.State = this.Config.state.Stop;
        }
    };

    //删除，一般在用户点击"删除"按钮时调用
    this.remove = function ()
    {
        this.Manager.del_file(this.fileSvr.id);
        this.app.delFile(this.fileSvr);
        this.ui.div.remove();
        if (this.State != this.Config.state.Complete) this.svr_remove();
    };
}